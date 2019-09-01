using Microsoft.Win32;
using Scoring_Report.Configuration.Comparisons;
using System;
using System.IO;

namespace Scoring_Report.Configuration.CustomRegistry
{
    public class RegistryKey
    {
        public RegistryHive Hive = RegistryHive.LocalMachine;

        public string KeyPath = string.Empty;

        public string ValueName = string.Empty;

        public bool ValueEquals = true;

        public IComparison[] Comparisons = null;

        public string CustomOutput = string.Empty;

        public bool IsScored = false;

        public static RegistryKey Parse(BinaryReader reader)
        {
            // Create instance of policy storage
            RegistryKey registryKey = new RegistryKey();

            // Read registry identifing fields
            registryKey.Hive = (RegistryHive)reader.ReadInt32();
            registryKey.KeyPath = reader.ReadString();
            registryKey.ValueName = reader.ReadString();

            // Read comparison values
            registryKey.ValueEquals = reader.ReadBoolean();

            int comparisonCount = reader.ReadInt32();
            registryKey.Comparisons = new IComparison[comparisonCount];

            for (int i = 0; i < comparisonCount; i++)
            {
                EComparison type = (EComparison)reader.ReadInt32();
                switch (type)
                {
                    case EComparison.Simple:
                        registryKey.Comparisons[i] = new ComparisonSimple();
                        break;
                    case EComparison.Regex:
                        registryKey.Comparisons[i] = new ComparisonRegex();
                        break;
                    case EComparison.Range:
                        registryKey.Comparisons[i] = new ComparisonRange();
                        break;
                }

                registryKey.Comparisons[i].Load(reader);
            }

            // Read scoring values
            registryKey.CustomOutput = reader.ReadString();
            registryKey.IsScored = reader.ReadBoolean();

            return registryKey;
        }

        public bool TryGetComparisonValue(Microsoft.Win32.RegistryKey key, out string strValue)
        {
            strValue = null;

            // Get the key's value
            object tempObj = key.GetValue(ValueName);

            if (tempObj == null) return false;

            strValue = tempObj.ToString();

            // If the key is a binary key, get the byte array values in hex, and replace the dashes
            if (key.GetValueKind(ValueName) == RegistryValueKind.Binary)
            {
                byte[] value = (byte[])tempObj;
                string valueAsString = BitConverter.ToString(value);
                strValue = valueAsString.Replace("-", "");
            }

            return true;
        }

        public bool TryGetRegistryValue(out string strValue)
        {
            // Check 64-bit and 32-bit registries
            foreach (RegistryView view in new RegistryView[]
            {
                RegistryView.Registry64,
                RegistryView.Registry32
            })
            {
                // Create reg key base
                using (Microsoft.Win32.RegistryKey rkbase = Microsoft.Win32.RegistryKey.OpenBaseKey(Hive, view))
                {
                    // Try to open the Registry Key
                    using (Microsoft.Win32.RegistryKey key = rkbase.OpenSubKey(KeyPath))
                    {
                        // If key was found
                        if (key != null)
                        {
                            // If value exists, get comparison value
                            if (TryGetComparisonValue(key, out strValue))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            strValue = null;
            return false;
        }

        public RegistryKey() { }
    }
}
