using System;
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Diagnostics;

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
                app.Run();

            }            
        }

    }
}