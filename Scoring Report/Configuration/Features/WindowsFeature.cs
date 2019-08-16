using System.Collections.Generic;
using System.IO;
using System.Management;

namespace Scoring_Report.Configuration.Features
{
    public class WindowsFeature
    {
        public string Name { get; set; } = "";

        public bool Installed { get; set; } = false;

        public bool IsScored { get; set; } = false;

        public WindowsFeature(string name)
        {
            Name = name;
        }

        public static List<WindowsFeature> GetWindowsFeatures()
        {
            List<WindowsFeature> features = new List<WindowsFeature>();

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
                features.Add(feature);
            }

            return features;
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
    }
}
