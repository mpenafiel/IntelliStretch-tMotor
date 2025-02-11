using System;
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows.Documents;
using System.Collections.Generic;

namespace IntelliStretch
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : System.Windows.Application
    {
        [System.STAThreadAttribute()]
        public static void Main()
        {
            bool newInstance = true;

            // Using Mutual exclusion to check if it is single instance
            using (Mutex mutex = new Mutex(true, "Rehabtek IntelliStretch App", out newInstance))
            {
                if (!newInstance) // If it is NOT the only instance
                {
                    if (MessageBox.Show("There is already another instance of IntelliStrech program running.  Do you want to restart a new instance?",
                        "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        Application.Current.Shutdown(); // Or exit this instance
                    }
                    else
                    {
                        // If choose restarting, then kill all previous instances first
                        Process current = Process.GetCurrentProcess();
                        foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                        {
                            if (process.Id != current.Id)  // For all other instances, except current one                            
                                process.Kill();
                        }
                        // Then follow same routine to create new instance
                    }
                }

                //Start new app
                SplashScreen splashScreen = new SplashScreen(@"images/IntelliStretch.png");
                splashScreen.Show(false);
                IntelliStretch.App app = new IntelliStretch.App();
                app.InitializeComponent(); // System settings initialization

                // Start splash window and main app
                splashScreen.Close(TimeSpan.FromMilliseconds(300));

                // Check default profile files
                string appPath = AppDomain.CurrentDomain.BaseDirectory;

                List<Games.GameInfo> gameProfiles = new List<Games.GameInfo>();

                // Check Game System
                string gamesFile = appPath + @"Games\gamelist.xml";

                if (!File.Exists(gamesFile))
                {
                    string gamesFolder = appPath + "Games";
                    if (!Directory.Exists(gamesFolder))   // Create settings folder if it does not exist
                        Directory.CreateDirectory(gamesFolder);


                    Utilities.SaveToXML<List<Games.GameInfo>>(gameProfiles, gamesFile);
                }
                app.Run();

            }            
        }

    }
}