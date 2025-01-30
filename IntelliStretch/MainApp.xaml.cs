using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.IO;
using IntelliStretch.UI;       
using NationalInstruments.DAQmx;
using System.Diagnostics;

namespace IntelliStretch
{
    /// <summary>
    /// Interaction logic for MainApp.xaml
    /// </summary>
    public partial class MainApp : Window
    {

        readonly System.Windows.Forms.Timer DAQStatusTimer = new System.Windows.Forms.Timer() { Interval = 100, Enabled = true };
        static bool dispError = false;
        DigitalSingleChannelWriter LEDwriter = null;
        bool[] dataArray = new bool[8];

        #region Constructor

        public MainApp()
        {
            InitializeComponent();

            DAQStatusTimer.Tick += (s, e) =>
            {
                try
                {
                    if (intelliProtocol != null && digitalWriteTask == null)
                    {
                        digitalWriteTask = new NationalInstruments.DAQmx.Task();

                        digitalWriteTask.DOChannels.CreateChannel(intelliProtocol.DAQ.DigitalChannel, "",
                                ChannelLineGrouping.OneChannelForAllLines);

                        dataArray[0] = true;
                        dataArray[1] = false;
                        dataArray[2] = false;
                        dataArray[3] = false;
                        dataArray[4] = false;
                        dataArray[5] = false;
                        dataArray[6] = false;
                        dataArray[7] = false;

                        LEDwriter = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                        LEDwriter.WriteSingleSampleMultiLine(true, dataArray);

                        dispError = false;
                    }
                    if (LEDwriter != null)
                    {
                        if (uiEvaluation.IsDAQStreaming())
                        {
                            dataArray[0] = true;
                            dataArray[1] = true;
                            dataArray[2] = true;
                            dataArray[3] = true;
                            dataArray[4] = true;
                            dataArray[5] = true;
                            dataArray[6] = true;
                            dataArray[7] = true;

                            LEDwriter.WriteSingleSampleMultiLine(true, dataArray);
                        }
                        else
                        {
                            dataArray[0] = true;
                            dataArray[1] = false;
                            dataArray[2] = false;
                            dataArray[3] = false;
                            dataArray[4] = false;
                            dataArray[5] = false;
                            dataArray[6] = false;
                            dataArray[7] = false;

                            LEDwriter.WriteSingleSampleMultiLine(true, dataArray);
                        }
                    }
                }
                catch (DaqException ex)
                {
                    Debug.WriteLine(ex);
                    // Include message here?
                    digitalWriteTask?.Dispose();
                    digitalWriteTask = null;
                }
            };
        }
   
        #endregion

        #region Variables
     
        // Non-public
        IntelliSerialPort sp;
        Protocols.System sysProtocol;
        UserProfile currentUserProfile;

        System.Timers.Timer scanTimer;

        enum TaskMode
        {
            Preparations = 0,
            Connection = 1,
            PrelimSettings = 2,
            Stretching = 3,
            Game = 4,
            Evaluation = 5,
            Report = 6
        }
      
        List<TaskMode> taskList;
        int taskIndex;
        Protocols.IntelliProtocol intelliProtocol;

        NationalInstruments.DAQmx.Task digitalWriteTask = null;

        #endregion 

        #region Properties

        public string CurrentUserDir { get; set; }
        public string CurrentDateDir { get; set; }
        public string DataFilePrefix { get; set; }
        public Protocols.IntelliProtocol IntelliProtocol 
        {
            get { return intelliProtocol; }
            set { intelliProtocol = value; }
        }

        #endregion

        #region Dependency Properties


        #endregion

        #region Routed Events

        public event RoutedEventHandler IntroAnimBegin
        {
            add { AddHandler(IntroAnimBeginEvent, value); }
            remove { RemoveHandler(IntroAnimBeginEvent, value); }
        }

        public static readonly RoutedEvent IntroAnimBeginEvent =
            EventManager.RegisterRoutedEvent("IntroAnimBegin", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MainApp));

