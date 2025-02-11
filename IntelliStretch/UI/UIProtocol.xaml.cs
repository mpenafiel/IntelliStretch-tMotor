using NationalInstruments.DAQmx;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace IntelliStretch.UI
{
    /// <summary>
    /// Interaction logic for UIProtocol.xaml
    /// </summary>
    public partial class UIProtocol : UserControl
    {
        public UIProtocol()
        {
            InitializeComponent();
            sbSlideIn = FindResource("sbSlideIn") as Storyboard;
            sbSlideOut = FindResource("sbSlideOut") as Storyboard;
        }

        #region Variables

        Storyboard sbSlideIn, sbSlideOut;
        Protocols.IntelliProtocol intelliProtocol;
        IntelliSerialPort sp;
        MainApp mainApp;
        bool IsResumeNeeded;

        public enum LayoutMode
        {
            ShowAll = 0,
            AssistiveMode = 1,
            ResistiveMode = 2,
            AssistAndResist = 3
        }
        public LayoutMode Mode { get; set; }

        #endregion

        #region Dependency properties


        public string FlexionName
        {
            get { return (string)GetValue(FlexionNameProperty); }
            set { SetValue(FlexionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlexionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlexionNameProperty =
            DependencyProperty.Register("FlexionName", typeof(string), typeof(UIProtocol), new UIPropertyMetadata("Flexion"));



        public string ExtensionName
        {
            get { return (string)GetValue(ExtensionNameProperty); }
            set { SetValue(ExtensionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtensionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtensionNameProperty =
            DependencyProperty.Register("ExtensionName", typeof(string), typeof(UIProtocol), new UIPropertyMetadata("Extension"));


        public int FlexionTorqueMax
        {
            get { return (int)GetValue(FlexionTorqueMaxProperty); }
            set { SetValue(FlexionTorqueMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlexionTorqueMax.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlexionTorqueMaxProperty =
            DependencyProperty.Register("FlexionTorqueMax", typeof(int), typeof(UIProtocol), new UIPropertyMetadata(30));



        public int ExtensionTorqueMax
        {
            get { return (int)GetValue(ExtensionTorqueMaxProperty); }
            set { SetValue(ExtensionTorqueMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtensionTorqueMax.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtensionTorqueMaxProperty =
            DependencyProperty.Register("ExtensionTorqueMax", typeof(int), typeof(UIProtocol), new UIPropertyMetadata(30));
        #endregion


        public void Load_Protocol(MainApp app, IntelliSerialPort port)
        {
            // Get references
            sp = port; 
            mainApp = app;

            // Load all settings
            intelliProtocol = mainApp.IntelliProtocol;
            if (tabGeneral.Visibility == Visibility.Visible)
            {
                Load_GeneralSettings(intelliProtocol.General);
                Load_SystemSettings(intelliProtocol.System);
            }
            if (tabStretch.Visibility == Visibility.Visible) Load_StretchSettings(intelliProtocol.Stretching);
            if (tabGames.Visibility == Visibility.Visible) Load_GameSettings(intelliProtocol.Game);
            if (tabDAQ.Visibility == Visibility.Visible) Load_DaqSettings(intelliProtocol.DAQ);

        }

        public void Reset_Layout(LayoutMode mode)
        {
            this.Mode = mode;

            switch (mode)
            {
                case LayoutMode.ShowAll:
                    tabGeneral.Visibility = Visibility.Visible;
                    tabStretch.Visibility = Visibility.Visible;
                    groupAssist.Visibility = Visibility.Visible;
                    groupResist.Visibility = Visibility.Visible;
                    Grid.SetRowSpan(groupAssist, 1);
                    Grid.SetRow(groupResist, 2);
                    Grid.SetRowSpan(groupResist, 1);
                    break;

                case LayoutMode.AssistiveMode:
                    tabGeneral.Visibility = Visibility.Collapsed;
                    tabStretch.Visibility = Visibility.Collapsed;
                    groupAssist.Visibility = Visibility.Visible;
                    groupResist.Visibility = Visibility.Collapsed;
                    tabDAQ.Visibility = Visibility.Collapsed;
                    Grid.SetRowSpan(groupAssist, 2);
                    tabCtrlConfig.SelectedIndex = 2;
                    break;

                case LayoutMode.ResistiveMode:
                    tabGeneral.Visibility = Visibility.Collapsed;
                    tabStretch.Visibility = Visibility.Collapsed;
                    groupAssist.Visibility = Visibility.Collapsed;
                    groupResist.Visibility = Visibility.Visible;
                    tabDAQ.Visibility = Visibility.Collapsed;
                    Grid.SetRow(groupResist, 0);
                    Grid.SetRowSpan(groupResist, 2);
                    tabCtrlConfig.SelectedIndex = 2;
                    break;

                case LayoutMode.AssistAndResist:
                    tabGeneral.Visibility = Visibility.Collapsed;
                    tabStretch.Visibility = Visibility.Collapsed;
                    groupAssist.Visibility = Visibility.Visible;
                    groupResist.Visibility = Visibility.Visible;
                    tabDAQ.Visibility = Visibility.Collapsed;
                    Grid.SetRowSpan(groupAssist, 1);
                    Grid.SetRow(groupResist, 2);
                    Grid.SetRowSpan(groupResist, 1);
                    break;
                    
            }
        }

        public void SlideIn(bool gameResume)
        {
            IsResumeNeeded = gameResume;
            if (Globals.Sound.pageSound != null) Globals.Sound.pageSound.Play();
            this.Visibility = Visibility.Visible;
            if (sbSlideIn != null) sbSlideIn.Begin();
        }

        private void SlideOut()
        {
            if (Globals.Sound.pageSound != null) Globals.Sound.pageSound.Play();
            if (sbSlideOut != null) sbSlideOut.Begin();
        }

        private void aniSlideOut_Completed(object sender, EventArgs e)
        {
            if (IsResumeNeeded) Resume_CurrentGame();
            // Hide config dialog when animation completed
            this.Visibility = Visibility.Hidden;
        }

        private void Load_SystemSettings(Protocols.SystemSettings protocol)
        {
            cboSampRate.Text = intelliProtocol.System.SamplingRate.ToString();
            tgBtnSave.IsChecked = intelliProtocol.System.IsSavingData;
            //ellipseSaveing.Fill = ((bool)tgBtnSave.IsChecked) ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red); // From the previous version, based on toggle button
        }

        private void Load_GeneralSettings(Protocols.GeneralSettings protocol)
        {
            sliderFlexMax.Value = protocol.FlexionMax;
            
            // Visually display a positive #
            if (protocol.ExtensionMax > 0) sliderExtMax.Value = protocol.ExtensionMax;
            else sliderExtMax.Value = -protocol.ExtensionMax;
            sliderExRange.Value = protocol.ExtraRange;
        }

        private void Load_StretchSettings(Protocols.StretchingProtocol protocol)
        {
            sliderStretchDuration.Value = protocol.Duration;
            sliderStretchHoldTime.Value = protocol.HoldingTime;
            sliderStretchFlexV.Value = protocol.FlexionVelocity;
            sliderStretchExtV.Value = protocol.ExtensionVelocity;
            sliderStretchFlexTq.Value = protocol.FlexionTorque;
            sliderStretchExtTq.Value = protocol.ExtensionTorque;
            this.FlexionTorqueMax = protocol.FlexionTorqueMax;
            this.ExtensionTorqueMax = protocol.ExtensionTorqueMax;
        }

        private void Load_GameSettings(Protocols.GameProtocol protocol)
        {
            // Assistive
            sliderAssistLevel.Value = protocol.AssistiveMode.Level;
            sliderAssistV.Value = protocol.AssistiveMode.Velocity;
            sliderAssistDelay.Value = protocol.AssistiveMode.DelayTime;

            // Resistive
            cboCtrlDirection.SelectedIndex = (protocol.ResistiveMode.ControlDirection == Protocols.Direction.Horizontal) ? 0 : 1;
            chkNoLoading.IsChecked = protocol.ResistiveMode.IsNoLoading;
            sliderResistScale.Value = protocol.ResistiveMode.ScalingFactor;
            //sliderResistFlex.Value = protocol.ResistiveMode.FlexionResistance;
            //sliderResistExt.Value = protocol.ResistiveMode.ExtensionResistance;
        }

        private void Load_DaqSettings(Protocols.DaqProtocol protocol)
        {
            string[] channels = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External);

            if (channels.Length == 0)
            {
                string message = "No device detected. Plug in device and refresh DAQ page.";
                MessageBox.Show(message, "DAQ Settings", MessageBoxButton.OK, MessageBoxImage.Information);

                physicalChannelComboBox1.Items.Clear(); physicalChannelComboBox2.Items.Clear();
            }
            else if (channels.Contains(protocol.Channel1) == false | channels.Contains(protocol.Channel2) == false)
            {
                string message = "Detected device is different from User Default. Would you like to update the settings to aquire from the new device?\n\n If you would like to use current device settings, press No. Otherwise, plug in the original device and refresh page";

                MessageBoxResult result;
                result = MessageBox.Show(message, "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    physicalChannelComboBox1.Items.Clear(); physicalChannelComboBox2.Items.Clear();

                    string[] newChannels = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External);

                    foreach (string channel in newChannels)
                    {
                        physicalChannelComboBox1.Items.Add(channel); physicalChannelComboBox2.Items.Add(channel);
                    }

                    if (physicalChannelComboBox1.Items.Count > 0)
                        physicalChannelComboBox1.SelectedIndex = 1; //Set physical channel to ai1

                    if (physicalChannelComboBox2.Items.Count > 1)
                        physicalChannelComboBox2.SelectedIndex = 3; //Set physical channel to ai3

                    tabCtrlConfig.SelectedIndex = 3;
                }
                else if (result == MessageBoxResult.No)
                {
                    physicalChannelComboBox1.Items.Clear();
                    physicalChannelComboBox2.Items.Clear();

                    physicalChannelComboBox1.Items.Add(protocol.Channel1);
                    physicalChannelComboBox2.Items.Add(protocol.Channel2);

                    physicalChannelComboBox1.SelectedItem = protocol.Channel1;
                    physicalChannelComboBox2.SelectedItem = protocol.Channel2;
                }
            }

            else
            {
                physicalChannelComboBox1.Items.Clear(); physicalChannelComboBox2.Items.Clear();

                foreach (string channel in channels)
                {
                    physicalChannelComboBox1.Items.Add(channel); physicalChannelComboBox2.Items.Add(channel);
                }

                if (physicalChannelComboBox1.Items.Count > 0) physicalChannelComboBox1.SelectedItem = protocol.Channel1;

                if (physicalChannelComboBox2.Items.Count > 1) physicalChannelComboBox2.SelectedItem = protocol.Channel2;
            }

            textboxName1.Text = protocol.Channel1_Name;
            textboxName2.Text = protocol.Channel2_Name;
            textboxModel.Text = protocol.Model;

        }

        private void Accept_General()
        {
            Get_GeneralSettings(intelliProtocol.General);
            Apply_GeneralSettings(intelliProtocol.General);
        }

        private void Accept_System()
        {
            Get_SystemSettings(intelliProtocol.System);
            Apply_SystemSettings(intelliProtocol.System);
        }

        private void Accept_Stretch()
        {
            Get_StretchSettings(intelliProtocol.Stretching);
            Apply_StretchSettings(intelliProtocol.Stretching);
        }

        private void Accept_Game()
        {
            if (groupAssist.Visibility == Visibility.Visible)
            {
                Get_AssistGameSettings(intelliProtocol.Game.AssistiveMode);
                Apply_AssistGameSettings(intelliProtocol.Game.AssistiveMode);
            }
            if (groupResist.Visibility == Visibility.Visible)
            {
                Get_ResistGameSettings(intelliProtocol.Game.ResistiveMode);
                Apply_ResistGameSettings(intelliProtocol.Game.ResistiveMode);
            }
        }

        private bool AcceptDaq()
        {
            return Get_DaqSettings(intelliProtocol.DAQ);
        }

        private void Apply_SystemSettings(Protocols.SystemSettings protocol)
        {
            if (sp.IsConnected)
            {
                sp.WriteCmd($"HZ{protocol.SamplingRate}"); // Set sampling rate 
            }

            if ((bool)tgBtnSave.IsChecked)
            {
                string newDataDir = mainApp.CurrentUserDir + mainApp.CurrentDateDir;
                sp.DataDir = newDataDir;
                sp.DataFilePrefix = mainApp.DataFilePrefix;
                if (!Directory.Exists(newDataDir)) Directory.CreateDirectory(newDataDir);  // Create data folder for today
            }
        }

        private void Get_SystemSettings(Protocols.SystemSettings protocol)
        {
            protocol.SamplingRate = Convert.ToInt32(cboSampRate.Text);
            protocol.IsSavingData = (bool)tgBtnSave.IsChecked;
        }

        private void Apply_GeneralSettings(Protocols.GeneralSettings protocol)
        {
            int newExtension;

            if (protocol.ExtensionMax > 0) newExtension = -protocol.ExtensionMax; // === Add By:Yupeng Jan.18.2011=== //Edited By: Michael 18.01.2024
            else newExtension = protocol.ExtensionMax;            

            if (sp.IsConnected)
            {
                sp.WriteCmd($"DF{protocol.FlexionMax}"); // Set max flexion            
                //sp.WriteCmd("PF" + protocol.ExtensionMax.ToString()); // Set max extension   // === Del By:Yupeng Jan.18.2011===         
                sp.WriteCmd($"PF{newExtension}"); // Set max extension              // === Add By:Yupeng Jan.18.2011===
                sp.WriteCmd($"EX{protocol.ExtraRange}"); // Set extra range
            }
        }

        private void Get_GeneralSettings(Protocols.GeneralSettings protocol)
        {
            protocol.FlexionMax = (int)sliderFlexMax.Value;
            // Check sign and convert to negative if necessary
            if ((int)sliderExtMax.Value > 0) protocol.ExtensionMax = -(int)sliderExtMax.Value;
            else protocol.ExtensionMax = (int)sliderExtMax.Value;
            protocol.ExtraRange = (int)sliderExRange.Value;
            
        }

        private void Apply_StretchSettings(Protocols.StretchingProtocol protocol)
        {
            // Apply settings to DSP
            if (sp.IsConnected)
            {
                sp.WriteCmd($"ML{protocol.Level}");  // Stretching level = 10
                sp.WriteCmd($"HT{protocol.HoldingTime}");  // Stretching holding time
                sp.WriteCmd($"DV{protocol.FlexionVelocity}"); // Stretching flexion velocity limit
                sp.WriteCmd($"PV{protocol.ExtensionVelocity}");  // Stretching extension velocity limit
                sp.WriteCmd($"DT{protocol.FlexionTorque}"); // Stretching flexion torque limit
                sp.WriteCmd($"PT{protocol.ExtensionTorque}"); // Stretching extension torque limit
            }
        }

        private void Get_StretchSettings(Protocols.StretchingProtocol protocol)
        {
            protocol.Duration = (int)sliderStretchDuration.Value;
            protocol.HoldingTime = (int)sliderStretchHoldTime.Value;
            protocol.FlexionVelocity = (int)sliderStretchFlexV.Value;
            protocol.ExtensionVelocity = (int)sliderStretchExtV.Value;
            protocol.FlexionTorque = (int)sliderStretchFlexTq.Value;
            protocol.ExtensionTorque = (int)sliderStretchExtTq.Value;
        }

        private void Apply_AssistGameSettings(Protocols.Assistive protocol)
        {
            if (sp.IsConnected)
            {
                sp.WriteCmd($"ML{protocol.Level}");
                sp.WriteCmd($"VL{protocol.Velocity}");
                sp.WriteCmd($"AD{protocol.DelayTime}");
            }
        }

        private void Get_AssistGameSettings(Protocols.Assistive protocol)
        {
            protocol.Level = (int)sliderAssistLevel.Value;
            protocol.Velocity = (int)sliderAssistV.Value;
            protocol.DelayTime = (int)sliderAssistDelay.Value;
        }

        private void Apply_ResistGameSettings(Protocols.Resistive protocol)
        {
            if (sp.IsConnected)
            {
                if ((bool)chkNoLoading.IsChecked)
                    sp.WriteCmd("BK"); // No loading => backdrivable
                else
                {
                    sp.WriteCmd("FC"); // Friction control
                    sp.WriteCmd($"FR{protocol.Resistance}"); // Set non-directional resistance
                    //sp.WriteCmd($"FP{protocol.FlexionResistance}"); // Set Dorsi/flexion Resistance            
                    //sp.WriteCmd($"FD{protocol.ExtensionResistance}"); // Set plantar/extension Resistance
                }
            }
        }

        private void Get_ResistGameSettings(Protocols.Resistive protocol)
        {
            protocol.ControlDirection = (cboCtrlDirection.SelectedIndex == 0) ? Protocols.Direction.Horizontal : Protocols.Direction.Vertical;
            protocol.IsNoLoading = (bool)chkNoLoading.IsChecked;
            protocol.ScalingFactor = (float)sliderResistScale.Value;
            protocol.FlexionResistance = (int)sliderResistance.Value;

            //protocol.FlexionResistance = (int)sliderResistFlex.Value;
            //protocol.ExtensionResistance = (int)sliderResistExt.Value;
        }

        private bool Get_DaqSettings(Protocols.DaqProtocol protocol)
        {
            int detectedChannels = physicalChannelComboBox1.Items.Count;
            if (detectedChannels > 0)
            {
                if ((string)physicalChannelComboBox1.Text != (string)physicalChannelComboBox2.Text)
                {
                    protocol.Channel1 = (string)physicalChannelComboBox1.Text;
                    protocol.Channel2 = (string)physicalChannelComboBox2.Text;
                    protocol.Channel1_Name = (string)textboxName1.Text;
                    protocol.Channel2_Name = (string)textboxName2.Text;
                    protocol.SamplingRate = Convert.ToInt32(comboBoxSampleRate.Text);
                    protocol.Model = (string)textboxModel.Text;
                    protocol.SampPerChan = Convert.ToInt32(comboBoxSampPerCh.Text);

                    int devIndex = physicalChannelComboBox1.Text.IndexOf("/");
                    string devName = physicalChannelComboBox1.Text.Substring(0, devIndex);
                    protocol.DigitalChannel = $"{devName}/port0/line0:7";

                    return true;
                }
                else
                {
                    string message = "Cannot accept current configuration. Please make sure two different channels are selected.";
                    MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

            }
            else
            {
                string message = "Cannot accept current configuration. Please make sure EMG device is plugged into the system.";
                MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

        }

        private void btnSaveDefault_Click(object sender, RoutedEventArgs e)
        {
            if (tabGeneral.Visibility == Visibility.Visible)
            {
                Get_GeneralSettings(intelliProtocol.General);
                Get_SystemSettings(intelliProtocol.System);
            }
            if (tabStretch.Visibility == Visibility.Visible) Get_StretchSettings(intelliProtocol.Stretching);
            if (groupAssist.Visibility == Visibility.Visible) Get_AssistGameSettings(intelliProtocol.Game.AssistiveMode);
            if (groupResist.Visibility == Visibility.Visible) Get_ResistGameSettings(intelliProtocol.Game.ResistiveMode);
            string message = "Are you sure you want to save new DAQ protocols? Only accept if you understand what you are doing.";
            if (tabDAQ.IsSelected)
            {
                if (MessageBox.Show(message, "Information", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    if (tabDAQ.Visibility == Visibility.Visible) Get_DaqSettings(intelliProtocol.DAQ);
                }
                else
                {
                    //exit without leaving Protocol page
                    return;
                }
            }

            mainApp.Save_Protocol();
        }

        private void btnAcceptProtocol_Click(object sender, RoutedEventArgs e)
        {
            
            if (tabGeneral.Visibility == Visibility.Visible)
            {
                Accept_General();
                Accept_System();
            }
            if (tabStretch.Visibility == Visibility.Visible) Accept_Stretch();
            if (tabGames.Visibility == Visibility.Visible) Accept_Game();
            string message = "Are you sure you want to accept new DAQ protocols? Only accept if you understand what you are doing.";
            if (tabDAQ.IsSelected)
            {
                if (MessageBox.Show(message, "Information", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    bool DaqAccepted;
                    DaqAccepted = AcceptDaq();
                    if (DaqAccepted == false) return;
                }
                else
                {
                    //exit without leaving Protocol page
                    return;
                }
            }
            SlideOut();
        }

        private void btnCancelProtocol_Click(object sender, RoutedEventArgs e)
        {
            SlideOut();
        }

        private void Resume_CurrentGame()
        {
            mainApp.uiGames.ResumeGame();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            physicalChannelComboBox1.Items.Clear(); physicalChannelComboBox2.Items.Clear();

            string[] newChannels = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External);

            foreach (string channel in newChannels)
            {
                physicalChannelComboBox1.Items.Add(channel); physicalChannelComboBox2.Items.Add(channel);
            }

            if (physicalChannelComboBox1.Items.Count > 0)
                physicalChannelComboBox1.SelectedIndex = 1; //Set physical channel to ai1

            if (physicalChannelComboBox2.Items.Count > 1)
                physicalChannelComboBox2.SelectedIndex = 3; //Set physical channel to ai3

            tabCtrlConfig.SelectedIndex = 3;
        }
    }
}
