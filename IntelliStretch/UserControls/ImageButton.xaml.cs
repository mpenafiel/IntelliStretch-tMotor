using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IntelliStretch.UserControls
{
    /// <summary>
    /// Interaction logic for ImageButton.xaml
    /// </summary>
    public partial class ImageButton : UserControl
    {
        public ImageButton()
        {
            InitializeComponent();
        }

        #region Dependency Properties
 
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ImageButton), new UIPropertyMetadata(""));



        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageButton), new UIPropertyMetadata(null));



        public Thickness ImageMargin
        {
            get { return (Thickness)GetValue(ImageMarginProperty); }
            set { SetValue(ImageMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageMarginProperty =
            DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(ImageButton), new UIPropertyMetadata(new Thickness(0)));




        public SolidColorBrush BackColor
        {
            get { return (SolidColorBrush)GetValue(BackColorProperty); }
            set { SetValue(BackColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackColorProperty =
            DependencyProperty.Register("BackColor", typeof(SolidColorBrush), typeof(ImageButton), new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(0x7E, 0xA0, 0xA0, 0xA0))));

        public double ButtonCornerRadius
        {
            get { return (double)GetValue(ButtonCornerRadiusProperty); }
            set { SetValue(ButtonCornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonCornerRadiusProperty =
            DependencyProperty.Register("ButtonCornerRadius", typeof(double), typeof(ImageButton), new UIPropertyMetadata(16d));


        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ImageButton), new FrameworkPropertyMetadata(Orientation.Vertical));




        public bool IsReflected
        {
            get { return (bool)GetValue(IsReflectedProperty); }
            set { SetValue(IsReflectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReflected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReflectedProperty =
            DependencyProperty.Register("IsReflected", typeof(bool), typeof(ImageButton), new UIPropertyMetadata(false));


       
        #endregion

        #region Properties

        public bool IsPressed { get; set; }
        
        #endregion

        #region Routed Events
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ImageButton));
        #endregion


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IntelliStretch.Globals.Sound.buttonSound.Play();
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }
        
    }
}
