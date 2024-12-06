using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IntelliStretch.UserControls
{
    /// <summary>
    /// Interaction logic for FlatTextButton.xaml
    /// </summary>
    public partial class FlatTextButton : UserControl
    {
        public FlatTextButton()
        {
            InitializeComponent();
        }

        #region Dependency Properties

        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(FlatTextButton), new UIPropertyMetadata(null));
        public string ButtonFontSize
        {
            get { return (string)GetValue(ButtonFontSizeProperty); }
            set { SetValue(ButtonFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonFontSizeProperty =
            DependencyProperty.Register("ButtonFontSize", typeof(string), typeof(FlatTextButton), new UIPropertyMetadata(null));

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(FlatTextButton), new UIPropertyMetadata(null));

        public string TextBoxText
        {
            get { return (string)GetValue(TextBoxTextProperty); }
            set { SetValue(TextBoxTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextBoxTextProperty =
            DependencyProperty.Register("TextBoxText", typeof(string), typeof(FlatTextButton), new UIPropertyMetadata(null));

        #endregion

        #region Routed Events
        public event RoutedEventHandler ButtonClick
        {
            add { AddHandler(ButtonClickEvent, value); }
            remove { RemoveHandler(ButtonClickEvent, value); }
        }

        public static readonly RoutedEvent ButtonClickEvent =
            EventManager.RegisterRoutedEvent("ButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FlatTextButton));

        #endregion


        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            Globals.Sound.buttonSound.Play();
            RaiseEvent(new RoutedEventArgs(ButtonClickEvent));
        }

        private void flatTextButton_GotFocus(object sender, RoutedEventArgs e)
        {
            bdrHighlight.BorderBrush = new SolidColorBrush(Colors.Red);
        }

        private void flatTextButton_LostFocus(object sender, RoutedEventArgs e)
        {
            bdrHighlight.BorderBrush = null;
        }
    }
}
