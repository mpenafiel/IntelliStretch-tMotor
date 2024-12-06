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
    public partial class HorizontalScroller : UserControl
    {
        public HorizontalScroller()
        {
            InitializeComponent();
        }

        #region Routed Events
        public event RoutedEventHandler ButtonMinClick
        {
            add { AddHandler(ButtonMinClickEvent, value); }
            remove { RemoveHandler(ButtonMinClickEvent, value); }
        }

        public static readonly RoutedEvent ButtonMinClickEvent =
            EventManager.RegisterRoutedEvent("ButtonMinClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HorizontalScroller));

        public event RoutedEventHandler ButtonMaxClick
        {
            add { AddHandler(ButtonMaxClickEvent, value); }
            remove { RemoveHandler(ButtonMaxClickEvent, value); }
        }

        public static readonly RoutedEvent ButtonMaxClickEvent =
            EventManager.RegisterRoutedEvent("ButtonMaxClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HorizontalScroller));

        #endregion

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ButtonMinClickEvent));
        }

        private void btnMax_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ButtonMaxClickEvent));
        }
    }
}
