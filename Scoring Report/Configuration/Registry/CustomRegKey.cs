using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoring_Report.Configuration.Registry
{
    public class CustomRegKey
    {
        public string KeyPath = string.Empty;

        public string ValueName = string.Empty;

        public string Value = string.Empty;

        public RegistryView KeyView = RegistryView.Default;

        public RegistryHive Hive = RegistryHive.LocalMachine;

        public bool Exists = false;

        public CustomRegKey()
        {

        }

        public bool RegistryKeyExists()
        {
            try
            {
                // Create reg key base
                RegistryKey rkbase = RegistryKey.OpenBaseKey(Hive, RegistryView.Registry64);

                // Try to open the Registry Key
                using (RegistryKey keyy = rkbase.OpenSubKey(KeyPath))
                {
                    if (keyy != null)
                    {
                        if (keyy.GetValue(ValueName) != null)
                        {
                            KeyView = RegistryView.Registry64;
                            return true;
                        }
                    }
                }

                // Repeat with Registry 32
                rkbase = RegistryKey.OpenBaseKey(Hive, RegistryView.Registry32);
                using (RegistryKey keyy = rkbase.OpenSubKey(KeyPath))
                {
                    if (keyy != null)
                    {
                        if (keyy.GetValue(ValueName) != null)
                        {
                            KeyView = RegistryView.Registry32;
                            return true;
                        }
                    }
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
                // Create reg key base
                RegistryKey rkbase = RegistryKey.OpenBaseKey(Hive, KeyView);

                // Try to open the Registry Key
                using (RegistryKey keyy = rkbase.OpenSubKey(KeyPath))
                {
                    if (keyy != null)
                    {
                        if (keyy.GetValue(ValueName) != null)
                        {
                            // Get the key's value
                            string temp = keyy.GetValue(ValueName).ToString();

                            if (keyy.GetValueKind(ValueName) == RegistryValueKind.String
                                || keyy.GetValueKind(ValueName) == RegistryValueKind.DWord)
                                return temp;

                            // If the key is a binary key, get the byte array values in hex, and replace the dashes
                            if(keyy.GetValueKind(ValueName) == RegistryValueKind.Binary)
                            {
                                byte[] value = (byte[])keyy.GetValue(ValueName);
                                string valueAsString = BitConverter.ToString(value);
                                return valueAsString.Replace("-", "");
                            }
                        }
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
