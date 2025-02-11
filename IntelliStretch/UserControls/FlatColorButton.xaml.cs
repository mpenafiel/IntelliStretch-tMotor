using System;
using System.Collections.Generic;
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
    public partial class FlatColorButton : UserControl
    {
        public FlatColorButton()
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
            DependencyProperty.Register("Text", typeof(string), typeof(FlatColorButton), new UIPropertyMetadata(""));

        public Color BackColor
        {
            get { return (Color)GetValue(BackColorProperty); }
            set { SetValue(BackColorProperty, value); }
        }

        public static readonly DependencyProperty BackColorProperty = 
            DependencyProperty.Register("BackColor", typeof(Color), typeof(FlatColorButton), new UIPropertyMetadata(Colors.Green));

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register("TextColor", typeof(Color), typeof(FlatColorButton), new UIPropertyMetadata(Colors.Black));

        #endregion

        #region Routed Events
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FlatColorButton));
        #endregion

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            IntelliStretch.Globals.Sound.buttonSound.Play();
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }
    }
}
