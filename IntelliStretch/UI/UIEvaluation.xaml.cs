using CsvHelper;
using IntelliStretch.Data;
//using NationalInstruments;
//using NationalInstruments.DAQmx;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NationalInstruments;
using NationalInstruments.DAQmx;
using static IntelliStretch.Protocols;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using ScottPlot.AxisPanels;

namespace IntelliStretch.UI
{
    public partial class UIEvaluation : UserControl
    {
        #region Plot Components
        private ScottPlot.Plottables.DataStreamer Streamer1 = null;
        private ScottPlot.Plottables.DataStreamer Streamer2 = null;
        private DataStreamer[] dataStreamers = new DataStreamer[2];

        private WpfPlot[] plots = new WpfPlot[2];

        private ScottPlot.Plottables.VerticalLine VLine1;
        private ScottPlot.Plottables.VerticalLine VLine2;
        #endregion

        #region NI DAQ
        private AnalogMultiChannelReader analogInReader;
        private NationalInstruments.DAQmx.Task myTask;
        private NationalInstruments.DAQmx.Task runningTask;
        private AsyncCallback analogCallback;
        private AnalogWaveform<double>[] data;

        public bool isStreamingDAQ = false;

        #endregion

        readonly System.Windows.Forms.Timer UpdatePlotTimer = new System.Windows.Forms.Timer() { Interval = 50, Enabled = true };

        public UIEvaluation()
        {
            InitializeComponent();

            //plot new data here using seperate timer function
            UpdatePlotTimer.Tick += (s, e) =>
            {
                if (Streamer1 != null && Streamer1.HasNewData)
                {
                    VLine1.IsVisible = Streamer1.Renderer is ScottPlot.DataViews.Wipe;
                    VLine1.Position = Streamer1.Data.NextIndex * Streamer1.Data.SamplePeriod + Streamer1.Data.OffsetX;

                    VLine2.IsVisible = Streamer2.Renderer is ScottPlot.DataViews.Wipe;
                    VLine2.Position = Streamer2.Data.NextIndex * Streamer2.Data.SamplePeriod + Streamer2.Data.OffsetX;

                    vPlot1.Refresh();
                    vPlot2.Refresh();
                    hPlot1.Refresh();
                    hPlot2.Refresh();
                }
            };
        }



        #region Variables
        IntelliSerialPort sp;
        //Streamer streamer;
        Protocols.IntelliProtocol intelliProtocol;
        MainApp mainApp;
        Interfaces.IUpdateUI currentUI;
        string measureMode;
        bool IsSavingData;

        private StreamWriter writer = null;
        private string fileContent = string.Empty;
        private string filePath = string.Empty;
        private CsvWriter csv = null;
        public string DataDir { get; set; }
        public string DataFilePrefix { get; set; }

        private Protocols.DaqProtocol daqProtocol;

        #endregion

        #region Dependency properties


