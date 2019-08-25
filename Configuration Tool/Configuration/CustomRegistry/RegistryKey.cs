using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration_Tool.Configuration.CustomRegistry
{
    public class RegistryKey
    {
        public string customoutput = string.Empty;

        public string KeyPath = string.Empty;

        public string ValueName = string.Empty;

        public string Value = string.Empty;

        public RegistryView KeyView = RegistryView.Default;

        public RegistryHive Hive = RegistryHive.LocalMachine;

        public ScoredItem<bool> KeyEquals = new ScoredItem<bool>(false, false);
        public bool Equalsbool = false;
        public string KeyEqualsStr = string.Empty;

        public static RegistryKey Parse(BinaryReader reader)
        {
            // Create instance of policy storage
            RegistryKey registryKey = new RegistryKey();

            registryKey.customoutput = reader.ReadString();

            registryKey.KeyPath = reader.ReadString();
            registryKey.ValueName = reader.ReadString();
            registryKey.Value = reader.ReadString();
            registryKey.Hive = (RegistryHive)reader.ReadInt32();

            registryKey.KeyEquals.IsScored = reader.ReadBoolean();
            registryKey.Equalsbool = reader.ReadBoolean();
            registryKey.KeyEqualsStr = reader.ReadString();

            return registryKey;
        }

        public RegistryKey()
        {

        }

        public bool RegistryKeyExists()
        {
            try
            {
                // Create reg key base
                using (Microsoft.Win32.RegistryKey rkbase = Microsoft.Win32.RegistryKey.OpenBaseKey(Hive, RegistryView.Registry64))
                {

                    // Try to open the Registry Key
                    using (Microsoft.Win32.RegistryKey keyy = rkbase.OpenSubKey(KeyPath))
                    {
                        if (keyy != null)
                        {
                            if (keyy.GetValue(ValueName) != null)
                            {
                                KeyView = RegistryView.Registry64;
                                rkbase.Dispose();
                                return true;
                            }
                        }
                    }
                    rkbase.Dispose();
                }

                // Repeat with Registry 32
                using (Microsoft.Win32.RegistryKey rkbase = Microsoft.Win32.RegistryKey.OpenBaseKey(Hive, RegistryView.Registry32))
                {
                    using (Microsoft.Win32.RegistryKey keyy = rkbase.OpenSubKey(KeyPath))
                    {
                        if (keyy != null)
                        {
                            if (keyy.GetValue(ValueName) != null)
                            {
                                KeyView = RegistryView.Registry32;
                                rkbase.Dispose();
                                return true;
                            }
                        }
                    }
                    rkbase.Dispose();
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public string GetRegistryValue()
        {
            try
            {
                if (RegistryKeyExists())
                {
                    // Create reg key base
                    using (Microsoft.Win32.RegistryKey rkbase = Microsoft.Win32.RegistryKey.OpenBaseKey(Hive, KeyView))
                    {

                        // Try to open the Registry Key
                        using (Microsoft.Win32.RegistryKey keyy = rkbase.OpenSubKey(KeyPath))
                        {
                            if (keyy != null)
                            {
                                if (keyy.GetValue(ValueName) != null)
                                {
                                    // Get the key's value
                                    string temp = keyy.GetValue(ValueName).ToString();

                                    if (keyy.GetValueKind(ValueName) == RegistryValueKind.String
                                        || keyy.GetValueKind(ValueName) == RegistryValueKind.DWord)
                                    {
                                        rkbase.Dispose();
                                        return temp;
                                    }

                                    // If the key is a binary key, get the byte array values in hex, and replace the dashes
                                    if (keyy.GetValueKind(ValueName) == RegistryValueKind.Binary)
                                    {
                                        byte[] value = (byte[])keyy.GetValue(ValueName);
                                        string valueAsString = BitConverter.ToString(value);
                                        rkbase.Dispose();
                                        return valueAsString.Replace("-", "");
                                    }
                                }
                            }
                        }
                        rkbase.Dispose();
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
