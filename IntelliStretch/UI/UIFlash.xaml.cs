using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Media.Imaging;
using IntelliStretch.Games;
using RehabGameLib;

namespace IntelliStretch.UI
{
    /// <summary>
    /// Interaction logic for UIFlash.xaml
    /// </summary>
    public partial class UIFlash : RehabGameBase
    {
        #region Constructors
        public UIFlash()
        {
            InitializeComponent();
        }

        public UIFlash(string source)
        {
            InitializeComponent();
            flashSource = source;
        }
        #endregion

        #region Variables
        AxShockwaveFlashObjects.AxShockwaveFlash axFlash;
        string flashSource;
        #endregion

        #region Methods

        private void HideFlash()
        {
            int imgWidth = (int)this.ActualWidth;
            int imgHeight = (int)this.ActualHeight;

            Bitmap bmp = new Bitmap(imgWidth, imgHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(imgWidth, imgHeight), CopyPixelOperation.SourceCopy);

            IntPtr hBitmap = bmp.GetHbitmap();
            BitmapSource imgSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            imgGame.Source = imgSource;
            DeleteObject(hBitmap);

            gridFlash.Visibility = Visibility.Collapsed;
            gridPreview.Visibility = Visibility.Visible;

        }

        private void ShowFlash()
        {
            gridFlash.Visibility = Visibility.Visible;
            gridPreview.Visibility = Visibility.Collapsed;

        }

        private void uiFlash_Loaded(object sender, RoutedEventArgs e)
        {
            // Load flash control
            WindowsFormsHost host = new WindowsFormsHost();
            axFlash = new AxShockwaveFlashObjects.AxShockwaveFlash();
            host.Visibility = Visibility.Collapsed;  // Trick to avoid initialization exception
            host.Child = axFlash;

            //ComponentResourceManager resources = new ComponentResourceManager(typeof(AxShockwaveFlashObjects.AxShockwaveFlash));
            //axFlash.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axFlash.OcxState")));
            //((ISupportInitialize)(axFlash)).EndInit();


            host.Visibility = Visibility.Visible;
            gridFlash.Children.Add(host);
            axFlash.Movie = flashSource;
            axFlash.FlashCall += new AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEventHandler(axFlash_FlashCall);
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        #endregion

        #region Flash ExternalInterface Handler

        private void SendToFlash(string functionName, string arg)
        {
            string methodName = "callFlash";  //methodName is fixed in the flash, is should be "callFlash"
            string[] paras = new string[2];
            paras[0] = functionName;  //paras[0] is the function name
            paras[1] = arg;  // parameter
            string cmd = FlashXML.GetXMLCmd(methodName, paras);

            try
            {
                axFlash.CallFunction(cmd);
            }
            catch (Exception)
            {
                return;
            }
        }

        private void SendToFlash(string functionName, string[] args)
        {
            string methodName = "callFlash";  //methodName is fixed in the flash, is should be "callFlash"
            int paramNum = args.Length+1;
            string[] paramArray = new string[paramNum];
            paramArray[0] = functionName;  //paras[0] is the function name
            for (int i = 1; i < paramNum; i++)
            {
                paramArray[i] = args[i - 1];
            }

            string cmd = FlashXML.GetXMLCmd(methodName, paramArray);
            axFlash.CallFunction(cmd);
        }
       
        // Flash call the C# method example 
        private void axFlash_FlashCall(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
        {
            string xmlMsg = e.request;
            string methodName = FlashXML.GetMethod(xmlMsg);
            string[] paramArray = FlashXML.GetParams(xmlMsg); //paras is the parameter array
            switch (methodName)
            {
                case "Target":
                    this.Target = Screen2Pos(Convert.ToSingle(paramArray[0]), this.FlexMax, this.ExtMax, this.CtrlPosMax, 0, this.isCtrlInversed);
                    break;
                case "Score":
                    this.TotalScore = Convert.ToInt32(paramArray[0]);
                    break;
            }
            if (GameEventNotified != null) GameEventNotified((GamePlayMode)Enum.Parse(typeof(GamePlayMode), methodName));
        }

        #endregion

        #region Abstract class implementation
              
        public override void UpdateUI(double data)
        {
            int newPos = (int) Pos2Screen(data, this.FlexMax, this.ExtMax, this.CtrlPosMax, 0, this.isCtrlInversed);

            try
            {
                if (axFlash != null) SendToFlash("setValue", newPos.ToString());
            }
            catch (Exception)
            {
                return;
            }
        }

        protected override void CollisionTest()
        {
            throw new NotImplementedException();
        }

        public override void CreateTarget(double target)
        {
            try
            {
                if (axFlash != null) SendToFlash("Target", ((int)target).ToString());
            }
            catch (Exception)
            {
                return;
            }
        }

        public override void ExitGame()
        {
            // dispose flash control
            axFlash.StopPlay();
            axFlash.Movie = "";
            gridFlash.Children.Clear();
            axFlash.Dispose();
            
            
        }

        public override void PauseGame()
        {
            HideFlash();
            if (axFlash != null) SendToFlash("pause", "");
        }

        public override void ResumeGame()
        {
            ShowFlash();
            if (axFlash != null) SendToFlash("resume", "");
        }

        public override void SetLevel(int level)
        {
            throw new NotImplementedException();
        }

        protected override void StartGame()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
