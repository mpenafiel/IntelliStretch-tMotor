using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using _3DTools;
using System.Windows.Media.Animation;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using IntelliStretch.Games;
using RehabGameLib;

namespace IntelliStretch.UI
{
    public partial class UICoverFlow : UserControl
    {
        public UICoverFlow()
        {
            InitializeComponent();
            Initialize_UserComponent();
        }

        #region Variables
        IntelliSerialPort sp;
        MainApp mainApp;

        List<Games.GameInfo> gameLibrary = new List<Games.GameInfo>(), gameList = new List<Games.GameInfo>(); // Game list
        Protocols.GeneralSettings generalSettings;
        Protocols.GameProtocol gameProtocol;
        Protocols.SystemSettings systemSettings;

        string gamePath;
        RehabGameBase rehabGame;
        Games.GameInfo selectedGame;
        GameControl gameControl;
        int newTarget;
        object objMutex = new object();

        // Game control
        int prevPos;
        INPUT mouseInput;
        #endregion

        #region Dependency Properties

        #region CurrentMidIndexProperty
        public static readonly DependencyProperty CurrentMidIndexProperty = DependencyProperty.Register(
            "CurrentMidIndex", typeof(double), typeof(UICoverFlow),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(CurrentMidIndexPropertyChangedCallback)));

