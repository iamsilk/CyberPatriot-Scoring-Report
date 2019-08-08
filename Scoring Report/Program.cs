#if !DEBUG
using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
#endif

namespace Scoring_Report
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static int Main(string[] args)
        {
            // Return values:
            // -1 - Unknown
            //  0 - Success
            //  1 - No arguments specified
            //  2 - Unknown argument

            // Debugable without installing service manually.
            // Thanks to https://www.dotnetforall.com/creating-installing-windows-service/
            #if DEBUG
            ScoringReport scoringReport = new ScoringReport();
            scoringReport.OnDebug();
            return 0;
            #else

            // Program self-installation thanks to:
            // https://stackoverflow.com/a/9021540

            // If program is being ran by user
            if (System.Environment.UserInteractive)
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("No arguments specified. Process must be started as a service.");
                    Console.WriteLine("Arguments:");
                    Console.WriteLine("\t/i - Install service");
                    Console.WriteLine("\t/u - Install service");
                    return 1;
                }
                switch (args[0].ToLower())
                {
                    case "/i":
                        return InstallService();
                    case "/u":
                        return UninstallService();
                    default:
                        Console.WriteLine("Unknown argument specified: {0}", args[0]);
                        return 2;
                }
            }
            else
            {
                ServiceBase.Run(new ScoringReport());
                return 0;
            }
            #endif
        }

        #if !DEBUG
        private static int InstallService()
        {
            ScoringReport service = new ScoringReport();

            int code = 0;

            try
            {
                // Get current executing path
                string path = Assembly.GetExecutingAssembly().Location;

                // Install the service with the Windows Service Control Manager (SCM)
                ManagedInstallerClass.InstallHelper(new string[] { path });
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.GetType() == typeof(Win32Exception))
                {
                    // if Win32Exception, service is already installed
                    Win32Exception wex = (Win32Exception)ex.InnerException;
                    Console.WriteLine("Error(0x{0:X}): Service already installed!", wex.ErrorCode);
                    code = wex.ErrorCode;
                }
                else
                {
                    // Unknown error
                    Console.WriteLine(ex.ToString());
                    code = -1;
                }
            }

            // Remove file logs to reduce clutter in directory
            if (File.Exists("InstallUtil.InstallLog")) File.Delete("InstallUtil.InstallLog");
            if (File.Exists("Scoring Report.InstallLog")) File.Delete("Scoring Report.InstallLog");
            if (File.Exists("Scoring Report.InstallState")) File.Delete("Scoring Report.InstallState");

            return code;
        }

        private static int UninstallService()
        {
            ScoringReport service = new ScoringReport();

            int code = 0;

            try
            {
                // Uninstall the service from the Windows Service Control Manager (SCM)
                ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
            }
            catch (Exception ex)
            {
                if (ex.InnerException.GetType() == typeof(Win32Exception))
                {
                    // Service isn't installed
                    Win32Exception wex = (Win32Exception)ex.InnerException;
                    Console.WriteLine("Error(0x{0:X}): Service not installed!", wex.ErrorCode);
                    code = wex.ErrorCode;
                }
                else
                {
                    // Unknown error
                    Console.WriteLine(ex.ToString());
                    code = -1;
                }
            }

            // Remove file logs to reduce clutter in directory
            if (File.Exists("InstallUtil.InstallLog")) File.Delete("InstallUtil.InstallLog");
            if (File.Exists("Scoring Report.InstallLog")) File.Delete("Scoring Report.InstallLog");
            if (File.Exists("Scoring Report.InstallState")) File.Delete("Scoring Report.InstallState");

            return code;
        }
        #endif
    }
}
