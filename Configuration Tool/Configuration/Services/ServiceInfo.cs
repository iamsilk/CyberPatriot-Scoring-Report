using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;

namespace Configuration_Tool.Configuration.Services
{
    public class ServiceInfo
    {
        public string Name { get; set; } = "";

        public string Status { get; set; } = "";

        public static List<string> Statuses { get; } = new List<string>()
        {
            "Running",
            "Stopped",
        };

        public string StartupType { get; set; } = "";

        public static List<string> StartupTypes { get; } = new List<string>()
        {
            "Automatic",
            "Disabled",
            "Manual",
        };

        public bool IsScored { get; set; } = false;

        public ServiceInfo(string name)
        {
            Name = name;
        }

        public static ServiceInfo Parse(BinaryReader reader)
        {
            // Get name
            string name = reader.ReadString();

            // Create storage for service info and all info
            ServiceInfo service = new ServiceInfo(name)
            {
                Status = reader.ReadString(),
                StartupType = reader.ReadString(),
                IsScored = reader.ReadBoolean()
            };

            return service;
        }

        public void Write(BinaryWriter writer)
        {
            // Write all info
            writer.Write(Name);
            writer.Write(Status);
            writer.Write(StartupType);
            writer.Write(IsScored);
        }

        public static void GetServices(IList<ServiceInfo> serviceInfos)
        {
            // Clear list
            serviceInfos.Clear();
            
            // Gets services of local machine
            ServiceController[] services = ServiceController.GetServices();

            // For each service on local machine
            foreach (ServiceController service in services)
            {
                // Create service info storage
                ServiceInfo info = new ServiceInfo(service.ServiceName);

                // Preset values as statuses can change and may not be persistent
                info.Status = "Stopped";
                info.StartupType = "Automatic";
                info.IsScored = false;

                // Add service info to list
                serviceInfos.Add(info);
            }
        }
    }
}
