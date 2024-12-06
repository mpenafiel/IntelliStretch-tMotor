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
using System.Windows.Threading;

namespace IntelliStretch.UserControls
{
    /// <summary>
    /// Interaction logic for StopWatch.xaml
    /// </summary>
    public partial class StopWatch : UserControl
    {
        public StopWatch()
        {
            InitializeComponent();
            timerStopWatch = new DispatcherTimer();
            timerStopWatch.Tick += new EventHandler(timerStopWatch_Tick);
            timerStopWatch.Interval = new TimeSpan(0, 0, 1);  // default interval = 1 sec
        }

        #region Variables

        DispatcherTimer timerStopWatch;

        #endregion

        #region Properties

        public int Min { get; set; }
        public int Sec { get; set; }
        #endregion

        #region Routed Events

        public event RoutedEventHandler Tick
        {
            add { AddHandler(TickEvent, value); }
            remove { RemoveHandler(TickEvent, value); }
        }

        public static readonly RoutedEvent TickEvent =
            EventManager.RegisterRoutedEvent("Tick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(StopWatch));

        #endregion

        public void Start()
        {
            Reset();
            timerStopWatch.Start(); // Start embedded timer
        }

        public void Stop()
        {
            timerStopWatch.Stop();  // Stop timer
        }

        private void Reset()
        {
            Min = 0;
            Sec = 0;
            Update_Clock();
        }

        private void Tick_Clock()
        {
            if (++Sec >= 60)
            {
                Min++;
                Sec = 0;
            }
        }

        private void timerStopWatch_Tick(object sender, EventArgs e)
        {
            Tick_Clock();  // ticking time
            Update_Clock();  // update time clock
            RaiseEvent(new RoutedEventArgs(TickEvent));  // raise user-defined tick event
        }

        private void Update_Clock()
        {
            txtTime.Text = Min.ToString("00") + ":" + Sec.ToString("00");
        }
    }
}
