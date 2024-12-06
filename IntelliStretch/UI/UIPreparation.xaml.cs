using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace IntelliStretch.UI
{
    /// <summary>
    /// Interaction logic for UIPreparation.xaml
    /// </summary>
    public partial class UIPreparation : UserControl
    {
        public UIPreparation()
        {
            InitializeComponent();
        }

        #region Variables

        Storyboard sbArrowIndicator, sbArrowNext, sbSlideWelcome, sbSlideInfo;
        MainApp mainApp;

        #endregion

        #region Dependency Properties


        public ImageSource DeviceImage
        {
            get { return (ImageSource)GetValue(DeviceImageProperty); }
            set { SetValue(DeviceImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeviceImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeviceImageProperty =
            DependencyProperty.Register("DeviceImage", typeof(ImageSource), typeof(UIPreparation), new UIPropertyMetadata(null));



        #endregion

        public void Load_Preparation(MainApp app)
        {
            mainApp = app;
        }

        public void Begin_Intro_Animation(string userName)
        {
            txtCurrentUser.Text = userName;
            if (sbArrowNext != null) sbArrowNext.Begin();
            Reset_UI();
            bdrHints.Visibility = Visibility.Visible;
            if (sbSlideWelcome != null) sbSlideWelcome.Begin();
        }

        private void Reset_UI()
        {
            gridWelcomeUser.Visibility = Visibility.Visible;
            gridInstruction.Visibility = Visibility.Collapsed;
        }

        private void uiPreparation_Loaded(object sender, RoutedEventArgs e)
        {
            // Load animations
            sbArrowNext = TryFindResource("sbArrowNext") as Storyboard;
            sbArrowIndicator = TryFindResource("sbArrowIndicator") as Storyboard;
            sbSlideWelcome = TryFindResource("sbSlideWelcome") as Storyboard;
            sbSlideInfo = TryFindResource("sbSlideInfo") as Storyboard;
        }

        private void gridInfo_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sbSlideInfo != null)
            {
                gridInstruction.Visibility = Visibility.Visible;
                Globals.Sound.buttonSound.Play();
                sbSlideInfo.Begin();
            }
            e.Handled = true;
        }

        private void Storyboard_Completed(object sender, System.EventArgs e)
        {
            // Reset position
            if (sbArrowIndicator != null) sbArrowIndicator.Begin();
            gridWelcomeUser.Visibility = Visibility.Collapsed;
            gridWelcomeUser.RenderTransform = new TranslateTransform(0, 0);
            mainApp.btnNext.Visibility = Visibility.Visible;
        }

    }
}