        public string FlexionName
        {
            get { return (string)GetValue(FlexionNameProperty); }
            set { SetValue(FlexionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlexionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlexionNameProperty =
            DependencyProperty.Register("FlexionName", typeof(string), typeof(UIEvaluation), new UIPropertyMetadata("Flexion"));



        public string ExtensionName
        {
            get { return (string)GetValue(ExtensionNameProperty); }
            set { SetValue(ExtensionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtensionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtensionNameProperty =
            DependencyProperty.Register("ExtensionName", typeof(string), typeof(UIEvaluation), new UIPropertyMetadata("Extension"));

        public ImageSource FlexionImage
        {
            get { return (ImageSource)GetValue(FlexionImageProperty); }
            set { SetValue(FlexionImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlexionImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlexionImageProperty =
            DependencyProperty.Register("FlexionImage", typeof(ImageSource), typeof(UIEvaluation), new UIPropertyMetadata(null));


        public ImageSource ExtensionImage
        {
            get { return (ImageSource)GetValue(ExtensionImageProperty); }
            set { SetValue(ExtensionImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtensionImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtensionImageProperty =
            DependencyProperty.Register("ExtensionImage", typeof(ImageSource), typeof(UIEvaluation), new UIPropertyMetadata(null));

        #endregion

        public void Load_Evaluation(MainApp app, IntelliSerialPort port)
        {
            mainApp = app;
            sp = port;
            intelliProtocol = mainApp.IntelliProtocol;
            Protocols.GeneralSettings generalProtocol = intelliProtocol.General;
            Protocols.StretchingProtocol stretchProtocol = intelliProtocol.Stretching;
            daqProtocol = intelliProtocol.DAQ;
            IsSavingData = intelliProtocol.System.IsSavingData;

            // Initalize control
            switch (generalProtocol.Joint)
            {
                case Protocols.Joint.Ankle:
                case Protocols.Joint.Knee:
                    vAROM.Visibility = Visibility.Visible;
                    hAROM.Visibility = Visibility.Collapsed;
                    vStrengthLayout.Visibility = Visibility.Visible;
                    vAROMLayout.Visibility = Visibility.Visible;
                    hStrengthLayout.Visibility = Visibility.Collapsed;
                    hAROMLayout.Visibility = Visibility.Collapsed;
                    vAROM.Initialize_Layout(generalProtocol.FlexionMax, generalProtocol.ExtensionMax);
                    vStrength.Initialize_Layout(stretchProtocol.FlexionTorqueMax, -stretchProtocol.ExtensionTorqueMax);
                    InitializePlots(vPlot1, vPlot2);
                    break;

                case Protocols.Joint.Elbow:
                case Protocols.Joint.Wrist:
                    vAROM.Visibility = Visibility.Collapsed;
                    hAROM.Visibility = Visibility.Visible;
                    vStrengthLayout.Visibility = Visibility.Collapsed;
                    vAROMLayout.Visibility = Visibility.Collapsed;
                    hStrengthLayout.Visibility = Visibility.Visible;
                    hAROMLayout.Visibility = Visibility.Visible;
                    hAROM.Initialize_Layout(generalProtocol.FlexionMax, generalProtocol.ExtensionMax);
                    hStrength.Initialize_Layout(stretchProtocol.FlexionTorqueMax, -stretchProtocol.ExtensionTorqueMax);
                    InitializePlots(hPlot1, hPlot2);
                    break;

                default:
                    break;
            }

            Check_CurrentUI();
            sliderLockPosition.Maximum = generalProtocol.FlexionMax;
            sliderLockPosition.Minimum = generalProtocol.ExtensionMax;

            // Add event handler
            sp.UpdateData = new IntelliSerialPort.DelegateUpdateData(Update_UI);
            sp.WriteCmd("BK");

        }

        public bool IsDAQStreaming()
        {
            return isStreamingDAQ;
        }


        private void InitializePlots(ScottPlot.WPF.WpfPlot plot1, ScottPlot.WPF.WpfPlot plot2)
        {
            StylePlot(plot1);
            StylePlot(plot2);

            Streamer1 = plot1.Plot.Add.DataStreamer(3000);
            Streamer2 = plot2.Plot.Add.DataStreamer(3000);

            Streamer1.Color = ScottPlot.Color.FromHex("#348EF6"); //#e5ff24
            Streamer2.Color = ScottPlot.Color.FromHex("#348EF6"); //#e5ff24

            dataStreamers[0] = Streamer1;
            dataStreamers[1] = Streamer2;

            dataStreamers[0].LineWidth = 3;
            dataStreamers[1].LineWidth = 3;

            plots[0] = plot1;
            plots[1] = plot2;

            VLine1 = plot1.Plot.Add.VerticalLine(0, 2, ScottPlot.Colors.Red);
            VLine1.LineWidth = 5;
            VLine2 = plot2.Plot.Add.VerticalLine(0, 2, ScottPlot.Colors.Red);
            VLine2.LineWidth = 5;

            plot1.Plot.Axes.ContinuouslyAutoscale = false;
            Streamer1.ManageAxisLimits = true;

            plot2.Plot.Axes.ContinuouslyAutoscale = false;
            Streamer2.ManageAxisLimits = true;

            plot1.Plot.Axes.SetLimitsY(-3, 3);
            plot2.Plot.Axes.SetLimitsY(-3, 3);
        }

        public void StylePlot(ScottPlot.WPF.WpfPlot plot)
        {
            
            plot.Plot.FigureBackground.Color = ScottPlot.Colors.Transparent;
            plot.Plot.Grid.MajorLineColor = ScottPlot.Colors.LightSlateGray;

            /* Original styling removes tick marks
            //remove default frame and add left and bottom axes
            plot.Plot.Axes.Frameless();
            ScottPlot.AxisPanels.LeftAxis leftAxis = plot.Plot.Axes.AddLeftAxis();
            ScottPlot.AxisPanels.BottomAxis bottomAxis = plot.Plot.Axes.AddBottomAxis();

            //Style left axis
            leftAxis.FrameLineStyle.Width = 3;
            leftAxis.Color(ScottPlot.Colors.Gray);


            //Style bottom axis
            bottomAxis.FrameLineStyle.Width = 3;
            bottomAxis.Color(ScottPlot.Colors.Gray);

            */

            // New styling with tick marks

            // Remove top and right axes
            plot.Plot.Axes.Right.FrameLineStyle.Width = 0;
            plot.Plot.Axes.Top.FrameLineStyle.Width = 0;
            ScottPlot.AxisPanels.LeftAxis leftAxis = (LeftAxis)plot.Plot.Axes.Left;
            ScottPlot.AxisPanels.BottomAxis bottomAxis = (BottomAxis)plot.Plot.Axes.Bottom;

            // Style left axis
            leftAxis.MajorTickStyle.Length = 8;
            leftAxis.MajorTickStyle.Width = 1.5f;
            leftAxis.MinorTickStyle.Length = 4;
            leftAxis.MinorTickStyle.Width = 1.5f;
            leftAxis.TickLabelStyle.FontSize = 20;
            leftAxis.FrameLineStyle.Width = 3;
            leftAxis.Color(ScottPlot.Colors.Gray);
            leftAxis.LabelText = "Amplitude (mV)";

            // Style axis
            bottomAxis.TickLabelStyle.IsVisible = false;
            bottomAxis.MajorTickStyle.Length = 0;
            bottomAxis.MinorTickStyle.Length = 0;
            bottomAxis.FrameLineStyle.Width = 3;
            bottomAxis.Color(ScottPlot.Colors.Gray);


            plot.Refresh();
        }

        private void Update_UI(IntelliSerialPort.AnkleData newAnkleData) => this.Dispatcher.Invoke(new Action(delegate
                                                                                     {
                                                                                         // Motor torque value does not have polarity, assess within UI
                                                                                         if (measureMode == "Strength")
                                                                                         {
                                                                                             if (btnFlexion.IsChecked) newAnkleData.ankleTorque = -newAnkleData.ankleTorque;
                                                                                         }
                                                                                         currentUI.Update_UI(newAnkleData);

                                                                                     }));

        private void btnMeasure_Click(object sender, RoutedEventArgs e)
        {
            btnMeasure.IsPressed = !btnMeasure.IsPressed;  // Check or uncheck the button

            if (btnMeasure.IsPressed)
            {
                if (cbDisplayEmg.IsChecked == true && tabStrength.IsSelected)
                {
                    try
                    {
                        if (runningTask == null)
                        {
                            // Create a new task
                            myTask = new NationalInstruments.DAQmx.Task();

                            // Create a virtual channel
                            myTask.AIChannels.CreateVoltageChannel(daqProtocol.Channel1, "",
                                AITerminalConfiguration.Rse, -5.0,
                                5.0, AIVoltageUnits.Volts);

                            // Create a virtual channel
                            myTask.AIChannels.CreateVoltageChannel(daqProtocol.Channel2, "",
                                AITerminalConfiguration.Rse, -5.0,
                                5.0, AIVoltageUnits.Volts);

                            // Configure the timing parameters
                            myTask.Timing.ConfigureSampleClock("", daqProtocol.SamplingRate,
                                SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, daqProtocol.SampPerChan);

                            // Verify the Task
                            myTask.Control(TaskAction.Verify);

                            runningTask = myTask;
                            analogInReader = new AnalogMultiChannelReader(myTask.Stream);
                            analogCallback = new AsyncCallback(AnalogInCallback);

                            // Use SynchronizeCallbacks to specify that the object 
                            // marshals callbacks across threads appropriately.
                            analogInReader.SynchronizeCallbacks = true;
                            analogInReader.BeginReadWaveform(daqProtocol.SampPerChan,
                                analogCallback, myTask);

                            if ((bool)btnRecord.IsChecked) Create_Writer();
                            isStreamingDAQ = true;
                        }

                    }
                    catch (DaqException exception)
                    {
                        // Check or uncheck the button
                        btnMeasure.IsPressed = !btnMeasure.IsPressed;

                        runningTask = null;
                        isStreamingDAQ = false;
                        myTask.Dispose();
                        sp.IsUpdating = false;
                        WriterHandler();
                        if (IsSavingData) sp.Stop_SaveData();
                        UI_Handler();

                        // Display Errors
                        MessageBox.Show(exception.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                UI_Handler();
                currentUI.Set_Initial(true);
                if (measureMode == "AROM" && sp.IsConnected) sp.WriteCmd("BK");//Add BK , Yupeng 04.2013 

                if (IsSavingData) sp.Start_SaveData(measureMode);
                sp.IsUpdating = true;

                  
            }
            else
            {
                if (runningTask != null)
                {
                    runningTask = null;
                    myTask.Dispose();
                    isStreamingDAQ = false;
                }
                WriterHandler();
                sp.IsUpdating = false;
                if (btnLock.IsChecked) switch_Device_Mode();
                if (IsSavingData)
                {
                    sp.Stop_SaveData();
                }
                UI_Handler();
            }     
        }

        private void Create_Writer()
        {
            string dataFile = DataDir + DataFilePrefix + "_EMG_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";


            //create new instance of writer
            if (writer == null) { writer = new StreamWriter(dataFile); }
            //create new instance of csv writer
            if (csv == null) { csv = new CsvWriter(writer, CultureInfo.InvariantCulture); }
        }

        private void WriterHandler()
        {
            if (writer != null)
            {
                csv = null;
                writer.Close();
                writer = null;
            }
        }

        static async System.Threading.Tasks.Task DataWriter(StreamWriter writer, CsvWriter csv, double[,] data)
        {
            for (int i = 0; i < data.GetLength(1); i++)
            {
                double ch1 = data[0, i];
                double ch2 = data[1, i];

                //write each line of data as tab-delimited
                csv.WriteRecord(ch1);
                csv.WriteRecord(ch2);
                csv.NextRecord();
            }
        }

        private async void AnalogInCallback(IAsyncResult ar)
        {
            try
            {
                if (runningTask != null && runningTask == ar.AsyncState)
                {
                    // Read the available data from the channels
                    data = analogInReader.EndReadWaveform(ar);
                    //Console.WriteLine(data.Length);

                    int counter = 0;

                    //scale data according to designated multiplier


                    //perform additional tasks if recording
                    if ((bool)btnRecord.IsChecked && writer != null && csv != null)
                    {
                        //create a two-dimensional array
                        double[,] data_2d = new double[data.Length, data[0].SampleCount];
                        for (int i = 0; i < data_2d.GetLength(0); i++)
                        {
                            int count = data[i].SampleCount;
                            //convert data in channel from AnalogWaveform<double> to double[]
                            double[] dataPerCh = data[i].GetRawData();
                            for (int j = 0; j < data_2d.GetLength(1); j++)
                            {
                                data_2d[i, j] = dataPerCh[j];
                            }
                        }
                        await DataWriter(writer, csv, data_2d);
                    }

                    foreach (AnalogWaveform<double> waveform in data)
                    {
                        int count = waveform.Samples.Count();
                        double[] dataPerCh = new double[waveform.GetRawData().Length];

                        dataPerCh = waveform.GetRawData();

                        dataStreamers[counter].AddRange(dataPerCh);
                        // slide marker to the left
                        plots[counter].Plot.GetPlottables<Marker>()
                            .ToList()
                            .ForEach(m => m.X -= count);

                        // remove off-screen marks
                        plots[counter].Plot.GetPlottables<Marker>()
                            .Where(m => m.X < 0)
                            .ToList()
                            .ForEach(m => plots[counter].Plot.Remove(m));

                        counter++;
                    }

                    analogInReader.BeginMemoryOptimizedReadWaveform(daqProtocol.SampPerChan,
                        analogCallback, myTask, data);
                }
            }
            catch (DaqException exception)
            {
                runningTask = null;
                myTask.Dispose();
                sp.IsUpdating = false;
                if (IsSavingData) sp.Stop_SaveData();

                UI_Handler();

                // Display Errors
                MessageBox.Show(exception.Message);
            }
        }

        private void UI_Handler()
        {
            if (btnMeasure.IsPressed)
            {
                btnMeasure.Image = Utilities.GetImage("Stop-new.png");
                btnMeasure.Text = "Stop ";
                mainApp.Buttons_Enabled(false);
                tabItems_Enabled(false);
            }
            else
            {
                btnMeasure.Image = Utilities.GetImage("Start-new.png");
                btnMeasure.Text = "Measure ";
                Apply_Measure();
                mainApp.Buttons_Enabled(true);
                tabItems_Enabled(true);
            }
        }

        private void Apply_Measure()
        {
            if (measureMode == "AROM")
            {
                // Save AROM
                if (vAROM.Visibility == Visibility.Visible)
                {
                    intelliProtocol.General.ActiveFlexionMax = (int)vAROM.ActiveFlexionMax;
                    intelliProtocol.General.ActiveExtensionMax = (int)vAROM.ActiveExtensionMax;
                }
                else
                {
                    intelliProtocol.General.ActiveFlexionMax = (int)hAROM.ActiveFlexionMax;
                    intelliProtocol.General.ActiveExtensionMax = (int)hAROM.ActiveExtensionMax;
                }
            }
        }

        private void switch_Device_Mode()
        {
            if (btnBackdrivable.IsChecked)
            {
                btnBackdrivable.IsChecked = false;
                btnLock.IsChecked = true;
                if (sp.IsConnected) sp.WriteCmd("LK");//change from BK to RL Yupeng 04.2013 //change from RL to BK, Michael 08.20.2024
            }
            else if (btnLock.IsChecked)
            {
                btnBackdrivable.IsChecked = true;
                btnLock.IsChecked = false;
                if (sp.IsConnected) sp.WriteCmd("BK"); //added Michael, 08.2024
            }
        }

        private void sliderLockPosition_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (btnLock.IsChecked) btnLock.IsChecked = false;
        }

        private void tabEvaluation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Check_CurrentUI();
            e.Handled = true;
        }

        private void btnBackdrivable_Click(object sender, RoutedEventArgs e)
        {
            btnBackdrivable.IsChecked = true;
            btnLock.IsChecked = false;
            if (sp.IsConnected) sp.WriteCmd("BK");//change from BK to RL Yupeng 04.2013 //change from RL to BK, Michael 08.20.2024
        }
        private void btnLock_Click(object sender, RoutedEventArgs e)
        {
            btnBackdrivable.IsChecked = false;
            btnLock.IsChecked = true;
            if (sp.IsConnected) sp.WriteCmd("LK"); //added Michael, 08.2024
            //{

                
                //sp.WriteCmd("ML10"); //deleted, Michael 08.2024
                //sp.WriteCmd("PS" + ((int)-sliderLockPosition.Value).ToString()); //deleted, Michael, 08.2024
                //sp.WriteCmd("DT38"); //change from 30 to 38 , Yupeng, 04.2013 //deleted, Michael, 08.2024
                //sp.WriteCmd("PT38"); //change from 30 to 38 , Yupeng, 04.2013 //deleted, Michael, 08.2024
                //sp.WriteCmd("SC"); //deleted, Michael, 08.2024
            //}
        }

        private void Check_CurrentUI()
        {
            switch ((tabEvaluation.SelectedItem as TabItem).Name)
            {
                case "tabAROM":
                    measureMode = "AROM";
                    if (vAROM.Visibility == Visibility.Visible) currentUI = vAROM;
                    else currentUI = hAROM;
                    currentUI.Set_DataMode(DataInfo.DataMode.Position);
                    
                    break;

                case "tabStrength":
                    measureMode = "Strength";
                    if (vStrength.Visibility == Visibility.Visible) currentUI = vStrength;
                    else currentUI = hStrength;
                    currentUI.Set_DataMode(DataInfo.DataMode.Torque);
                    break;

                default:
                    break;
            }
        }

        private void sliderResistance_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (sp.IsConnected)
            {
                if (sliderResistance.Value == 0)//(sliderResistFlex.Value == 0 && sliderResistExt.Value == 0)
                    sp.WriteCmd("BK"); // No loading => backdrivable
                else
                {
                    sp.WriteCmd("FC"); // Friction control
                    sp.WriteCmd($"FR{sliderResistance.Value}"); // set non-directional Resistance
                    //sp.WriteCmd($"FP{sliderResistFlex.Value}"); // Set Dorsi/flexion Resistance            
                    //sp.WriteCmd($"FD{sliderResistExt.Value}"); // Set plantar/extension Resistance
                }
            }
            e.Handled = true;
        }

        private void tabItems_Enabled(bool IsEnabled)
        {
            foreach (TabItem item in tabEvaluation.Items)
            {
                if (!item.IsSelected) item.IsEnabled = IsEnabled;  // Disable other tab items except current one
            }
        }

        private void cbDisplayEmg_Changed(object sender, RoutedEventArgs e)
        {
            if (intelliProtocol.General.Joint == Protocols.Joint.Ankle | intelliProtocol.General.Joint == Protocols.Joint.Knee)
            {
                if (cbDisplayEmg.IsChecked == true)
                {
                    vPlot1.Visibility = Visibility.Visible;
                    vPlot2.Visibility = Visibility.Visible;

                    strength_v_FlexionGrid.SetValue(Grid.ColumnProperty, 2);
                    strength_v_ExtensionGrid.SetValue(Grid.ColumnProperty, 2);

                    vStrength.SetValue(Grid.ColumnSpanProperty, 1);
                    emgStack.Visibility = Visibility.Visible;
                }
                else
                {
                    vPlot1.Visibility = Visibility.Collapsed;
                    vPlot2.Visibility = Visibility.Collapsed;

                    strength_v_FlexionGrid.SetValue(Grid.ColumnProperty, 2);
                    strength_v_ExtensionGrid.SetValue(Grid.ColumnProperty, 2);

                    vStrength.SetValue(Grid.ColumnSpanProperty, 2);
                    emgStack.Visibility = Visibility.Collapsed;
                }
            }
            else if (intelliProtocol.General.Joint == Protocols.Joint.Elbow | intelliProtocol.General.Joint == Protocols.Joint.Wrist)
            {
                if (cbDisplayEmg.IsChecked == true)
                {
                    hPlot1.Visibility = Visibility.Visible;
                    hPlot2.Visibility = Visibility.Visible;

                    strength_h_FlexionGrid.SetValue(Grid.ColumnProperty, 1);
                    strength_h_ExtensionGrid.SetValue(Grid.ColumnProperty, 0);

                    strength_h_FlexionGrid.SetValue(Grid.RowProperty, 1);
                    strength_h_ExtensionGrid.SetValue(Grid.RowProperty, 1);

                    strength_h_ExtensionGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    strength_h_FlexionGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                    hStrength.SetValue(Grid.RowProperty, 2);
                    hStrength.SetValue(Grid.RowSpanProperty, 1);
                    emgStack.Visibility = Visibility.Visible;
                }
                else
                {
                    hPlot1.Visibility = Visibility.Collapsed;
                    hPlot2.Visibility = Visibility.Collapsed;

                    strength_h_FlexionGrid.SetValue(Grid.RowProperty, 0);
                    strength_h_ExtensionGrid.SetValue(Grid.RowProperty, 0);

                    hStrength.SetValue(Grid.RowProperty, 1);
                    hStrength.SetValue(Grid.RowSpanProperty, 2);
                    emgStack.Visibility = Visibility.Collapsed;

                    strength_h_ExtensionGrid.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    strength_h_FlexionGrid.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                    if (intelliProtocol.General.JointSide == Protocols.JointSide.Left)
                    {
                        strength_h_ExtensionGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        strength_h_FlexionGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

                        strength_h_FlexionGrid.SetValue(Grid.ColumnProperty, 0);
                        strength_h_ExtensionGrid.SetValue(Grid.ColumnProperty, 1);
                    }
                    else if (intelliProtocol.General.JointSide == Protocols.JointSide.Right)
                    {
                        strength_h_ExtensionGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        strength_h_FlexionGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

                        strength_h_FlexionGrid.SetValue(Grid.ColumnProperty, 1);
                        strength_h_ExtensionGrid.SetValue(Grid.ColumnProperty, 0);
                    }
                }
            }
        }

        private void switch_EMG_Scale(object sender, RoutedEventArgs e)
        {
            /*if (x1.IsChecked)
            {
                x1.IsChecked = true;
                x10.IsChecked = false;
                x50.IsChecked = false;
                x100.IsChecked = false;
            }
            else if (x10.IsChecked)
            {
                x1.IsChecked = false;
                x10.IsChecked = true;
                x50.IsChecked = false;
                x100.IsChecked = false;
            }
            else if (x50.IsChecked)
            {
                x1.IsChecked = false;
                x10.IsChecked = false;
                x50.IsChecked = true;
                x100.IsChecked = false;
            }
            else if (x10.IsChecked)
            {
                x1.IsChecked = false;
                x10.IsChecked = false;
                x50.IsChecked = false;
                x100.IsChecked = true;
            }*/
        }

        private void btnFlexion_Click(object sender, RoutedEventArgs e)
        {
            btnFlexion.IsChecked = true;
            btnExtension.IsChecked = false;
        }

        private void btnExtension_Click(object sender, RoutedEventArgs e)
        {
            btnFlexion.IsChecked = false;
            btnExtension.IsChecked = true;
        }

        private void x1_Click(object sender, RoutedEventArgs e)
        {
            //x1.IsChecked = true;
            //x10.IsChecked = false;
            //x50.IsChecked = false;
            //x100.IsChecked = false;
        }

        private void x10_Click(object sender, RoutedEventArgs e)
        {
            //x1.IsChecked = false;
            //x10.IsChecked = true;
            //x50.IsChecked = false;
            //x100.IsChecked = false;
        }

        private void x50_Click(object sender, RoutedEventArgs e)
        {
            //x1.IsChecked = false;
            //x10.IsChecked = false;
            //x50.IsChecked = true;
            //x100.IsChecked = false;
        }

        private void x100_Click(object sender, RoutedEventArgs e)
        {
            //x1.IsChecked = false;
            //x10.IsChecked = false;
            //x50.IsChecked = false;
            //x100.IsChecked = true;
        }
    }
}