        #endregion


        // ---- Methods -----------

        #region Public methods

        public void Save_Protocol()
        {
            string userDefaultFile = CurrentUserDir  + "Default.xml";
            Utilities.SaveToXML<Protocols.IntelliProtocol>(intelliProtocol, userDefaultFile);
            MessageBox.Show("Current protocol has been saved!");
        }

        #endregion


        #region Initialization

        private void Initialize_Sounds()
        {
            Globals.Sound.buttonSound = Utilities.LoadSound(IntelliStretch.Globals.Sound.ButtonClickUri); // Load button sound
            Globals.Sound.clickSound = Utilities.LoadSound(IntelliStretch.Globals.Sound.SelectClickUri); // Load click sound
            Globals.Sound.pageSound = Utilities.LoadSound(IntelliStretch.Globals.Sound.PageFlipUri); // Load page flip sound
            
        }

        private void Initialize_Settings()
        {
            // Check default profile files
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            
            // Check device system
            string sysFile = appPath + "System.xml";
            if (!File.Exists(sysFile))  // If default system file does not exsit
            {
                sysProtocol = new Protocols.System();
                sysProtocol.Joint = Protocols.Joint.All;  // Default = All-In-One version
                sysProtocol.HasSensor = false;  // Default = NO sensor
                Utilities.SaveToXML<Protocols.System>(sysProtocol, sysFile);
            }
            else
                sysProtocol = Utilities.ReadFromXML<Protocols.System>(sysFile, true); // Read system settings if available


            // Check default user protocol
            string defaultFile = appPath + @"Profiles\Default.xml";

            if (!File.Exists(defaultFile)) // If default file does not exsit
            {
                string defaultFolder = appPath + "Profiles";
                if (!Directory.Exists(defaultFolder))   // Create settings folder if it does not exist
                    Directory.CreateDirectory(defaultFolder);

                // Restore to factory default settings
                Protocols.IntelliProtocol defaultProtocol = new Protocols.IntelliProtocol();
                //System default settings
                defaultProtocol.System.SamplingRate = 50;
                defaultProtocol.System.IsSavingData = false;

                //General default settings
                defaultProtocol.General.Joint = sysProtocol.Joint;
                defaultProtocol.General.FlexionMax = 10;
                defaultProtocol.General.ExtensionMax = -20;
                defaultProtocol.General.ExtraRange = 5;
                defaultProtocol.General.ActiveFlexionMax = 10;
                defaultProtocol.General.ActiveExtensionMax = -20;

                //Stretching default settings
                defaultProtocol.Stretching.Level = 10;
                defaultProtocol.Stretching.Duration = 10;
                defaultProtocol.Stretching.HoldingTime = 100;
                defaultProtocol.Stretching.FlexionVelocity = 40;
                defaultProtocol.Stretching.ExtensionVelocity = 40;
                defaultProtocol.Stretching.FlexionTorque = 10;
                defaultProtocol.Stretching.ExtensionTorque = 5;
                defaultProtocol.Stretching.FlexionTorqueMax = 30;
                defaultProtocol.Stretching.ExtensionTorqueMax = 30;

                //Game default settings
                defaultProtocol.Game.AssistiveMode.Level = 5;
                defaultProtocol.Game.AssistiveMode.Velocity = 5;
                defaultProtocol.Game.AssistiveMode.DelayTime = 3;
                // Vertial - All=0, Ankle=1;  Horizontal - Elbow=2, Wrist=4, Arm=6
                defaultProtocol.Game.ResistiveMode.ControlDirection = (((int)sysProtocol.Joint & 0x110) == 0) ? Protocols.Direction.Vertical : Protocols.Direction.Horizontal;
                defaultProtocol.Game.ResistiveMode.ScalingFactor = 20;
                defaultProtocol.Game.ResistiveMode.IsNoLoading = false;
                defaultProtocol.Game.ResistiveMode.FlexionResistance = 0;
                defaultProtocol.Game.ResistiveMode.ExtensionResistance = 0;
                defaultProtocol.Game.ResistiveMode.Resistance = 0;

                //DAQ factory default settings
                defaultProtocol.DAQ.Model = "NI USB-6008";
                defaultProtocol.DAQ.Channel1 = "Dev1/ai1";
                defaultProtocol.DAQ.Channel2 = "Dev1/ai3";
                defaultProtocol.DAQ.Channel1_Name = "Channel 1";
                defaultProtocol.DAQ.Channel2_Name = "Channel 2";
                defaultProtocol.DAQ.SamplingRate = 1000;
                defaultProtocol.DAQ.SampPerChan = 100;
                defaultProtocol.DAQ.DigitalChannel = "Dev1/port0/line0:2";

                Utilities.SaveToXML<Protocols.IntelliProtocol>(defaultProtocol, defaultFile);
            }
        }

