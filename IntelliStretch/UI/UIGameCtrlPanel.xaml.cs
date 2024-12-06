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
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace IntelliStretch.UI
{
    /// <summary>
    /// Interaction logic for UIGameCtrlPanel.xaml
    /// </summary>
    public partial class UIGameCtrlPanel : UserControl
    {
        public UIGameCtrlPanel()
        {
            InitializeComponent();
        }

        Storyboard sbShowPanel, sbHidePanel;

        public delegate void DelegateGameEnded();
        public DelegateGameEnded Game_Ended;
        public delegate void DelegateGameSettings();
        public DelegateGameSettings Game_Settings;



        public float JointPosition
        {
            get { return (float)GetValue(JointPositionProperty); }
            set { SetValue(JointPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for JointPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty JointPositionProperty =
            DependencyProperty.Register("JointPosition", typeof(float), typeof(UIGameCtrlPanel), new UIPropertyMetadata(0f));


        public float TargetPosition
        {
            get { return (float)GetValue(TargetPositionProperty); }
            set { SetValue(TargetPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetPositionProperty =
            DependencyProperty.Register("TargetPosition", typeof(float), typeof(UIGameCtrlPanel), new UIPropertyMetadata(0f));

       
        public float TorqueValue
        {
            get { return (float)GetValue(TorqueValueProperty); }
            set { SetValue(TorqueValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TorqueValueProperty =
            DependencyProperty.Register("TorqueValue", typeof(float), typeof(UIGameCtrlPanel), new UIPropertyMetadata(0f));


        public int AssistLevel
        {
            get { return (int)GetValue(AssistLevelProperty); }
            set { SetValue(AssistLevelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AssistLevel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AssistLevelProperty =
            DependencyProperty.Register("AssistLevel", typeof(int), typeof(UIGameCtrlPanel), new UIPropertyMetadata(0));




        private void Initialize_Animations()
        {
            sbShowPanel = TryFindResource("sbShowPanel") as Storyboard;
            sbHidePanel = TryFindResource("sbHidePanel") as Storyboard;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize_Animations();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            // Open game settings
            if (Game_Settings != null) Game_Settings();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            // Exit game
            stopwatchGame.Stop();
            if (Game_Ended != null) Game_Ended();
            this.Visibility = Visibility.Collapsed;
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Button).Width = 64;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Button).Width = 32;
        }
    }
}
