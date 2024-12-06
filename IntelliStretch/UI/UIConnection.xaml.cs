using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IntelliStretch.UI
{
    public partial class UIConnection : UserControl
    {
        public UIConnection()
        {
            InitializeComponent();
        }

        #region Dependency Properties

        public ImageSource DeviceImage
        {
            get { return (ImageSource)GetValue(DeviceImageProperty); }
            set { SetValue(DeviceImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeviceImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeviceImageProperty =
            DependencyProperty.Register("DeviceImage", typeof(ImageSource), typeof(UIConnection), new UIPropertyMetadata(null));

        #endregion

        #region Routed Events
        public event RoutedEventHandler Connction_Click
        {
            add { AddHandler(Connction_ClickEvent, value); }
            remove { RemoveHandler(Connction_ClickEvent, value); }
        }

        public static readonly RoutedEvent Connction_ClickEvent =
            EventManager.RegisterRoutedEvent("Connction_Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIConnection));


        public event RoutedEventHandler Start_Click
        {
            add { AddHandler(Start_ClickEvent, value); }
            remove { RemoveHandler(Start_ClickEvent, value); }
        }

        public static readonly RoutedEvent Start_ClickEvent =
            EventManager.RegisterRoutedEvent("Start_Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIConnection));
        #endregion


        public void ConnectionUI_Update(bool isDone, bool isReady)
        {
            // Update UI after being connected
            this.Dispatcher.Invoke(new Action(delegate
            {
                waitAnimation.Visibility = isDone ? Visibility.Collapsed : Visibility.Visible;
                btnConnect.Visibility = (isDone & !isReady) ? Visibility.Visible : Visibility.Collapsed;
                btnStart.Visibility = (isDone & isReady) ? Visibility.Visible : Visibility.Collapsed;
            }));
        }

        public void txtStatus_Update(string msg)
        {
            // Update connection status
            txtStatus.Dispatcher.Invoke(new Action(delegate { txtStatus.Text = msg; }));
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(Connction_ClickEvent));
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(Start_ClickEvent));
        }
    }
}
