using System;
using System.Collections.Generic;
using System.IO;
using System.Management;

namespace Configuration_Tool.Configuration.Features
{
    public class WindowsFeature : IComparable<WindowsFeature>
    {
        public string Name { get; set; } = "";

        public bool Installed { get; set; } = false;

        public bool IsScored { get; set; } = false;

        public WindowsFeature(string name)
        {
            Name = name;
        }

        public int CompareTo(WindowsFeature feature)
        {
            return Name.CompareTo(feature.Name);
        }

        public static void GetWindowsFeatures(IList<WindowsFeature> features)
        {
            // Clear list
            features.Clear();

            // Create Temp List
            List<WindowsFeature> windowsFeaturestemp = new List<WindowsFeature>();

            SelectQuery query = new SelectQuery("Win32_OptionalFeature");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            ManagementObjectCollection collection = searcher.Get();

            foreach (ManagementObject managementObject in collection)
            {
                // Get caption and install state
                string caption = (string)managementObject["Caption"];
                string name = (string)managementObject["Name"];
                uint installState = (uint)managementObject["InstallState"];

                // If caption is empty, use the feature's name
                if (string.IsNullOrWhiteSpace(caption)) caption = name;

                // Create feature storage
                WindowsFeature feature = new WindowsFeature(caption);

                // Set installed boolean from install state
                feature.Installed = installState == 1;

                // Add feature to list
                windowsFeaturestemp.Add(feature);
            }

            windowsFeaturestemp.Sort();

            foreach (WindowsFeature windowsFeature in windowsFeaturestemp)
                features.Add(windowsFeature);
        }

        public static WindowsFeature Parse(BinaryReader reader)
        {
            // Get name of feature
            string name = reader.ReadString();

            // Create storage of feature information
            WindowsFeature feature = new WindowsFeature(name);

            // Get install and scoring status
            feature.Installed = reader.ReadBoolean();
            feature.IsScored = reader.ReadBoolean();

            return feature;
        }

        public void Write(BinaryWriter writer)
        {
            // Write name, install, and scoring status
            writer.Write(Name);
            writer.Write(Installed);
            writer.Write(IsScored);
        }
    }
}
