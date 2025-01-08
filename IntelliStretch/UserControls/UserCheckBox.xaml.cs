using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace IntelliStretch.UserControls
{
    public partial class UserCheckBox : UserControl
    {
        public UserCheckBox()
        {
            InitializeComponent();
        }

        #region Dependency Properties


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(UserCheckBox), new UIPropertyMetadata(null));


        public SolidColorBrush BrushColor
        {
            get { return (SolidColorBrush)GetValue(BrushColorProperty); }
            set { SetValue(BrushColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BrushColorProperty =
            DependencyProperty.Register("BrushColor", typeof(SolidColorBrush), typeof(UserCheckBox), new UIPropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public SolidColorBrush TextColor
        {
            get { return (SolidColorBrush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register("TextColor", typeof(SolidColorBrush), typeof(UserCheckBox), new UIPropertyMetadata(new SolidColorBrush(Colors.Black)));

        public SolidColorBrush BackColor
        {
            get { return (SolidColorBrush)GetValue(BackColorProperty); }
            set { SetValue(BackColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackColorProperty =
            DependencyProperty.Register("BackColor", typeof(SolidColorBrush), typeof(UserCheckBox), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(UserCheckBox), new UIPropertyMetadata(false));


        public bool IsTextVisible
        {
            get { return (bool)GetValue(IsTextVisibleProperty); }
            set { SetValue(IsTextVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTextVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTextVisibleProperty =
            DependencyProperty.Register("IsTextVisible", typeof(bool), typeof(UserCheckBox), new UIPropertyMetadata(true));



        public double CheckBorderThickness
        {
            get { return (double)GetValue(CheckBorderThicknessProperty); }
            set { SetValue(CheckBorderThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CheckBorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckBorderThicknessProperty =
            DependencyProperty.Register("CheckBorderThickness", typeof(double), typeof(UserCheckBox), new UIPropertyMetadata(2d));




        public Thickness BorderMargin
        {
            get { return (Thickness)GetValue(BorderMarginProperty); }
            set { SetValue(BorderMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderMarginProperty =
            DependencyProperty.Register("BorderMargin", typeof(Thickness), typeof(UserCheckBox), new UIPropertyMetadata(new Thickness(10d, 10d, 5d, 5d)));



        public Thickness ImageMargin
        {
            get { return (Thickness)GetValue(ImageMarginProperty); }
            set { SetValue(ImageMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageMarginProperty =
            DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(UserCheckBox), new UIPropertyMetadata(new Thickness(5d)));



        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(UserCheckBox), new UIPropertyMetadata(new Thickness(10d,0d,10d,0d)));


        #endregion

        #region Routed Events
        public event RoutedEventHandler CheckToggled
        {
            add { AddHandler(CheckToggledEvent, value); }
            remove { RemoveHandler(CheckToggledEvent, value); }
        }

        public static readonly RoutedEvent CheckToggledEvent =
            EventManager.RegisterRoutedEvent("CheckToggled", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UserCheckBox));
        #endregion

        private void userCheckBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsChecked = !IsChecked;
            RaiseEvent(new RoutedEventArgs(CheckToggledEvent));

        }
    }
}
