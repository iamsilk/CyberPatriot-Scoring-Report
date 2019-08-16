using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;

namespace Configuration_Tool.Configuration.Services
{
    public class ServiceInfo : IComparable<ServiceInfo>
    {
        public string DisplayName { get; set; } = "";

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

        public void Write(BinaryWriter writer)
        {
            // Write all info
            writer.Write(Name);
            writer.Write(DisplayName);
            writer.Write(Status);
            writer.Write(StartupType);
            writer.Write(IsScored);
        }

        public static void GetServices(IList<ServiceInfo> serviceInfos)
        {
            // Clear list
            serviceInfos.Clear();

            // Create temp list for storing the list
            List<ServiceInfo> serviceInfoTempList = new List<ServiceInfo>();

            // Gets services of local machine
            ServiceController[] services = ServiceController.GetServices();

            // For each service on local machine
            foreach (ServiceController service in services)
            {
                // Create service info storage
                ServiceInfo info = new ServiceInfo(service.ServiceName);

                // Preset values as statuses can change and may not be persistent
                info.DisplayName = GetServiceDisplayName(service.ServiceName);
                info.Status = "Stopped";
                info.StartupType = "Automatic";
                info.IsScored = false;

                // Add service info to temp list
                serviceInfoTempList.Add(info);
            }

            // Gets drivers of local machine
            services = ServiceController.GetDevices();

            // For each service on local machine
            foreach (ServiceController service in services)
            {
                // Create service info storage
                ServiceInfo info = new ServiceInfo(service.ServiceName);

                // Preset values as statuses can change and may not be persistent
                info.DisplayName = GetServiceDisplayName(service.ServiceName);
                info.Status = "Stopped";
                info.StartupType = "Automatic";
                info.IsScored = false;

                // Add service info to temp list
                serviceInfoTempList.Add(info);
            }

            // Sort the temp array
            serviceInfoTempList.Sort();
            foreach (ServiceInfo tempserv in serviceInfoTempList)
                serviceInfos.Add(tempserv);
        }
        public static string GetServiceDisplayName(string service)
        {
            try
            {
                ServiceController serviceController = new ServiceController(service);
                return serviceController.DisplayName;
            }
            catch { return null; }
        }
    }
}
