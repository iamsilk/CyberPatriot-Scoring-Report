using Scoring_Report.Configuration;
using Scoring_Report.Configuration.UserRights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Policies
{
    public static class SecurityPolicyManager
    {
        public static SecuritySettings Settings { get; private set; } = new SecuritySettings();

        public static SeceditWrapper SeceditWrapper = null;
        
        public static void GetSeceditInfo()
        {
            SeceditWrapper = new SeceditWrapper();
            SeceditWrapper.LoadPolicy();
        }

        public static void GetPasswordPolicy()
        {
            // Outputed pointer to USER_MODALS_INFO_0 structure containing data
            IntPtr bufferPtr;
            
            // Call NetUserModalsGet for local machine and requesting password parameters (level = 0)
            uint ret = WinAPI.NetUserModalsGet(null, 0, out bufferPtr);

            // Return value is 0 on success
            if (ret == 0)
            {
                // Convert pointer to structure to accessable structure
                USER_MODALS_INFO_0 userModalsInfo0 = Marshal.PtrToStructure<USER_MODALS_INFO_0>(bufferPtr);

                Settings.AccountPolicies.PasswordPolicy.MinimumPasswordLength = userModalsInfo0.min_passwd_len;

                // Sets stored value to retrieved / 86400 as retrieved value is in seconds when our comparisons are in days
                // 24 hours   60 mins   60 seconds   
                // -------- x ------- x ---------- = 86400 seconds / day
                //   1 day     1 hour      1 min

                // If max_passwd_age is below zero, no max age is set. Set our compared age to zero
                if (userModalsInfo0.max_passwd_age < 0)
                {
                    Settings.AccountPolicies.PasswordPolicy.MaximumPasswordAge = 0;
                }
                else
                {
                    Settings.AccountPolicies.PasswordPolicy.MaximumPasswordAge = userModalsInfo0.max_passwd_age / 86400;
                }

                Settings.AccountPolicies.PasswordPolicy.MinimumPasswordAge = userModalsInfo0.min_passwd_age / 86400;

                Settings.AccountPolicies.PasswordPolicy.EnforcePasswordHistory = userModalsInfo0.password_hist_len;
            }
            
            Settings.AccountPolicies.PasswordPolicy.PasswordComplexity = int.Parse(SeceditWrapper.ParsedIniFile["System Access"]["PasswordComplexity"]);

            Settings.AccountPolicies.PasswordPolicy.ReversibleEncryption = int.Parse(SeceditWrapper.ParsedIniFile["System Access"]["ClearTextPassword"]);
        }

        public static void GetLockoutPolicy()
        {
            // Outputed pointer to USER_MODALS_INFO_3 structure containing data
            IntPtr bufferPtr;

            // Call NetUserModalsGet for local machine (null) and requesting lockout parameters (level = 3)
            uint ret = WinAPI.NetUserModalsGet(null, 3, out bufferPtr);

            // Return value is 0 on success
            if (ret == 0)
            {
                // Convert pointer to structure to accessable structure
                USER_MODALS_INFO_3 userModalsInfo3 = Marshal.PtrToStructure<USER_MODALS_INFO_3>(bufferPtr);

                // Time values are stored in seconds, converted to minutes

                Settings.AccountPolicies.LockoutPolicy.AccountLockoutDuration = userModalsInfo3.lockout_duration / 60;

                Settings.AccountPolicies.LockoutPolicy.ResetLockoutCounterAfter = userModalsInfo3.lockout_observation_window / 60;

                Settings.AccountPolicies.LockoutPolicy.AccountLockoutThreshold = userModalsInfo3.lockout_threshold;
            }
        }

        private static readonly Dictionary<POLICY_AUDIT_EVENT_TYPE, string> AuditHeaders = new Dictionary<POLICY_AUDIT_EVENT_TYPE, string>()
        {
            { POLICY_AUDIT_EVENT_TYPE.AuditCategoryAccountLogon, "Audit account logon events" },
            { POLICY_AUDIT_EVENT_TYPE.AuditCategoryAccountManagement, "Audit account management" },
            { POLICY_AUDIT_EVENT_TYPE.AuditCategoryDetailedTracking, "Audit process tracking" },
            { POLICY_AUDIT_EVENT_TYPE.AuditCategoryDirectoryServiceAccess, "Audit directory service access" },
            { POLICY_AUDIT_EVENT_TYPE.AuditCategoryLogon, "Audit logon events" },
            { POLICY_AUDIT_EVENT_TYPE.AuditCategoryObjectAccess, "Audit object access" },
            { POLICY_AUDIT_EVENT_TYPE.AuditCategoryPolicyChange, "Audit policy change" },
            { POLICY_AUDIT_EVENT_TYPE.AuditCategoryPrivilegeUse, "Audit privilege use" },
            { POLICY_AUDIT_EVENT_TYPE.AuditCategorySystem, "Audit system events" }
        };

        public static void GetAuditPolicy()
        {
            IntPtr handle;

            // Null name gets local machine
            string name = null;
            LSA_OBJECT_ATTRIBUTES objAttrib = default(LSA_OBJECT_ATTRIBUTES);

            // Get handle to local policies, returns zero on success
            if (WinAPI.LsaOpenPolicy(ref name, ref objAttrib, 2, out handle) != 0) return;

            IntPtr buffer;
            // Query for event audit policy details
            if (WinAPI.LsaQueryInformationPolicy(handle, POLICY_INFORMATION_CLASS.PolicyAuditEventsInformation, out buffer) != 0)
            {
                // Query failed, close policy handle
                WinAPI.LsaClose(handle);

                return;
            }

            // Get event info from pointer to structure
            POLICY_AUDIT_EVENTS_INFO auditPolicyInfo = Marshal.PtrToStructure<POLICY_AUDIT_EVENTS_INFO>(buffer);

            // Create array with length of returned length of audit events
            POLICY_AUDIT_EVENT[] auditEventInfo = new POLICY_AUDIT_EVENT[auditPolicyInfo.MaximumAuditEventCount];

            // Store modifiable pointer to array
            IntPtr elementPtr = auditPolicyInfo.EventAuditingOptions;

            // For every event in array
            for (int i = 0; i < auditEventInfo.Length; i++)
            {
                // Get integer from pointer, and convert to type enum
                auditEventInfo[i] = (POLICY_AUDIT_EVENT)Marshal.PtrToStructure<int>(elementPtr);

                // Add size of integer to pointer to get next integer in array
                elementPtr += sizeof(int);

                // Get header of current audit type
                string header = AuditHeaders[(POLICY_AUDIT_EVENT_TYPE)i];
                
                // Store audit info with header
                Settings.LocalPolicies.AuditPolicy.HeaderSettingPairs[header] = auditEventInfo[i];
            }

            // Close handle and free memory
            WinAPI.LsaClose(handle);
            WinAPI.LsaFreeMemory(buffer);
        }

        public static void GetUserRightsAssignment()
        {
            IntPtr handle;

            // Null name gets local machine
            string name = null;
            LSA_OBJECT_ATTRIBUTES objAttrib = default(LSA_OBJECT_ATTRIBUTES);

            // Permissions necessary to enumerate accounts
            const uint POLICY_LOOKUP_NAMES = 0x00000800;
            const uint POLICY_VIEW_LOCAL_INFORMATION = 0x00000001;

            // Get handle to local policies, returns zero on success
            if (WinAPI.LsaOpenPolicy(ref name, ref objAttrib, POLICY_LOOKUP_NAMES | POLICY_VIEW_LOCAL_INFORMATION, out handle) != 0) return; //987135

            // For each scored definition, retrieve details for them
            foreach (UserRightsDefinition definition in ConfigurationManager.UserRightsDefinitions)
            {
                // Get unicode string from constant name
                LSA_UNICODE_STRING privileges = new LSA_UNICODE_STRING(definition.ConstantName);

                // Create managed "pointer" to string as function takes pointer to unicode string
                LSA_UNICODE_STRING[] pointer = new LSA_UNICODE_STRING[1];
                pointer[0] = privileges;

                const uint STATUS_NO_MORE_ENTRIES = 0x8000001a;

                IntPtr buffer;
                int count;
                uint ret = WinAPI.LsaEnumerateAccountsWithUserRight(handle, pointer, out buffer, out count);

                List<SecurityIdentifier> identifiers = new List<SecurityIdentifier>();

                // Returns 0 on success
                if (ret == 0)
                {
                    IntPtr current = buffer;
                    // For every index of identifiers
                    for (int i = 0; i < count; i++)
                    {
                        // Get information from pointer
                        LSA_ENUMERATION_INFORMATION identifierInfo = Marshal.PtrToStructure<LSA_ENUMERATION_INFORMATION>(current);
                        
                        // Get SID from information
                        SecurityIdentifier identifier = new SecurityIdentifier(identifierInfo.PSid);

                        // Add identifier to list
                        identifiers.Add(identifier);

                        // Add size of object to pointer to get next element of unmanaged array
                        current += Marshal.SizeOf(typeof(LSA_ENUMERATION_INFORMATION));
                    }

                    // Free memory
                    WinAPI.LsaFreeMemory(buffer);
                }
                // If this value is returned, there are no users with the specified privilege.
                // If this is not the value returned, it's some other issue. Just skip
                else if (ret != STATUS_NO_MORE_ENTRIES)
                {
                    continue;
                }

                if (Settings.LocalPolicies.UserRightsAssignment.UserRightsSetting.ContainsKey(definition.ConstantName))
                {
                    Settings.LocalPolicies.UserRightsAssignment.UserRightsSetting[definition.ConstantName] = identifiers;
                }
                else
                {
                    Settings.LocalPolicies.UserRightsAssignment.UserRightsSetting.Add(definition.ConstantName, identifiers);
                }
            }

            // Close handle
            WinAPI.LsaClose(handle);
        }
    }
}
