using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace IntelliStretch.UI
{
    /// <summary>
    /// Interaction logic for UIMainMenu.xaml
    /// </summary>
    public partial class UIMainMenu : UserControl
    {
        public UIMainMenu()
        {
            InitializeComponent();
        }

        #region Variables
        
        Storyboard sbMenuSlideIn, sbMenuSlideOut;

        #endregion

        #region Dependency Properties

        public ImageSource JointImage
        {
            get { return (ImageSource)GetValue(JointImageProperty); }
            set { SetValue(JointImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for JointImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty JointImageProperty =
            DependencyProperty.Register("JointImage", typeof(ImageSource), typeof(UIMainMenu), new UIPropertyMetadata(null));



        #endregion

        #region Routed Events
        public event RoutedEventHandler ButtonClick
        {
            add { AddHandler(ButtonClickEvent, value); }
            remove { RemoveHandler(ButtonClickEvent, value); }
        }

        public static readonly RoutedEvent ButtonClickEvent =
            EventManager.RegisterRoutedEvent("ButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIMainMenu));


        #endregion

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ButtonClickEvent);
            newEventArgs.Source = (e.Source as UserControls.ImageButton).Name;
            RaiseEvent(newEventArgs);  // Trigger button click event, expose menu button name as an arg
        }

        private void uiMainMenu_Loaded(object sender, RoutedEventArgs e)
        {
            // Animations
            sbMenuSlideIn = TryFindResource("sbMenuSlideIn") as Storyboard;  // Load slidein animation
            sbMenuSlideOut = TryFindResource("sbMenuSlideOut") as Storyboard;  // Load slideout animation
        }

        public void SlideIn()
        {
            this.Visibility = Visibility.Visible;
            if (Globals.Sound.pageSound != null) Globals.Sound.pageSound.Play();
            if (sbMenuSlideIn != null) sbMenuSlideIn.Begin();
        }

        public void SlideOut()
        {
            if (Globals.Sound.pageSound != null) Globals.Sound.pageSound.Play();
            if (sbMenuSlideOut != null) sbMenuSlideOut.Begin();
        }

        private void sbMenuSlideOut_Completed(object sender, System.EventArgs e)
        {
            // Hide main menu when animation completed
            this.Visibility = Visibility.Collapsed;
        }


    }
}
