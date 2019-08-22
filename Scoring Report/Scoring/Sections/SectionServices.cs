using Scoring_Report.Configuration;
using Scoring_Report.Configuration.Services;
using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;

namespace Scoring_Report.Scoring.Sections
{
    public class SectionServices : ISection
    {
        public string Header => TranslationManager.Translate("SectionServices");

        public List<ServiceInfo> Services { get; } = new List<ServiceInfo>();

        public ESectionType Type => ESectionType.Services;

        public int MaxScore()
        {
            return Services.Count;
        }

        public SectionDetails GetScore()
        {
            SectionDetails details = new SectionDetails(0, new List<string>(), this);

            // Gets services of local machine
            ServiceController[] services = ServiceController.GetServices();

            // Gets drivers of local machine
            ServiceController[] drivers = ServiceController.GetDevices();

            // Combine arrays
            List<ServiceController> combined = new List<ServiceController>(services);
            combined.AddRange(drivers);

            // For each service info config
            foreach (ServiceInfo info in Services)
            {
                // For each service on local machine
                foreach (ServiceController service in combined)
                {
                    // If config and service have matching names
                    if (info.Name == service.ServiceName)
                    {
                        // Check status and startup type
                        if (info.Status == ServiceInfo.Statuses[service.Status] &&
                            info.StartupType == ServiceInfo.StartupTypes[service.StartType])
                        {
                            // Set correctly, increment score and give message
                            details.Points++;
                            details.Output.Add(TranslationManager.Translate("Service", info.Name, info.Status, info.StartupType));
                        }

                        // Found match, stop looking
                        break;
                    }
                }
            }

            return details;
        }

        public void Load(BinaryReader reader)
        {
            // Clear current config
            Services.Clear();

            // Get number of service infos
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                // Get service info
                ServiceInfo service = ServiceInfo.Parse(reader);

                // Add service to list
                Services.Add(service);
            }
        }
    }
}
