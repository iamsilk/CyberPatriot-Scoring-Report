using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Scoring_Report
{
    public partial class ScoringReport : ServiceBase
    {
        public bool IsRunning = false;
        private Thread LoopThread;

        private int LoopDelay = 10000;

        // Auto reset events thanks to: https://stackoverflow.com/a/2033431
        private AutoResetEvent StopRequest = new AutoResetEvent(false);

        public ScoringReport()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
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
