using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IntelliStretch.Data;

namespace IntelliStretch.UserControls
{
    /// <summary>
    /// Interaction logic for HorizontalRangeFinder.xaml
    /// </summary>
    public partial class HorizontalRangeFinder : UserControl, Interfaces.IUpdateUI
    {
        public HorizontalRangeFinder()
        {
            InitializeComponent();
        }

        #region Properties
        public float ActiveFlexionMax { get; set; }
        public float ActiveExtensionMax { get; set; }
        #endregion

        #region Dependency Properties

        public string FlexionName
        {
            get { return (string)GetValue(FlexionNameProperty); }
            set { SetValue(FlexionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlexionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlexionNameProperty =
            DependencyProperty.Register("FlexionName", typeof(string), typeof(HorizontalRangeFinder), new UIPropertyMetadata("Flexion"));


        public string ExtensionName
        {
            get { return (string)GetValue(ExtensionNameProperty); }
            set { SetValue(ExtensionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtensionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtensionNameProperty =
            DependencyProperty.Register("ExtensionName", typeof(string), typeof(HorizontalRangeFinder), new UIPropertyMetadata("Extension"));



        public int PassiveFlexMax
        {
            get { return (int)GetValue(PassiveFlexMaxProperty); }
            set { SetValue(PassiveFlexMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PassiveFlexMax.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PassiveFlexMaxProperty =
            DependencyProperty.Register("PassiveFlexMax", typeof(int), typeof(HorizontalRangeFinder), new UIPropertyMetadata(20));



        public int PassiveExtMax
        {
            get { return (int)GetValue(PassiveExtMaxProperty); }
            set { SetValue(PassiveExtMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PassiveExtMax.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PassiveExtMaxProperty =
            DependencyProperty.Register("PassiveExtMax", typeof(int), typeof(HorizontalRangeFinder), new UIPropertyMetadata(30));



        #endregion

        double activeFlexPos;
        double activeExtPos;
        bool IsInitialPos;
        DataInfo.DataMode currentDataMode;


        public void Initialize_Layout(int flexMax, int extMax)
        {
            PassiveFlexMax = flexMax;
            PassiveExtMax = extMax;

        }

 
        private double Pos2Screen(float dataPos, int max, int min)
        {
            return (cvsRange.ActualWidth - 40) * (dataPos - min) / (max - min);
        }

        #region IUpdateUI Members

        public void Update_UI(IntelliSerialPort.AnkleData newAnkleData)
        {
            float currentPos;
            double newScrPos;

            if (currentDataMode == DataInfo.DataMode.Position)
            {
                currentPos = newAnkleData.anklePos;

                if (currentPos > PassiveFlexMax)
                    currentPos = PassiveFlexMax;
                else if (currentPos < PassiveExtMax)
                    currentPos = PassiveExtMax;
            
                newScrPos = Pos2Screen(currentPos, PassiveFlexMax, PassiveExtMax);

                ShowTextBlock.Text = "Position (deg): " + newAnkleData.anklePos.ToString("#0.0");
                                 
            }
            else
            {
                currentPos = -newAnkleData.ankleTorque;

                if (currentPos > PassiveFlexMax)
                    currentPos = PassiveFlexMax;
                else if (currentPos < PassiveExtMax)
                    currentPos = PassiveExtMax;

                newScrPos = Pos2Screen(currentPos, PassiveFlexMax, PassiveExtMax);

                ShowTextBlock.Text = "Torque (Nm): " + newAnkleData.ankleTorque.ToString("#0.0");

            }

            
            Canvas.SetLeft(pnlCurrentPos, newScrPos);
            txtCurrentPos.Text = currentPos.ToString("#0.0");

            if (this.IsInitialPos)
            {
                // Initialize search positions
                activeFlexPos = newScrPos;
                activeExtPos = newScrPos;
                rectAROM.Width = activeFlexPos - activeExtPos;
                Canvas.SetLeft(pnlAFlexMax, activeFlexPos);
                Canvas.SetLeft(pnlAExtMax, activeExtPos);
                Canvas.SetLeft(rectAROM, activeExtPos);
                this.ActiveFlexionMax = currentPos;
                this.ActiveExtensionMax = currentPos;
                this.IsInitialPos = false;
            }
            else
            {
                // Update Flex / Ext limit positions
                if (currentPos > this.ActiveFlexionMax)
                {
                    this.ActiveFlexionMax = currentPos;
                    Canvas.SetLeft(pnlAFlexMax, newScrPos);
                    activeFlexPos = newScrPos;
                    rectAROM.Width = activeFlexPos - activeExtPos;
                    txtActiveFlexionMax.Text = currentPos.ToString("#0.0");
                }
                else if (currentPos < this.ActiveExtensionMax)
                {
                    this.ActiveExtensionMax = currentPos;
                    Canvas.SetLeft(pnlAExtMax, newScrPos);
                    activeExtPos = newScrPos;
                    rectAROM.Width = activeFlexPos - activeExtPos;
                    Canvas.SetLeft(rectAROM, activeExtPos);
                    txtActiveExtensionMax.Text = currentPos.ToString("#0.0");
                }
            }
        }

        public void Set_Initial(bool IsInitial)
        {
            IsInitialPos = IsInitial;
        }

        public void Set_DataMode(IntelliStretch.Data.DataInfo.DataMode dataMode)
        {
            currentDataMode = dataMode;
            Reset_Layout();
        }
        #endregion

        private void rangeFinder_LayoutUpdated(object sender, EventArgs e)
        {
            // Neutral position line
            Reset_Layout();
            if (cvsRange.ActualWidth != 0.0) this.LayoutUpdated -= rangeFinder_LayoutUpdated;
        }

        private void Reset_Layout()
        {
            // Neutral position line
            double neutralPos;

            if (currentDataMode == DataInfo.DataMode.Position)
                neutralPos = Pos2Screen(0, PassiveFlexMax, PassiveExtMax);
            else
                neutralPos = Pos2Screen(0, PassiveFlexMax, PassiveExtMax);

            Canvas.SetTop(rectNeutral, neutralPos - 1);

            // Initialize arrows
            Canvas.SetLeft(pnlCurrentPos, neutralPos);
            Canvas.SetLeft(pnlAFlexMax, neutralPos);
            Canvas.SetLeft(pnlAExtMax, neutralPos);
            rectAROM.Width = 0;

        }

    }
}