        private void Initialize_Variables()
        {
            // Tasks
            taskList = new List<TaskMode>(new TaskMode[] {
                TaskMode.Preparations,
                TaskMode.Connection,
                TaskMode.PrelimSettings,
                TaskMode.Stretching,
                TaskMode.Evaluation,
                TaskMode.Game,
                TaskMode.Report
            });
            taskIndex = 0;

            // Serial port
            sp = new IntelliSerialPort();
            sp.IsConnected = false;

            // streamer .. assign streamer protocol here
            //streamer = new Streamer();

            // Parent window handle
            uiPreparation.Load_Preparation(this);
            uiProfileLoader.Load_ProfileLoader(this, sysProtocol.Joint, sp);
            uiGameLib.Initialize_Gamelist(this);
        }
        
        private void Initialize_Connection()
        {
            btnNext.Visibility = (sp.IsConnected) ? Visibility.Visible : Visibility.Hidden;
        }

        private void Initialize_PrelimSettings()
        {
            btnNext.Visibility = Visibility.Hidden;
            
            // Set backdrivable
            if (sp.IsConnected)
            {
                sp.WriteCmd("ST");//used to be BK, changed to ST by gio jan 16 2025
                //if (sysProtocol.HasSensor) sp.WriteCmd("MO");  // === Del by Yupeng Jan.18.2011
                sp.UpdateData = new IntelliSerialPort.DelegateUpdateData(uiPrelimSettings.Update_DataInfo);
                sp.IsUpdating = true;
            }

            // Load protcols
            uiPrelimSettings.Load_GeneralSettings(this, sp);
        }

