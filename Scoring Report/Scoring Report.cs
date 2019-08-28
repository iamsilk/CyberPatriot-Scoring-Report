using Scoring_Report.Configuration;
using Scoring_Report.Scoring;
using Scoring_Report.Scoring.Output;
using System.ServiceProcess;
using System.Threading;

namespace Scoring_Report
{
    public partial class ScoringReport : ServiceBase
    {
        public bool IsRunning = false;
        private Thread LoopThread;

        private int LoopDelay = 1000;

        // Auto reset events thanks to: https://stackoverflow.com/a/2033431
        private AutoResetEvent StopRequest = new AutoResetEvent(false);

        public ScoringReport()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            // Load configuration
            string configPath = "";
            string translationsPath = "";

            if (args != null)
            {
                // Skip first command line argument, as it is the current directory
                if (args.Length > 1) configPath = args[1];

                // Second argument is translations file
                if (args.Length > 2) translationsPath = args[2];
            }

            // Setup scoring manager
            ScoringManager.Setup();

            // Start up configuration
            ConfigurationManager.Startup(configPath);

            // Start up translation manager
            TranslationManager.Startup(translationsPath);

            // Setup output manager
            OutputManager.Setup();

            // Create thread with loop function
            LoopThread = new Thread(Loop);

            // Run thread
            IsRunning = true;
            LoopThread.Start();
        }

        protected override void OnStop()
        {
            // Signal thread to stop and wait
            StopRequest.Set();
            LoopThread.Join();
        }

        private void Loop()
        {
            // Loop until stopped
            while (IsRunning)
            {
                // If configuration is loaded, check and output scores
                if (ConfigurationManager.LoadedConfigFromFile)
                    ScoringManager.CheckAndOutput();

                // Allow configuration to check for any config updates
                ConfigurationManager.Loop();

                // Allow translation manager to check for updates
                TranslationManager.Loop();

                // Delay by loop delay length of milliseconds
                // If StopRequest.Set is called during delay, will return true
                if (StopRequest.WaitOne(LoopDelay))
                {
                    IsRunning = false;
                    break;
                }
            }
        }
    }
}