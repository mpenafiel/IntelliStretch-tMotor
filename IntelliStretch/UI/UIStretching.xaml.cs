using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IntelliStretch.UI
{
    public partial class UIStretching : UserControl
    {
        public UIStretching()
        {
            InitializeComponent();
        }

        #region Variables

        Image imgJoint;
        TextBlock txtJointInfo;
        IntelliSerialPort sp;
        MainApp mainApp;
        Protocols.Joint selectedJoint;
        RotateTransform jointRotate;
        int jointTabIndex;
        Protocols.StretchingProtocol defaultStretch, customStretch;
        Protocols.IntelliProtocol intelliProtocol;

        #endregion

        #region Dependency properties

        public int FlexionTorqueMax
        {
            get { return (int)GetValue(FlexionTorqueMaxProperty); }
            set { SetValue(FlexionTorqueMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlexionTorqueMax.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlexionTorqueMaxProperty =
            DependencyProperty.Register("FlexionTorqueMax", typeof(int), typeof(UIStretching), new UIPropertyMetadata(30));


        public int ExtensionTorqueMax
        {
            get { return (int)GetValue(ExtensionTorqueMaxProperty); }
            set { SetValue(ExtensionTorqueMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtensionTorqueMax.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtensionTorqueMaxProperty =
            DependencyProperty.Register("ExtensionTorqueMax", typeof(int), typeof(UIStretching), new UIPropertyMetadata(30));


        public string FlexionName
        {
            get { return (string)GetValue(FlexionNameProperty); }
            set { SetValue(FlexionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlexionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlexionNameProperty =
            DependencyProperty.Register("FlexionName", typeof(string), typeof(UIStretching), new UIPropertyMetadata("Flexion"));



        public string ExtensionName
        {
            get { return (string)GetValue(ExtensionNameProperty); }
            set { SetValue(ExtensionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtensionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtensionNameProperty =
            DependencyProperty.Register("ExtensionName", typeof(string), typeof(UIStretching), new UIPropertyMetadata("Extension"));



        public Visibility ModeSelectionVisibility
        {
            get { return (Visibility)GetValue(ModeSelectionVisibilityProperty); }
            set { SetValue(ModeSelectionVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ModeSelectionVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeSelectionVisibilityProperty =
            DependencyProperty.Register("ModeSelectionVisibility", typeof(Visibility), typeof(UIStretching), new UIPropertyMetadata(Visibility.Collapsed));



        public bool IsReflected
        {
            get { return (bool)GetValue(IsReflectedProperty); }
            set { SetValue(IsReflectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReflected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReflectedProperty =
            DependencyProperty.Register("IsReflected", typeof(bool), typeof(UIStretching), new UIPropertyMetadata(false));



        #endregion


        public void Load_StretchProtocol(MainApp app, IntelliSerialPort port)
        {
            // Get references 
            mainApp = app;
            sp = port;  
            sp.UpdateData += new IntelliSerialPort.DelegateUpdateData(Update_UI); // Add event handler

            // Get protocols
            //Protocols.IntelliProtocol intelliProtocol = mainApp.IntelliProtocol; --------yp July262011
            intelliProtocol = mainApp.IntelliProtocol;
            selectedJoint = intelliProtocol.General.Joint;
            jointTabIndex = GetJointTabIndex(selectedJoint);
            //if (defaultStretch == null) defaultStretch = new Protocols.StretchingProtocol(intelliProtocol.Stretching); ----------Yp&Liang July26-2011
            if (defaultStretch == null) defaultStretch = intelliProtocol.Stretching;
            customStretch = intelliProtocol.Stretching;
            Load_StretchSettings(customStretch);

            // Initialize all tabs
            tabDemoAnkle.Visibility = Visibility.Collapsed;
            tabDemoKnee.Visibility = Visibility.Collapsed;
            tabDemoElbow.Visibility = Visibility.Collapsed;
            tabDemoWrist.Visibility = Visibility.Collapsed;

            switch (selectedJoint)
            {
                case Protocols.Joint.Ankle:  // Choose joint as ankle
                    imgJoint = imgFoot;  
                    imgJoint.RenderTransformOrigin = new Point(0.75, 0.6);  // Define rotation center
                    txtJointInfo = txtAnkleDataInfo;
                    tabDemoAnkle.Visibility = Visibility.Visible;
                    break;
                case Protocols.Joint.Elbow:  // Choose joint as elbow
                    imgJoint = imgForearm;
                    imgJoint.RenderTransformOrigin = new Point(0.5835, 0.5746);  // Define rotation center
                    txtJointInfo = txtElbowDataInfo;
                    tabDemoElbow.Visibility = Visibility.Visible;                    
                    break;
                case Protocols.Joint.Knee:  // Choose joint as knee
                    imgJoint = imgKnee;
                    imgJoint.RenderTransformOrigin = new Point(0.6, 0.25);
                    txtJointInfo = txtKneeDataInfo;
                    tabDemoKnee.Visibility = Visibility.Visible;                    
                    break;
                case Protocols.Joint.Wrist:  // Choose joint as wrist
                    imgJoint = imgWrist;
                    imgJoint.RenderTransformOrigin = new Point(0.5, 0.73);
                    txtJointInfo = txtWristDataInfo;
                    tabDemoWrist.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }

            tabConfig.IsSelected = true;
            this.IsReflected = (mainApp.IntelliProtocol.General.JointSide == Protocols.JointSide.Left); // Reflect display images if left arm side is selected, right arm and ankle remain unchanged
        }

        private void Update_UI(IntelliSerialPort.AnkleData newAnkleData)
        {
            this.Dispatcher.Invoke(new Action(delegate
            {
                jointRotate.Angle = (this.IsReflected) ? -newAnkleData.anklePos : newAnkleData.anklePos;
                imgJoint.RenderTransform = jointRotate;

                txtJointInfo.Dispatcher.Invoke(new Action(delegate
                {
                    txtJointInfo.Text = "Position (deg): " + newAnkleData.anklePos.ToString("#0.0")
                                    + "\r\nTorque (Nm): " + newAnkleData.ankleTorque.ToString("#0.0")
                                    + "\r\nCurrent Level: " + (newAnkleData.ankleAm * 100).ToString() + "%";
                }));
            }));
        }

        private void uiStretching_Loaded(object sender, RoutedEventArgs e)
        {
            jointRotate = new RotateTransform();
            
        }

        private void tabStretching_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void btnStretchingCtrl_Click(object sender, RoutedEventArgs e)
        {
            btnStretchingCtrl.IsPressed = !btnStretchingCtrl.IsPressed;  // Check or uncheck the button

            if (btnStretchingCtrl.IsPressed)
            {
                btnStretchingCtrl.Image = Utilities.GetImage("Stop-new.png");
                btnStretchingCtrl.Text = "Stop ";

                if (sp.IsConnected)
                {
                    sp.WriteCmd("ML10");  // Reset Stretching level = 10, Max level
                    sp.WriteCmd("IS");  // Start IntelliStretch

                    if (!btnStretchMode.IsChecked)
                    {
                        // Intelligent mode
                        if (mainApp.IntelliProtocol.System.IsSavingData) sp.Start_SaveData("Stretching_Intelligent");
                    }   
                    else
                    {
                        sp.WriteCmd("HT0");  // No holding time
                        sp.WriteCmd("IF");  // Basic stretching
                        if (mainApp.IntelliProtocol.System.IsSavingData) sp.Start_SaveData("Stretching_Basic");
                    }

                    sp.IsUpdating = true;
                }
                mainApp.Buttons_Enabled(false);
                stopWatchStretch.Start();
            }
            else
            {
                if (sp.IsConnected)                                         // Add by Yue Feb.12.2011
                {
                    sp.IsUpdating = false;
                    sp.WriteCmd("BK");  // Stop stretching, release motor //Changed RL to BK, Added by Michael Aug.19.2024
                    if (mainApp.IntelliProtocol.System.IsSavingData) sp.Stop_SaveData();
                }
                stopWatchStretch.Stop();
                btnStretchingCtrl.Image = Utilities.GetImage("Play-new.png");
                btnStretchingCtrl.Text = "Start  ";
                mainApp.Buttons_Enabled(true);
            }
        }

        private void btnAcceptStretchProtocol_Click(object sender, RoutedEventArgs e)
        {
           // Accept settings
            Accept_StretchSettings();

            // Hide config grid
            tabStretching.SelectedIndex = jointTabIndex;
        }

        private void btnCancelStretchProtocol_Click(object sender, RoutedEventArgs e)
        {
            // Hide config grid
            tabStretching.SelectedIndex = jointTabIndex;
        }

        private int GetJointTabIndex(Protocols.Joint joint)
        {
            int index = 1;
            int intJoint = (int)joint;

            while ((intJoint >>= 1) != 0) index++;

            return index;
        }

        private void btnSaveDefaultStretch_Click(object sender, RoutedEventArgs e)
        {
            Get_CurrentSettings(defaultStretch);

            mainApp.Save_Protocol();
        }

        private void btnDefaultStretch_Click(object sender, RoutedEventArgs e)
        {
            Load_StretchSettings(defaultStretch);
            Apply_StretchSettings(defaultStretch);
        }

        private void btnCustomStretch_Click(object sender, RoutedEventArgs e)
        {
            // Hide config grid
            tabStretching.SelectedIndex = 0;
        }

        private void Accept_StretchSettings()
        {
            // Accept current settings
            Get_CurrentSettings(customStretch);
            Apply_StretchSettings(customStretch);
        }

        private void Get_CurrentSettings(Protocols.StretchingProtocol protocol)
        {
            protocol.Duration = (int)sliderStretchDuration.Value;
            protocol.HoldingTime = (int)sliderStretchHoldTime.Value;
            protocol.FlexionVelocity = (int)sliderStretchFlexV.Value;
            protocol.ExtensionVelocity = (int)sliderStretchExtV.Value;
            protocol.FlexionTorque = (int)sliderStretchFlexTq.Value;
            protocol.ExtensionTorque = (int)sliderStretchExtTq.Value;
        }

        private void Apply_StretchSettings(Protocols.StretchingProtocol protocol)
        {
            // Apply settings to DSP
            if (sp.IsConnected)
            {
                sp.WriteCmd("ML10");  // Stretching level = 10
                if (btnStretchMode.IsChecked)
                    sp.WriteCmd("HT0");
                else
                    sp.WriteCmd($"HT{protocol.HoldingTime}");  // Stretching holding time


                sp.WriteCmd($"DV{protocol.ExtensionVelocity}"); // Stretching dorsi/flexion velocity 
                sp.WriteCmd($"PV{protocol.FlexionVelocity}");  // Stretching plantar/extension velocity limit
                sp.WriteCmd($"DT{protocol.FlexionTorque}"); // Stretching dorsi/flexion torque limit
                sp.WriteCmd($"PT{protocol.ExtensionTorque}"); // Stretching plantar/extension torque limit

            }
        }

        private void Load_StretchSettings(Protocols.StretchingProtocol protocol)
        {
            // Load setting values to sliders
            sliderStretchDuration.Value = protocol.Duration;
            sliderStretchHoldTime.Value = protocol.HoldingTime;
            sliderStretchFlexV.Value = protocol.FlexionVelocity;
            sliderStretchExtV.Value = protocol.ExtensionVelocity;
            sliderStretchFlexTq.Value = protocol.FlexionTorque;
            sliderStretchExtTq.Value = protocol.ExtensionTorque;
            this.FlexionTorqueMax = protocol.FlexionTorqueMax;
            this.ExtensionTorqueMax = protocol.ExtensionTorqueMax;
        }

        private void stopWatchStretch_Tick(object sender, RoutedEventArgs e)
        {
            if (stopWatchStretch.Min >= customStretch.Duration) // if stretching time expires
            {
                btnStretchingCtrl_Click(null, null);
            }
        }

        private void btnStretchMode_Click(object sender, RoutedEventArgs e)
        {
            if (btnStretchMode.IsChecked)
            {
                btnStretchMode.Text = "Basic";
                if (sp.IsConnected)
                {
                    // If stretching is going on, switch to basic stretching
                    sp.WriteCmd("HT0");  // No holding time
                    sp.WriteCmd("IF");  // Basic stretching
                }
            }
            else
            {
                btnStretchMode.Text = "Intelligent";
                if (sp.IsConnected)
                {
                    // If stretching is going on, switch to intelligent stretching
                    sp.WriteCmd($"HT{customStretch.HoldingTime}");  // Reset holding time
                    sp.WriteCmd("IS");  //  Intellistretching
                }
            }
        }
    }
}