        private void Initialize_Stretching()
        {
            btnNext.Visibility = Visibility.Visible;
            // Load protcols
            uiStretching.Load_StretchProtocol(this, sp);
            uiStretching.ModeSelectionVisibility = (sysProtocol.HasSensor) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Initialize_Games()
        {
            btnNext.Visibility = Visibility.Visible;

            uiGames.Load_GameProtocol(this, sp);
        }

        private void Initialize_Evaluation()
        {
            btnNext.Visibility = Visibility.Visible;

            uiEvaluation.Load_Evaluation(this, sp);

            string newDataDir = this.CurrentUserDir + this.CurrentDateDir;
            uiEvaluation.DataDir = newDataDir;
            uiEvaluation.DataFilePrefix = this.DataFilePrefix;

            if (!Directory.Exists(newDataDir)) Directory.CreateDirectory(newDataDir);  // Create data folder for today
        }

        private void Initialize_Report()
        {
            btnNext.Visibility = Visibility.Visible;
            uiReport.Load_Report(currentUserProfile, intelliProtocol);
        }

        private void ReInitialize_Port()
        {
            if (sp.IsConnected)
            {
                if (sp.UpdateData != null) sp.UpdateData = null;
                sp.IsUpdating = false;
            }
        }

        #endregion


        #region Preparation

        public void Begin_Preparation(UserProfile profile)
        {
            tabContent.Visibility = Visibility.Visible;
            currentUserProfile = profile;
            uiPreparation.Begin_Intro_Animation(profile.FirstName);
        }

        #endregion


        #region Connection

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            sp.UpdateStatus = new IntelliSerialPort.DelegateUpdateStatus(uiConnection.txtStatus_Update);
            sp.UpdateUI = new IntelliSerialPort.DelegateUpdateUI(uiConnection.ConnectionUI_Update);
            sp.AutoConnect();

            // Initialize commands to STM32 ???

            // set fixed digits for value

            //General Settings
            string extensionMax;
            // Check sign of max extension and set to negative
            if ( intelliProtocol.General.ExtensionMax > 0) extensionMax =  $"PF{-(intelliProtocol.General.ExtensionMax)}";
            else extensionMax = $"PF{intelliProtocol.General.ExtensionMax}";

            string flexionMax = $"DF{intelliProtocol.General.FlexionMax}";
            string extraRange = $"EX{intelliProtocol.General.ExtraRange}";

            //Stretching Settings
            string maxLevel = $"ML{intelliProtocol.Stretching.Level}";
            string holdingTime = $"HT{intelliProtocol.Stretching.HoldingTime}";
            string flexionVelocity = $"DV{intelliProtocol.Stretching.FlexionVelocity}";
            string extensionVelocity = $"PV{intelliProtocol.Stretching.ExtensionVelocity}";
            string flexionTorque = $"DT{intelliProtocol.Stretching.FlexionTorque}";
            string extensionTorque = $"PT{intelliProtocol.Stretching.ExtensionTorque}";

            //System Settings
            //string sampleRate = $"HZ{intelliProtocol.System.SamplingRate}";

            //Perform handshake, receive confirmation from STM32?

            if (sp.IsConnected)
            {
                //sp.WriteCmd(sampleRate);

                sp.WriteCmd(maxLevel);
                sp.WriteCmd(holdingTime);
                sp.WriteCmd(flexionVelocity);
                sp.WriteCmd(extensionVelocity);
                sp.WriteCmd(flexionTorque);
                sp.WriteCmd(extensionTorque);

                sp.WriteCmd(extensionMax);
                sp.WriteCmd(flexionMax);
                sp.WriteCmd(extraRange);
            }

            //if (sp.IsOpen()) sp.readLine();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // Check if AutoSave == true                                // === Add by Yue Feb.12.2011
            if (intelliProtocol.System.IsSavingData)
            {
                string newDataDir = this.CurrentUserDir + this.CurrentDateDir;
                sp.DataDir = newDataDir;
                sp.DataFilePrefix = this.DataFilePrefix;

                if (!Directory.Exists(newDataDir)) Directory.CreateDirectory(newDataDir);  // Create data folder for today
            }
            
            // === Send "MO" cmd after clicking [Start], shift from PrelimSetting page
            if (sp.IsConnected)                                         // === Add by Yupeng Jan.18.2011
            {                                                           // === Add by Yupeng Jan.18.2011
                sp.WriteCmd("ST");                                      // ===== Add by Michael, Aug.16.2024
                //sp.WriteCmd("BK");                                     // === Add by Yupeng Jan.18.2011changed to ST by gio jan 16 2025 // removed by gio 16_jan_2025

                

               // if (sysProtocol.HasSensor) sp.WriteCmd("MO");          // === Add by Yupeng Jan.18.2011
            }
            btnNext_Click(null, null);
        }

        #endregion


        #region Preliminary Settings

        private void uiPrelimSettings_Settings_Done(object sender, RoutedEventArgs e)
        {
            // Display next button
            if (btnNext.Visibility == Visibility.Hidden) btnNext.Visibility = Visibility.Visible;
        }

        #endregion


        #region Stretching

        #endregion


        #region Games

