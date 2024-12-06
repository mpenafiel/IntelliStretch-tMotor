using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IntelliStretch.UI
{
    /// <summary>
    /// Interaction logic for UIPrelimSettings.xaml
    /// </summary>
    public partial class UIPrelimSettings : UserControl
    {
        public UIPrelimSettings()
        {
            InitializeComponent();
        }

        #region Variables

        IntelliSerialPort sp;
        MainApp mainApp;
        Protocols.GeneralSettings generalSettings;
        Image currentJoinImage;
        UserControls.FlatTextButton currentTxtButton;
        bool IsMeasuring;
        
        #endregion

        #region Dependency Properties

        public ImageSource NeutralImage
        {
            get { return (ImageSource)GetValue(NeutralImageProperty); }
            set { SetValue(NeutralImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NeutralImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NeutralImageProperty =
            DependencyProperty.Register("NeutralImage", typeof(ImageSource), typeof(UIPrelimSettings), new UIPropertyMetadata(null));


        public ImageSource FlexionImage
        {
            get { return (ImageSource)GetValue(FlexionImageProperty); }
            set { SetValue(FlexionImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlexionImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlexionImageProperty =
            DependencyProperty.Register("FlexionImage", typeof(ImageSource), typeof(UIPrelimSettings), new UIPropertyMetadata(null));


        public ImageSource ExtensionImage
        {
            get { return (ImageSource)GetValue(ExtensionImageProperty); }
            set { SetValue(ExtensionImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtensionImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtensionImageProperty =
            DependencyProperty.Register("ExtensionImage", typeof(ImageSource), typeof(UIPrelimSettings), new UIPropertyMetadata(null));


        public ImageSource GuideLinesImage
        {
            get { return (ImageSource)GetValue(GuideLinesImageProperty); }
            set { SetValue(GuideLinesImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GuideLinesImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GuideLinesImageProperty =
            DependencyProperty.Register("GuideLinesImage", typeof(ImageSource), typeof(UIPrelimSettings), new UIPropertyMetadata(null));



        public bool IsReflected
        {
            get { return (bool)GetValue(IsReflectedProperty); }
            set { SetValue(IsReflectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReflected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReflectedProperty =
            DependencyProperty.Register("IsReflected", typeof(bool), typeof(UIPrelimSettings), new UIPropertyMetadata(false));




        public string FlexionName
        {
            get { return (string)GetValue(FlexionNameProperty); }
            set { SetValue(FlexionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlexionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlexionNameProperty =
            DependencyProperty.Register("FlexionName", typeof(string), typeof(UIPrelimSettings), new UIPropertyMetadata("Flexion"));



        public string ExtensionName
        {
            get { return (string)GetValue(ExtensionNameProperty); }
            set { SetValue(ExtensionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtensionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtensionNameProperty =
            DependencyProperty.Register("ExtensionName", typeof(string), typeof(UIPrelimSettings), new UIPropertyMetadata("Extension"));



        #endregion

        #region Routed Events
        public event RoutedEventHandler Settings_Done
        {
            add { AddHandler(Settings_DoneEvent, value); }
            remove { RemoveHandler(Settings_DoneEvent, value); }
        }

        public static readonly RoutedEvent Settings_DoneEvent =
            EventManager.RegisterRoutedEvent("Settings_Done", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIPrelimSettings));


        #endregion


        public void Load_GeneralSettings(MainApp app, IntelliSerialPort port)
        {
            sp = port;
            mainApp = app;
            generalSettings = mainApp.IntelliProtocol.General;
            setFlexion.TextBoxText = generalSettings.FlexionMax.ToString();

            if (generalSettings.ExtensionMax > 0) setExtension.TextBoxText = generalSettings.ExtensionMax.ToString();
            else setExtension.TextBoxText = (-generalSettings.ExtensionMax).ToString();
            setExRange.Text = generalSettings.ExtraRange.ToString();
            this.IsReflected = (generalSettings.JointSide == Protocols.JointSide.Left); // Reflect display images if left arm side is selected, right arm and ankle remain unchanged
        }

        public void Update_DataInfo(IntelliSerialPort.AnkleData newAnkleData)
        {
            txtDataInfo.Dispatcher.Invoke(new Action(delegate
            {
                txtDataInfo.Text = "Position (deg): " + newAnkleData.anklePos.ToString("#0.0") + "\u00B0"
                                + "\r\nTorque (Nm): " + newAnkleData.ankleTorque.ToString("#0.0")
                                + "\r\nCurrent Level: " + (newAnkleData.ankleAm * 100).ToString() + "%";
            }));

             if (IsMeasuring && currentTxtButton != null)
            {
                currentTxtButton.Dispatcher.Invoke(new Action(delegate
                {
                    int newPos = (int)newAnkleData.anklePos;
                    if (newPos < 0) newPos = -newPos;
                    currentTxtButton.TextBoxText = newPos.ToString();
                }));
                IsMeasuring = false;
            }
        }


        private void btnSetNeutral_Click(object sender, RoutedEventArgs e)
        {
            // Set neutral postion
            if (sp.IsConnected) sp.WriteCmd("ZP");
            //setNeutral.Content = "0.00";

            //stackGeneralSettings.IsEnabled = true;  // Enable general settings
        }

        private void btnApplyGeneralSettings_Click(object sender, RoutedEventArgs e)
        {
            //Update general settings
            int newFlexion = Convert.ToInt32(setFlexion.TextBoxText);
            int newExtension = Convert.ToInt32(setExtension.TextBoxText);
            int newExRange = Convert.ToInt32(setExRange.Text);

            // Check sign of Extension and flip to negative if positive
            if (newExtension > 0) newExtension = -newExtension;

            generalSettings.FlexionMax = newFlexion;
            generalSettings.ExtensionMax = newExtension; // Flipped sign, added by Michael, 27.08.2024
            generalSettings.ExtraRange = newExRange;
            generalSettings.ActiveFlexionMax = newFlexion;
            generalSettings.ActiveExtensionMax = newExtension; // Flipped sign, added by Michael, 27.08.2024
            generalSettings.GameFlexionMax = newFlexion;
            generalSettings.GameExtensionMax = newExtension; // Flipped sign, added by Michael, 27.08.2024
            mainApp.IntelliProtocol.General = generalSettings;  // Apply changes

            if (sp.IsConnected)
            {
                sp.WriteCmd($"DF{newFlexion}"); // Set max Dorsi            
                sp.WriteCmd($"PF{newExtension}"); // Set max plantar            
                sp.WriteCmd($"EX{newExRange}"); // Set extra range
            }

            // Raise Done event
            RaiseEvent(new RoutedEventArgs(Settings_DoneEvent)); 
        }

        
        private void setting_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.Source == setFlexion)
            {
                Update_JointImage(imgFlexion);
                currentTxtButton = setFlexion;
                Update_SliderPicker(0, 40, 5, Convert.ToInt32(currentTxtButton.TextBoxText));// Chang Max. ROM from 90degree to 40 degree April.2013       
            }
            else if (e.Source == setExtension)
            {
                Update_JointImage(imgExtension);
                currentTxtButton = setExtension;
                Update_SliderPicker(0, 30, 5, Convert.ToInt32(currentTxtButton.TextBoxText));// Chang Max. ROM from 60degree to 30 degree April.2013          
            }
            else
            {
                Update_JointImage(imgNeutral);
                currentTxtButton = null;
            }
        }

        private void Update_JointImage(Image newImage)
        {
            if (currentJoinImage != null && currentJoinImage != newImage)
            {
                currentJoinImage.Visibility = Visibility.Collapsed;
                newImage.Visibility = Visibility.Visible;
            }
            currentJoinImage = newImage;
        }

        private void Update_SliderPicker(int minValue, int maxValue, int largeChange, int defaultValue)
        {
            sliderValuePicker.Minimum = minValue;
            sliderValuePicker.Maximum = maxValue;
            sliderValuePicker.LargeChange = largeChange;
            sliderValuePicker.Value = defaultValue;
            sliderValuePicker.BeginAnimation(OpacityProperty, new System.Windows.Media.Animation.DoubleAnimation(0d, 1d, new Duration(new TimeSpan(0,0,1))));
        }

        private void sliderValuePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (currentTxtButton != null)
                currentTxtButton.Dispatcher.Invoke(new Action(delegate
                {
                    currentTxtButton.TextBoxText = sliderValuePicker.Value.ToString();
                }));

        }

        private void Measure_ButtonClick(object sender, RoutedEventArgs e)
        {
            currentTxtButton = e.Source as UserControls.FlatTextButton;
            IsMeasuring = true;
        }

        private void uiPrelimSettings_Loaded(object sender, RoutedEventArgs e)
        {
            currentJoinImage = imgNeutral;
        }

        private void exRangeBdr_GotFocus(object sender, RoutedEventArgs e)
        {
            sliderValuePicker.Visibility = Visibility.Hidden;
            exRangeBdr.BorderBrush = new SolidColorBrush(Colors.Red);
            Update_JointImage(imgNeutral);

        }

        private void exRangeBdr_LostFocus(object sender, RoutedEventArgs e)
        {
            sliderValuePicker.Visibility = Visibility.Visible;
            exRangeBdr.BorderBrush = null;
        }

        private void exRangeScroller_ButtonMinClick(object sender, RoutedEventArgs e)
        {
            int range_val = Int32.Parse(setExRange.Text);

            if (range_val > 0) range_val = range_val - 1;
            setExRange.Text = range_val.ToString();
        }

        private void exRangeScroller_ButtonMaxClick(object sender, RoutedEventArgs e)
        {
            int range_val = Int32.Parse(setExRange.Text);

            if (range_val < 5) range_val++;
            setExRange.Text = range_val.ToString();

        }
    }
}
