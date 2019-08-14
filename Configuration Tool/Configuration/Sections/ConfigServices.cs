using Configuration_Tool.Configuration.Services;
using Configuration_Tool.Controls;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Configuration_Tool.Configuration.Sections
{
    public class ConfigServices : IConfig
    {
        public EConfigType Type => EConfigType.Services;

        public MainWindow MainWindow { get; set; }

        public void Load(BinaryReader reader)
        {
            // Get services
            ServiceInfo.GetServices(ConfigurationManager.Services);

            // Get number of service infos
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get service info
                ServiceInfo service = ServiceInfo.Parse(reader);

                // Find service with matching name
                ServiceInfo match = ConfigurationManager.Services.FirstOrDefault(x => x.Name == service.Name);

                // If match was found
                if (match != null)
                {
                    match.Status = service.Status;
                    match.StartupType = service.StartupType;
                    match.IsScored = service.IsScored;
                }
                else
                {
                    // Add service to list
                    ConfigurationManager.Services.Add(service);
                }
            }
        }

        public void Save(BinaryWriter writer)
        {
            // Get all scored services
            IEnumerable<ServiceInfo> services = ConfigurationManager.Services.Where(x => x.IsScored);

            // Write number of services
            writer.Write(services.Count());

            // For each scored service info config
            foreach (ServiceInfo service in services)
            {
                // Write service info
                service.Write(writer);
            }
        }
    }
}
