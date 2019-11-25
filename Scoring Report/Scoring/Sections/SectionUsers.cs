using Scoring_Report.Configuration;
using Scoring_Report.Configuration.Users;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Runtime.InteropServices;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionUsers : ISection
    {
        public ESectionType Type => ESectionType.Users;

        public string Header => TranslationManager.Translate("SectionUsers");

        public static List<UserSettings> Users { get; } = new List<UserSettings>();

        public int MaxScore()
        {
            // Set max to 0
            int max = 0;

            // For each user settings loaded in configuration
            foreach (UserSettings settings in Users)
            {
                // Check all scorable parameters and increment
                // max by one for each scored parameter
                if (settings.Exists.IsScored) max++;
                if (settings.Password.IsScored) max++;
                if (settings.PasswordExpired.IsScored) max++;
                if (settings.PasswordChangeDisabled.IsScored) max++;
                if (settings.PasswordNeverExpires.IsScored) max++;
                if (settings.AccountDisabled.IsScored) max++;
                if (settings.AccountLockedOut.IsScored) max++;
            }

            return max;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            // If no configuration for this section, return empty details
            if (Users.Count == 0) return details;

            // Create instance for communicating with active directory
            using (PrincipalContext context = new PrincipalContext(ContextType.Machine))
            {
                // Create searcher for active directory
                using (PrincipalSearcher searcher = new PrincipalSearcher(new UserPrincipal(context)))
                {
                    // For each user in configuration
                    foreach (UserSettings settings in Users)
                    {
                        // Keep a boolean value for detecting if the user exists
                        bool userexists = false;

                        // Store identifier and type temporarily for output
                        string id = "";
                        string idType = "";

                        // Used for formatting, set to settings in case user doesn't exist
                        string username = settings.Username;
                        string sid = settings.SecurityID;

                        if (settings.IdentifiedBySID)
                        {
                            id = settings.SecurityID;
                            idType = TranslationManager.Translate("SecurityID");
                        }
                        else
                        {
                            id = settings.Username;
                            idType = TranslationManager.Translate("Username");
                        }

                        // For each user on the machine
                        foreach (UserPrincipal user in searcher.FindAll())
                        {
                            // Check if users are the same
                            bool isUser = false;

                            // Set then compare so we only retrieve once
                            string tempUsername = user.SamAccountName;
                            string tempSid = user.Sid.Value;

                            if (settings.IdentifiedBySID)
                            {
                                isUser = user.Sid.MatchesConfig(settings.SecurityID);
                            }
                            else
                            {
                                if (settings.Username == tempUsername)
                                    isUser = true;
                            }

                            if (!isUser) continue;

                            // Save username/sid for later formatting
                            username = tempUsername;
                            sid = tempSid;

                            userexists = true;
                            // Check if password is scored/valid
                            if (settings.Password.IsScored)
                            {
                                // If password was set again since last check
                                if (user.LastPasswordSet.HasValue && (user.LastPasswordSet.Value - settings.PasswordLastChecked).Seconds > 0)
                                {
                                    // Set last check to current checking time
                                    settings.PasswordLastChecked = user.LastPasswordSet.Value;

                                    // https://docs.microsoft.com/en-us/dotnet/api/system.security.principal.windowsimpersonationcontext?redirectedfrom=MSDN&view=netframework-4.8
                                    const int LOGON32_PROVIDER_DEFAULT = 0;
                                    //This parameter causes LogonUser to create a primary token.
                                    const int LOGON32_LOGON_INTERACTIVE = 2;

                                    // Check if account is locked out
                                    bool accountLockedOut = user.IsAccountLockedOut();

                                    // If account is locked out, unlock to check pass
                                    if (accountLockedOut)
                                    {
                                        user.UnlockAccount();
                                    }

                                    IntPtr safeTokenHandle;
                                    // Use WinAPI to gain a logon token for user. If gained successfully, the password is correct.
                                    bool correctPass = WinAPI.LogonUser(user.SamAccountName, Environment.MachineName, settings.Password.Value,
                                        LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out safeTokenHandle);

                                    int ret = Marshal.GetLastWin32Error();
                                    Console.WriteLine("LogonUser failed with error code : {0}", ret);

                                    /* Through tests of whether the specified password is correct/incorrect
                                     * and if the account is active/inactive, the following are the results
                                     * (bool correctPass, int ret)
                                     * 
                                     * Correct Password and Active - True, 0
                                     * Correct Password and Inactive - False, 1331
                                     * Incorrect Password and Inactive - False, 1326
                                     * Incorrect Password and Active - False, 1326
                                     * 
                                     * Through this analysis, regardless if the account is active,
                                     * we should check if the error code returned is 1326, if so
                                     * the password is incorrect.
                                     */

                                    settings.PasswordLastStatus = ret != 1326;

                                    // If account was locked out
                                    if (accountLockedOut)
                                    {
                                        // Relock account

                                        string incorrectPassword = settings.Password.Value;

                                        // If password was correct
                                        if (settings.PasswordLastStatus)
                                        {
                                            if (incorrectPassword.Length > 0)
                                            {
                                                // Remove last character from password to make it incorrect
                                                incorrectPassword = incorrectPassword.Remove(incorrectPassword.Length - 1);
                                            }
                                            else
                                            {
                                                // Add character to end of password
                                                incorrectPassword = incorrectPassword + "a";
                                            }
                                        }

                                        // While account isn't locked out
                                        while (!user.IsAccountLockedOut())
                                        {
                                            // Logon with incorrect creds
                                            WinAPI.LogonUser(user.SamAccountName, Environment.MachineName, incorrectPassword,
                                                LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out safeTokenHandle);
                                        }
                                    }
                                    else
                                    {
                                        // Make sure it's unlocked
                                        user.UnlockAccount();
                                    }

                                    // If LogonUser returns true, a handle was created for us to impersonate
                                    // the user. We want to close this handle.
                                    if (correctPass)
                                    {
                                        // Close token handle
                                        WinAPI.CloseHandle(safeTokenHandle);
                                    }
                                }

                                // If password was not correct
                                if (!settings.PasswordLastStatus)
                                {
                                    details.Points++;
                                    details.Output.Add(TranslationManager.Translate("PasswordChanged", id, idType, username, sid));
                                }
                            }

                            PropertyCollection properties = ((DirectoryEntry)user.GetUnderlyingObject()).Properties;

                            // Check if each parameter is scored, if so, compare values                       
                            if (settings.PasswordExpired.IsScored && settings.PasswordExpired.Value == ((int)properties["PasswordExpired"][0] == 1))
                            {
                                details.Points++;
                                details.Output.Add(TranslationManager.Translate("PasswordExpired", id, idType, settings.PasswordExpired.Value, username, sid));
                            }
                            if (settings.PasswordChangeDisabled.IsScored && settings.PasswordChangeDisabled.Value == user.UserCannotChangePassword)
                            {
                                details.Points++;
                                details.Output.Add(TranslationManager.Translate("PasswordChangeDisabled", id, idType, settings.PasswordChangeDisabled.Value, username, sid));
                            }
                            if (settings.PasswordNeverExpires.IsScored & settings.PasswordNeverExpires.Value == user.PasswordNeverExpires)
                            {
                                details.Points++;
                                details.Output.Add(TranslationManager.Translate("PasswordNeverExpires", id, idType, settings.PasswordNeverExpires.Value, username, sid));
                            }
                            if (settings.AccountDisabled.IsScored && settings.AccountDisabled.Value == !user.Enabled)
                            {
                                details.Points++;
                                details.Output.Add(TranslationManager.Translate("AccountDisabled", id, idType, settings.AccountDisabled.Value, username, sid));
                            }
                            if (settings.AccountLockedOut.IsScored && settings.AccountLockedOut.Value == user.IsAccountLockedOut())
                            {
                                details.Points++;
                                details.Output.Add(TranslationManager.Translate("AccountLockedOut", id, idType, settings.AccountLockedOut.Value, username, sid));
                            }
                        }

                        if (settings.Exists.IsScored && settings.Exists.Value == userexists)
                        {
                            details.Points++;
                            details.Output.Add(TranslationManager.Translate("UserExists", id, idType, settings.Exists.Value, username, sid));
                        }
                    }
                }
            }

            return details;
        }

        public void Load(BinaryReader reader)
        {
            // Clear current list of user settings
            Users.Clear();

            // Number of user settings instances
            int count = reader.ReadInt32();

            // For each user settings instance
            for (int i = 0; i < count; i++)
            {
                // Parse user settings instance from binary reader
                UserSettings settings = UserSettings.Parse(reader);

                // Add user settings to main list
                Users.Add(settings);
            }
        }
    }
}