        private static void CurrentMidIndexPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            UICoverFlow win = sender as UICoverFlow;
            if (win != null)
            {
                win.ReLayoutInteractiveVisual3D();
            }
        }

        /// <summary>
        /// Get or Set current index
        /// </summary>
        public double CurrentMidIndex
        {
            get
            {
                return (double)this.GetValue(CurrentMidIndexProperty);
            }
            set
            {
                this.SetValue(CurrentMidIndexProperty, value);
            }
        }
        #endregion

        #region ModelAngleProperty
        public static readonly DependencyProperty ModelAngleProperty = DependencyProperty.Register(
            "ModelAngle", typeof(double), typeof(UICoverFlow),
            new FrameworkPropertyMetadata(70.0, new PropertyChangedCallback(ModelAnglePropertyChangedCallback)));


        private static void ModelAnglePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            UICoverFlow win = sender as UICoverFlow;
            if (win != null)
            {
                win.ReLayoutInteractiveVisual3D();
            }
        }

        /// <summary>
        /// Y axis rotation angle
        /// </summary>
        public double ModelAngle
        {
            get
            {
                return (double)this.GetValue(ModelAngleProperty);
            }
            set
            {
                this.SetValue(ModelAngleProperty, value);
            }
        }
        #endregion

        #region XDistanceBetweenModelsProperty
        public static readonly DependencyProperty XDistanceBetweenModelsProperty = DependencyProperty.Register(
            "XDistanceBetweenModels", typeof(double), typeof(UICoverFlow),
            new FrameworkPropertyMetadata(0.5, XDistanceBetweenModelsPropertyChangedCallback));

        private static void XDistanceBetweenModelsPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            UICoverFlow win = sender as UICoverFlow;
            if (win != null)
            {
                win.ReLayoutInteractiveVisual3D();
            }
        }

        /// <summary>
        /// Distance between model on X axis
        /// </summary>
        public double XDistanceBetweenModels
        {
            get
            {
                return (double)this.GetValue(XDistanceBetweenModelsProperty);
            }
            set
            {
                this.SetValue(XDistanceBetweenModelsProperty, value);
            }
        }
        #endregion

        #region ZDistanceBetweenModelsProperty
        public static readonly DependencyProperty ZDistanceBetweenModelsProperty = DependencyProperty.Register(
            "ZDistanceBetweenModels", typeof(double), typeof(UICoverFlow),
            new FrameworkPropertyMetadata(0.5, ZDistanceBetweenModelsPropertyChangedCallback));

        private static void ZDistanceBetweenModelsPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            UICoverFlow win = sender as UICoverFlow;
            if (win != null)
            {
                win.ReLayoutInteractiveVisual3D();
            }
        }

        /// <summary>
        /// Distance Between Models on Z axis
        /// </summary>
        public double ZDistanceBetweenModels
        {
            get
            {
                return (double)this.GetValue(ZDistanceBetweenModelsProperty);
            }
            set
            {
                this.SetValue(ZDistanceBetweenModelsProperty, value);
            }
        }
        #endregion

        #region MidModelDistanceProperty
        public static readonly DependencyProperty MidModelDistanceProperty = DependencyProperty.Register(
            "MidModelDistance", typeof(double), typeof(UICoverFlow),
            new FrameworkPropertyMetadata(1.5, MidModelDistancePropertyChangedCallback));

        private static void MidModelDistancePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            UICoverFlow win = sender as UICoverFlow;
            if (win != null)
            {
                win.ReLayoutInteractiveVisual3D();
            }
        }

        /// <summary>
        /// Distance between current selected Model and other models
        /// </summary>
        public double MidModelDistance
        {
            get
            {
                return (double)this.GetValue(MidModelDistanceProperty);
            }
            set
            {
                this.SetValue(MidModelDistanceProperty, value);
            }
        }
        #endregion



        public string GameTitle
        {
            get { return (string)GetValue(GameTitleProperty); }
            set { SetValue(GameTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GameTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GameTitleProperty =
            DependencyProperty.Register("GameTitle", typeof(string), typeof(UICoverFlow), new UIPropertyMetadata(null));


        public string GameDescription
        {
            get { return (string)GetValue(GameDescriptionProperty); }
            set { SetValue(GameDescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GameDescription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GameDescriptionProperty =
            DependencyProperty.Register("GameDescription", typeof(string), typeof(UICoverFlow), new UIPropertyMetadata(null));



        public string GameModeString
        {
            get { return (string)GetValue(GameModeProperty); }
            set { SetValue(GameModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GameMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GameModeProperty =
            DependencyProperty.Register("GameModeString", typeof(string), typeof(UICoverFlow), new UIPropertyMetadata(null));



        public string ROMModeString
        {
            get { return (string)GetValue(ROMModeStringProperty); }
            set { SetValue(ROMModeStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ROMModeString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ROMModeStringProperty =
            DependencyProperty.Register("ROMModeString", typeof(string), typeof(UICoverFlow), new UIPropertyMetadata("P-ROM"));

        public string LoadStatusString
        {
            get { return (string)GetValue(LoadStatusStringProperty); }
            set { SetValue(LoadStatusStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoadStatusString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadStatusStringProperty =
            DependencyProperty.Register("LoadStatusString", typeof(string), typeof(UICoverFlow), new UIPropertyMetadata("Waiting"));


        #endregion

        #region Properties

        public List<Games.GameInfo> GameLibrary 
        {
            get { return gameLibrary; }
            set { gameLibrary = value; }
        }
        public bool IsChanged { get; set; }
        #endregion

        #region Mouse API Interop
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        struct INPUT
        {
            public int type;
            public MOUSEINPUT mi;
        }

        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        class INPUTTYPE
        {
            public const int MOUSE = 0;
            public const int KEYBOARD = 1;
            public const int HARDWARE = 2;
        }

        class MOUSEEVENTF
        {
            public const int MOVE = 0x0001; /* mouse move */
            public const int LEFTDOWN = 0x0002; /* left button down */
            public const int LEFTUP = 0x0004; /* left button up */
            public const int RIGHTDOWN = 0x0008; /* right button down */
            public const int RIGHTUP = 0x0010; /* right button up */
            public const int MIDDLEDOWN = 0x0020; /* middle button down */
            public const int MIDDLEUP = 0x0040; /* middle button up */
            public const int XDOWN = 0x0080; /* x button down */
            public const int XUP = 0x0100; /* x button down */
            public const int WHEEL = 0x0800; /* wheel button rolled */
            public const int VIRTUALDESK = 0x4000; /* map to entire virtual desktop */
            public const int ABSOLUTE = 0x8000; /* absolute move */
        }
        #endregion

        private void Initialize_UserComponent()
        {
            Load_GameLibrary();

            // Initialize cover flow outlook
            ModelAngle = 80;
            XDistanceBetweenModels = 0.4;
            ZDistanceBetweenModels = 1.8;
            MidModelDistance = 2;
        }

        private void Load_GameLibrary()
        {
            gamePath = AppDomain.CurrentDomain.BaseDirectory + "Games\\";
            gameLibrary = Utilities.ReadFromXML<List<Games.GameInfo>>(gamePath + "gamelist.xml", true);
            IsChanged = true;
        }

        public void Save_GameLibrary()
        {
            if (MessageBox.Show("Do you want to save this game list for future treatment?", "Hint", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Utilities.SaveToXML<List<Games.GameInfo>>(gameLibrary, gamePath + "gamelist.xml");
        }

        private void Load_GameList()
        {
            gameList.Clear();
            foreach (Games.GameInfo game in gameLibrary)
            {
                if (game.IsInUse)
                    gameList.Add(game);
            }
        }

        public void Refresh_GameList()
        {
            if (IsChanged)
            {
                Load_GameList();
                this.LoadImageToViewport3D(this.GetUserImages());
            }
        }

        public void Load_GameProtocol(MainApp app, IntelliSerialPort port)
        {
            sp = port;
            mainApp = app;

            systemSettings = mainApp.IntelliProtocol.System;
            generalSettings = mainApp.IntelliProtocol.General;
            gameProtocol = mainApp.IntelliProtocol.Game;
            Refresh_GameList();
            Change_CurrentGame(0);

            gameControl = new GameControl();

            
        }

        // Load images to Viewport3D
        private void LoadImageToViewport3D(List<string> images)
        {
            if (images == null)
            {
                return;
            }

            for (int i = viewport3D.Children.Count - 1; i > 0; i--)
            {
                viewport3D.Children.RemoveAt(i);
            }
            //this.viewport3D.Children.Clear();
            for (int i = 0; i < images.Count; i++)
            {
                string imageFile = images[i];
                InteractiveVisual3D iv3d = this.CreateInteractiveVisual3D(imageFile, i);
                if (this.viewport3D.Children.Count <= i + 1)
                    this.viewport3D.Children.Add(iv3d);
                else
                    this.viewport3D.Children[i + 1] = iv3d;                    
            }

            this.ReLayoutInteractiveVisual3D();

            IsChanged = false;
        }

        // Load images
        private List<string> GetUserImages()
        {
            List<string> images = new List<string>();

            foreach (Games.GameInfo game in gameList)
                images.Add(gamePath + game.Name + @"\" + game.Preview);

            return images;
        }


        // Create visual from image path
        /// <param name="imageFile">image path</param>
        /// <returns>Create Visual</returns>
        private Visual CreateVisual(string imageFile, int index)
        {
            BitmapImage bmp = null;

            try
            {
                bmp = new BitmapImage(new Uri(imageFile));
            }
            catch
            {
            }

            Image img = new Image();
            img.Width = 50;
            img.Source = bmp;

            Border outBorder = new Border();
            outBorder.BorderBrush = Brushes.White;
            outBorder.BorderThickness = new Thickness(0.5);
            outBorder.Child = img;

            outBorder.MouseDown  += delegate(object sender, MouseButtonEventArgs e)
            {
                if (this.CurrentMidIndex == index)          
                    Game_Selected();         
                else
                    Change_CurrentGame(index);
                //this.CurrentMidIndex = index;
                e.Handled = true;
            };
            return outBorder;

        }


        // Create Geometry 3D 
        private Geometry3D CreateGeometry3D()
        {
            MeshGeometry3D geometry = new MeshGeometry3D();

            geometry.Positions = new Point3DCollection();
            geometry.Positions.Add(new Point3D(-1, 1, 0));
            geometry.Positions.Add(new Point3D(-1, -1, 0));
            geometry.Positions.Add(new Point3D(1, -1, 0));
            geometry.Positions.Add(new Point3D(1, 1, 0));

            geometry.TriangleIndices = new Int32Collection();
            geometry.TriangleIndices.Add(0);
            geometry.TriangleIndices.Add(1);
            geometry.TriangleIndices.Add(2);
            geometry.TriangleIndices.Add(0);
            geometry.TriangleIndices.Add(2);
            geometry.TriangleIndices.Add(3);

            geometry.TextureCoordinates = new PointCollection();
            geometry.TextureCoordinates.Add(new Point(0, 0));
            geometry.TextureCoordinates.Add(new Point(0, 1));
            geometry.TextureCoordinates.Add(new Point(1, 1));
            geometry.TextureCoordinates.Add(new Point(1, 0));

            return geometry;
        }

        // Create interactive visual3D from image path
        /// <param name="imageFile"></param>
        /// <returns></returns>
        private InteractiveVisual3D CreateInteractiveVisual3D(string imageFile, int index)
        {
            InteractiveVisual3D iv3d = new InteractiveVisual3D();
            iv3d.Visual = this.CreateVisual(imageFile, index);
            iv3d.Geometry = this.CreateGeometry3D();
            iv3d.Transform = this.CreateEmptyTransform3DGroup();

            return iv3d;
        }

        // Create empty transform3DGroup 
        private Transform3DGroup CreateEmptyTransform3DGroup()
        {
            Transform3DGroup group = new Transform3DGroup();
            group.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0)));
            group.Children.Add(new TranslateTransform3D(new Vector3D()));
            group.Children.Add(new ScaleTransform3D(1.5, 1.5, 1.5));

            return group;
        }

        // Transformation on index change in list
        /// <param name="index">index in list</param>
        /// <param name="midIndex">selected index</param>
        private void GetTransformOfInteractiveVisual3D(int index, double midIndex, out double angle, out double offsetX, out double offsetZ)
        {
            double disToMidIndex = index - midIndex;


            //rotate images
            angle = 0;
            if (disToMidIndex < 0)
            {
                angle = this.ModelAngle;//rotate N on left
            }
            else if (disToMidIndex > 0)
            {
                angle = (-this.ModelAngle);//rotate -N on right
            }



            //shift/extension on X axis
            offsetX = 0;//no move for selected (middle) image
            if (Math.Abs(disToMidIndex) <= 1)
            {
                offsetX = disToMidIndex * this.MidModelDistance;
            }
            else if (disToMidIndex != 0)
            {
                offsetX = disToMidIndex * this.XDistanceBetweenModels + (disToMidIndex > 0 ? this.MidModelDistance : -this.MidModelDistance);
            }


            //Move unselected images gradually along negative direction on Z axis -> highlight selected images in middle
            offsetZ = Math.Abs(disToMidIndex) * -this.ZDistanceBetweenModels;

        }

       // Relayout InteractiveVisual3D
        private void ReLayoutInteractiveVisual3D()
        {
            int j = 0;
            for (int i = 0; i < this.viewport3D.Children.Count; i++)
            {
                InteractiveVisual3D iv3d = this.viewport3D.Children[i] as InteractiveVisual3D;
                if (iv3d != null)
                {
                    double angle = 0;
                    double offsetX = 0;
                    double offsetZ = 0;
                    this.GetTransformOfInteractiveVisual3D(j++, this.CurrentMidIndex, out angle, out offsetX, out offsetZ);


                    NameScope.SetNameScope(this, new NameScope());
                    this.RegisterName("iv3d", iv3d);
                    Duration time = new Duration(TimeSpan.FromSeconds(0.3));

                    DoubleAnimation angleAnimation = new DoubleAnimation(angle, time);
                    DoubleAnimation xAnimation = new DoubleAnimation(offsetX, time);
                    DoubleAnimation zAnimation = new DoubleAnimation(offsetZ, time);

                    Storyboard story = new Storyboard();
                    story.Children.Add(angleAnimation);
                    story.Children.Add(xAnimation);
                    story.Children.Add(zAnimation);

                    Storyboard.SetTargetName(angleAnimation, "iv3d");
                    Storyboard.SetTargetName(xAnimation, "iv3d");
                    Storyboard.SetTargetName(zAnimation, "iv3d");

                    Storyboard.SetTargetProperty(
                        angleAnimation,
                        new PropertyPath("(ModelVisual3D.Transform).(Transform3DGroup.Children)[0].(RotateTransform3D.Rotation).(AxisAngleRotation3D.Angle)"));

                    Storyboard.SetTargetProperty(
                        xAnimation,
                        new PropertyPath("(ModelVisual3D.Transform).(Transform3DGroup.Children)[1].(TranslateTransform3D.OffsetX)"));
                    Storyboard.SetTargetProperty(
                        zAnimation,
                        new PropertyPath("(ModelVisual3D.Transform).(Transform3DGroup.Children)[1].(TranslateTransform3D.OffsetZ)"));

                    story.Begin(this);

                }
            }
        }

        private void Change_CurrentGame(int gameIndex)
        {
            Globals.Sound.pageSound.Play();
            this.CurrentMidIndex = gameIndex;
            Games.GameInfo currentGame = gameList[gameIndex];
            this.GameTitle = currentGame.Name;
            this.GameDescription = currentGame.Description;
            if (currentGame.GameMode == GameMode.Assistive)
                this.GameModeString = "A";  // Assistive
            else if(currentGame.GameMode == GameMode.Resistive)
                this.GameModeString = "R";  // Resistive

            this.DefaultROMMode();         // Set Passive ROM for game control _By Yupeng Mar.31.2011

        }

        private void Game_Added()
        {
            switch (selectedGame.ControlMode)
            {
                case ControlMode.Position:
                    rehabGame.FlexMax = generalSettings.GameFlexionMax;
                    rehabGame.ExtMax = generalSettings.GameExtensionMax;

                    switch (selectedGame.ControlDirection)
                    {
                        case Direction.X:
                            rehabGame.CtrlPosMax = selectedGame.Width;
                            break;
                        case Direction.Y:
                            rehabGame.CtrlPosMax = selectedGame.Height;
                            break;
                        default:
                            break;
                    }
                    break;
                case ControlMode.Velocity:
                    break;
                case ControlMode.Torque:
                    break;
                default:
                    break;
            }

            // game control panel delegates
            mainApp.uiGamePanel.Game_Settings = new UIGameCtrlPanel.DelegateGameSettings(Show_GameSettings);
            mainApp.uiGamePanel.Game_Ended = new UIGameCtrlPanel.DelegateGameEnded(ExitGame);

            mainApp.gridGame.Visibility = Visibility.Visible;
            mainApp.gridMain.Visibility = Visibility.Collapsed;
            mainApp.uiGamePanel.Visibility = Visibility.Visible;

            rehabGame.GameEventNotified = new RehabGameBase.GameEvent(GameEvent_Notified);
            sp.UpdateData += new IntelliSerialPort.DelegateUpdateData(UpdateGame);  // Add method to update game delegate
            mainApp.gridGame.Children.Add(rehabGame);
        }

        private void Game_Selected()
        {
            // Select game
            int currentGameIndex = (int)this.CurrentMidIndex;
            selectedGame = gameList[currentGameIndex];
            
            switch (selectedGame.Category)
            {
                case GameCategory.Flash:
                    rehabGame = new UIFlash(gamePath + selectedGame.Name + "\\" + selectedGame.Source) as RehabGameBase;
                    rehabGame.isCtrlInversed = false;
                    Game_Added();      
                    StartGame();
                    //MessageBox.Show("Flash games have been temporarily disabled");
                    break;

                case GameCategory.TV3D:
                    break;

                case GameCategory.WPF:
                    // WPF games  
                    mainApp.gridGame.Visibility = Visibility.Visible;

                    Assembly gameAssembly = Assembly.LoadFrom(gamePath + selectedGame.Name + @"\" + selectedGame.Source);  // Load game assembly
                    Type typeCurrentGame = gameAssembly.GetType(selectedGame.ClassName);
                    rehabGame = Activator.CreateInstance(typeCurrentGame) as RehabGameBase;
                    rehabGame.isCtrlInversed = true;
                    Game_Added();
                    break;

                case GameCategory.External:
                    // External games

                    ProcessStartInfo gameProcessInfo = new ProcessStartInfo(selectedGame.Source);
                    gameProcessInfo.WorkingDirectory = gamePath + selectedGame.Name + @"\";
                    Process processGame = Process.Start(gameProcessInfo);
                    sp.UpdateData += new IntelliSerialPort.DelegateUpdateData(ExternalGame_Update);  // Add method to update game delegate

                    mouseInput = new INPUT();
                    mouseInput.type = INPUTTYPE.MOUSE;
                    mouseInput.mi.dwFlags = MOUSEEVENTF.MOVE;

                    StartGame();
                    processGame.WaitForExit();
                    ExternalGame_Ended();
                    processGame.Close();

                    break;

                default:
                    break;
            }
            

        }

        #region Game Handlers

        private void GameEvent_Notified(RehabGameBase.GamePlayMode playMode)
        {
            switch (playMode)
            {
                case RehabGameBase.GamePlayMode.Exit:
                    break;
                case RehabGameBase.GamePlayMode.Instruction:
                    break;
                case RehabGameBase.GamePlayMode.Level:
                    gameControl.LevelSelector.NewLevel = rehabGame.Level;
                    break;
                case RehabGameBase.GamePlayMode.Over:
                    break;
                case RehabGameBase.GamePlayMode.Paused:
                    break;
                case RehabGameBase.GamePlayMode.Playing:
                    break;
                case RehabGameBase.GamePlayMode.Start:
                    StartGame();
                    break;

                case RehabGameBase.GamePlayMode.Target:
                    Debug.WriteLine("Target Entered");
                    if (sp.IsConnected)
                    {
                        newTarget = rehabGame.Target;
                        sp.WriteCmd($"PS{newTarget}");
                    }
                    break;

                case RehabGameBase.GamePlayMode.TargetRequest:
                    Debug.WriteLine("Target Requested");
                    gameControl.TargetCreator.CreateTarget(generalSettings.GameExtensionMax, generalSettings.GameFlexionMax);
                    rehabGame.CreateTarget(gameControl.TargetCreator.NewTarget);  // target in position
                    if (gameControl.LevelSelector.CheckGameLevel(rehabGame.TargetReachedCount)) rehabGame.SetLevel(gameControl.LevelSelector.NewLevel);
                    break;

                case RehabGameBase.GamePlayMode.TargetReady:
                    Debug.WriteLine("Target Ready");
                    if (sp.IsConnected)
                    {
                        newTarget = -gameControl.TargetCreator.NewTarget;
                        sp.WriteCmd($"PS{newTarget}");                        
                    }
                    break;

                case RehabGameBase.GamePlayMode.Title:
                    break;

                default:
                    break;
            }
        }

        private void UpdateGame(IntelliSerialPort.AnkleData newData)
        {
            // Update Game
            rehabGame.UpdateUI(newData.anklePos);
            // Update data display
            this.Dispatcher.Invoke(new Action(delegate
            {
                mainApp.uiGamePanel.JointPosition = newData.anklePos;
                int newAssistLevel = (int) (newData.ankleAm * 100);
                mainApp.uiGamePanel.AssistLevel = (newAssistLevel > 0)  ? newAssistLevel : -newAssistLevel;
                mainApp.uiGamePanel.TargetPosition = newData.targetPos;
                mainApp.uiGamePanel.TorqueValue = newData.ankleTorque;//Liang
                //if (newTarget != (int)-newData.targetPos) sp.WriteCmd($"PS{newTarget}");
            }));
        }

        public void ResumeGame()
        {
            if (rehabGame != null) rehabGame.ResumeGame();
        }

        private void ExitGame()
        {
            if (sp.IsConnected)
            {
                sp.IsUpdating = false;
                if (systemSettings.IsSavingData) Stop_SaveData();
                sp.UpdateData -= UpdateGame;
                sp.WriteCmd("BK");  // Release motor when game ended // Changed from RL to BK, Michael 
            }

            mainApp.uiGamePanel.Game_Settings -= Show_GameSettings;
            mainApp.uiGamePanel.Game_Ended -= ExitGame;

            rehabGame.ExitGame();
            mainApp.gridMain.Visibility = Visibility.Visible;
            mainApp.gridGame.Visibility = Visibility.Collapsed;
            mainApp.gridGame.Children.Clear();
        }

        private void StartGame()
        {
            mainApp.uiGamePanel.stopwatchGame.Start();  // Start timer

            if (sp.IsConnected)
            {
                if (selectedGame.GameMode == GameMode.Assistive) // Assistive
                {
                    sp.WriteCmd("AG");
                    sp.WriteCmd($"ML{ gameProtocol.AssistiveMode.Level}");
                    sp.WriteCmd($"VL{gameProtocol.AssistiveMode.Velocity}");
                    sp.WriteCmd($"AD{gameProtocol.AssistiveMode.DelayTime}");
                    

                    gameControl.LevelSelector.NewLevel = 0;
                }
                else if (selectedGame.GameMode == GameMode.Resistive) // Resistive
                {
                    sp.WriteCmd("RG");
                    if (gameProtocol.ResistiveMode.IsNoLoading)
                        sp.WriteCmd("BK"); // No loading => backdrivable
                    else
                    {
                        sp.WriteCmd("FC"); // Friction control
                        sp.WriteCmd($"FR{gameProtocol.ResistiveMode.Resistance}"); // non-directional Resistance
                        //sp.WriteCmd($"FP{gameProtocol.ResistiveMode.FlexionResistance}"); // Set Dorsi/flexion Resistance            
                        //sp.WriteCmd($"FD{gameProtocol.ResistiveMode.ExtensionResistance}"); // Set plantar/extension Resistance
                    }
                }

                if (systemSettings.IsSavingData) Start_SaveData(selectedGame.Name);
                sp.IsUpdating = true;
            }
        }

        private void Show_GameSettings()
        {
            if (rehabGame != null) rehabGame.PauseGame();

            UIProtocol.LayoutMode layoutMode = UIProtocol.LayoutMode.AssistiveMode;

            switch (selectedGame.GameMode)
	        {
		        case GameMode.AssistAndResist:
                    layoutMode = UIProtocol.LayoutMode.AssistAndResist;
                    break;

                case GameMode.Assistive:
                    layoutMode = UIProtocol.LayoutMode.AssistiveMode;
                    break;

                case GameMode.Resistive:
                    layoutMode = UIProtocol.LayoutMode.ResistiveMode;
                    break;
	        }

            mainApp.uiProtocol.Reset_Layout(layoutMode);
            mainApp.uiProtocol.Load_Protocol(mainApp, sp);
            mainApp.uiProtocol.SlideIn(true);
        }

        #endregion


        #region External
        private void ExternalGame_Update(IntelliSerialPort.AnkleData newData)
        {
            // Update External Game
            //int newPos = Utilities.Pos2Screen(newData.anklePos, generalSettings.DorsiPos, generalSettings.PlantarPos, generalSettings.ExtraRange, ctrlPosMax);
            int newPos = (int)(newData.anklePos * gameProtocol.ResistiveMode.ScalingFactor);
            mouseInput.mi.dx = newPos - prevPos;
            SendInput(1, ref mouseInput, Marshal.SizeOf(mouseInput));
            prevPos = newPos;
        }

        private void ExternalGame_Ended()
        {
            if (sp.IsConnected)
            {
                sp.IsUpdating = false;
                if (systemSettings.IsSavingData) Stop_SaveData();
                sp.UpdateData -= ExternalGame_Update;  // remove delegate
                sp.WriteCmd("BK");  // Release motor when game ended
            }

        }

        #endregion


        private void Start_SaveData(string gameName)
        {
            sp.Start_SaveData("Game_" + gameName + "_");
        }

        private void Stop_SaveData()
        {
            sp.Stop_SaveData();
        }


        public void SwitchROMMode()
        {
            if (ROMModeString == "P-ROM")
            {
                if (generalSettings.ActiveFlexionMax - generalSettings.ActiveExtensionMax < 5)
                { ;}
                //MessageBox.Show( "Measured AROM is too small. /n Can not switch to AROM setting." );
                else
                {
                    generalSettings.GameFlexionMax = generalSettings.ActiveFlexionMax;
                    generalSettings.GameExtensionMax = generalSettings.ActiveExtensionMax;
                    ROMModeString = "M-ROM"; // AROM
                }
            }
            else
            {
                generalSettings.GameFlexionMax = generalSettings.FlexionMax;
                generalSettings.GameExtensionMax = generalSettings.ExtensionMax;
                ROMModeString = "P-ROM"; // PROM
            }
        }
        public void DefaultROMMode()   // Add by Yupeng Mar.31.2011
        {
            if (GameModeString == "A")
            {
                generalSettings.GameFlexionMax = generalSettings.FlexionMax;
                generalSettings.GameExtensionMax = generalSettings.ExtensionMax;
                ROMModeString = "P-ROM"; // PROM
            }
            else if (GameModeString == "R")
            {
                if (generalSettings.ActiveFlexionMax - generalSettings.ActiveExtensionMax < 5)
                {
                    generalSettings.GameFlexionMax = generalSettings.FlexionMax;
                    generalSettings.GameExtensionMax = generalSettings.ExtensionMax;
                    ROMModeString = "P-ROM"; // PROM;
                }
                else
                {
                    generalSettings.GameFlexionMax = generalSettings.ActiveFlexionMax;
                    generalSettings.GameExtensionMax = generalSettings.ActiveExtensionMax;
                    ROMModeString = "M-ROM"; // AROM
                }
            }
        }
    }
}
