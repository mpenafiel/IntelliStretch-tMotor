using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IntelliStretch.UserControls
{
    /// <summary>
    /// Interaction logic for ColorButton.xaml
    /// </summary>
    public partial class ColorButton : UserControl
    {
        public ColorButton()
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
            DependencyProperty.Register("Text", typeof(string), typeof(ColorButton), new UIPropertyMetadata(""));


        public double TextFontSize
        {
            get { return (double)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextFontSizeProperty =
            DependencyProperty.Register("TextFontSize", typeof(double), typeof(ColorButton), new UIPropertyMetadata(16d));

        public double ButtonCornerRadius
        {
            get { return (double)GetValue(ButtonCornerRadiusProperty); }
            set { SetValue(ButtonCornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonCornerRadiusProperty =
            DependencyProperty.Register("ButtonCornerRadius", typeof(double), typeof(ColorButton), new UIPropertyMetadata(16d));


        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorButton), new UIPropertyMetadata(Colors.Black));


        public Color GradientColor
        {
            get { return (Color)GetValue(GradientColorProperty); }
            set { SetValue(GradientColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GradientColorProperty =
            DependencyProperty.Register("GradientColor", typeof(Color), typeof(ColorButton), new UIPropertyMetadata(Colors.WhiteSmoke));

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register("TextColor", typeof(Color), typeof(ColorButton), new UIPropertyMetadata(Colors.Black));


        #endregion

        #region Routed Events
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorButton));
        #endregion


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IntelliStretch.Globals.Sound.buttonSound.Play();
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }


    }
}
