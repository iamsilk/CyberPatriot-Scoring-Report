using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;

namespace Scoring_Report.Configuration.Services
{
    public class ServiceInfo : IComparable<ServiceInfo>
    {
        public string DisplayName { get; set; } = "";

        public string Name { get; set; } = "";

        public string Status { get; set; } = "";

        public static Dictionary<ServiceControllerStatus, string> Statuses { get; } = new Dictionary<ServiceControllerStatus, string>()
        {
            { ServiceControllerStatus.Running, "Running" },
            { ServiceControllerStatus.Stopped, "Stopped" },
        };

        public string StartupType { get; set; } = "";

        public static Dictionary<ServiceStartMode, string> StartupTypes { get; } = new Dictionary<ServiceStartMode, string>()
        {
            { ServiceStartMode.Automatic, "Automatic" },
            { ServiceStartMode.Disabled, "Disabled" },
            { ServiceStartMode.Manual, "Manual" },
        };

        public bool IsScored { get; set; } = false;

        public ServiceInfo(string name)
        {
            Name = name;
        }

        public int CompareTo(ServiceInfo serviceInfo)
        {
            return DisplayName.CompareTo(serviceInfo.DisplayName);
        }

        public static ServiceInfo Parse(BinaryReader reader)
        {
            // Get name
            string name = reader.ReadString();

            // Create storage for service info and all info
            ServiceInfo service = new ServiceInfo(name)
            {
                DisplayName = reader.ReadString(),
                Status = reader.ReadString(),
                StartupType = reader.ReadString(),
                IsScored = reader.ReadBoolean()
            };

            return service;
        }
    }
}