        private void bdrROM_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            uiGames.SwitchROMMode();
        }

        #endregion


        #region Evaluation

        private void Restore_TorqueLimits()
        {
            if (sp.IsConnected)
            {
                sp.WriteCmd($"DT{intelliProtocol.Stretching.FlexionTorque}"); // Stretching dorsi/flexion torque limit
                sp.WriteCmd($"PT{intelliProtocol.Stretching.ExtensionTorque}"); // Stretching plantar/extension torque limit
            }
        }

        #endregion


        #region Window Events

        private void wndMainApp_Loaded(object sender, RoutedEventArgs e)
        {
            // Initializations
            Initialize_Settings();
            Initialize_Variables();
            Initialize_Sounds();
        }

        private void wndMainApp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.D)
            {
                // Debug mode
                btnNext.Visibility = (btnNext.Visibility == Visibility.Hidden) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        #endregion


        #region Navigation

        private void btnNavigator_Click(object sender, RoutedEventArgs e)
        {
            // Slide in main menu
            uiMainMenu.SlideIn();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (taskList[taskIndex] == TaskMode.Evaluation) Restore_TorqueLimits();
            taskIndex++;
            Select_Task(taskList[taskIndex]);  // Navigate to next page
        }

        private void Select_Task(TaskMode task)
        {
            ReInitialize_Port();

            switch (task)
            {
                case TaskMode.Preparations:
                    return;

                case TaskMode.Connection:
                    Initialize_Connection();
                    tabContent.SelectedItem = tabConnection;
                    break;

                case TaskMode.PrelimSettings:
                    Initialize_PrelimSettings();
                    tabContent.SelectedItem = tabPrelimSettings;
                    break;

                case TaskMode.Stretching:
                    Initialize_Stretching();
                    tabContent.SelectedItem = tabStretching;
                    break;

                case TaskMode.Game:
                    Initialize_Games();
                    tabContent.SelectedItem = tabGames;
                    break;

                case TaskMode.Evaluation:
                    Initialize_Evaluation();
                    tabContent.SelectedItem = tabEvaluation;
                    break;

                case TaskMode.Report:
                    Initialize_Report();
                    tabContent.SelectedItem = tabReport;
                    break;
            }

            taskIndex = taskList.IndexOf(task);
            if (taskIndex == taskList.Count - 1) btnNext.Visibility = Visibility.Hidden;
        }
      
        #endregion


        #region Main Menu

        private void uiMainMenu_Button_Click(object sender, RoutedEventArgs e)
        {
            // Slide out main menu            
            uiMainMenu.SlideOut(); 
            // restore torque limits
            if (taskList[taskIndex] == TaskMode.Evaluation) Restore_TorqueLimits();

            string menuBtnName = e.OriginalSource.ToString();  // Menu button name as arg
            switch (menuBtnName)
            {
                case "btnConnection":
                    Select_Task(TaskMode.Connection);
                    break;

                case "btnPrelimSettings":
                    Select_Task(TaskMode.PrelimSettings);
                    break;

                case "btnUsers":
                    uiProfileLoader.Reset_UI();
                    break;

                case "btnGameLib":
                    uiGameLib.SlideIn();
                    break;
                    
                case "btnProtocol":
                    uiProtocol.Reset_Layout(UI.UIProtocol.LayoutMode.ShowAll);
                    uiProtocol.Load_Protocol(this, sp);
                    uiProtocol.SlideIn(false);
                    break;

                case "btnStretching":
                    Select_Task(TaskMode.Stretching);
                    break;

                case "btnGames":
                    Select_Task(TaskMode.Game);
                    break;

                case "btnEvaluation":
                    Select_Task(TaskMode.Evaluation);
                    break;

                case "btnReport":
                    Select_Task(TaskMode.Report);
                    break;

                case "btnExit":
                    if (MessageBox.Show("Do you want to quit IntelliStretch?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==  MessageBoxResult.Yes)
                    {
                        //if (sp.IsOpen()) sp.Close();
                        System.Windows.Application.Current.Shutdown();
                    }
                        
                    break;
            }

        }      

        #endregion


        public void Buttons_Enabled(bool IsEnabled)
        {
            btnNavigator.IsEnabled = IsEnabled;
            btnNext.IsEnabled = IsEnabled;
        }  
    }
}
