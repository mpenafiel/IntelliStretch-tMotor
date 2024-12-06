using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media.Animation;

namespace IntelliStretch.UI
{
    /// <summary>
    /// Interaction logic for UIProfileLoader.xaml
    /// </summary>
    public partial class UIProfileLoader : UserControl
    {
        public UIProfileLoader()
        {
            InitializeComponent();
        }

        #region Variables
        MainApp mainApp;
        Protocols.IntelliProtocol intelliProtocol;
        Protocols.Joint sysJoint;
        List<UserProfile> userProfiles;
        UserProfile selectedProfile;
        string appPath, jointName;
        Storyboard sbScrollIn, sbScrollOut;

        IntelliSerialPort sp;

        #endregion

        public void Load_ProfileLoader(MainApp app, Protocols.Joint joint, IntelliSerialPort port)
        {
            // Add references
            mainApp = app;
            intelliProtocol = app.IntelliProtocol;
            sysJoint = joint;

            sp = port;
        }
       
        public void Reset_UI()
        {
            this.Visibility = Visibility.Visible;
            bdrPopup.Visibility = Visibility.Visible;
            gridJoints.Visibility = Visibility.Collapsed;
            gridJointSide.Visibility = Visibility.Collapsed;
            stackElbow.Visibility = Visibility.Collapsed;
            stackWrist.Visibility = Visibility.Collapsed;
            chkSaveJoint.IsChecked = false;
            if (sbScrollIn != null) sbScrollIn.Begin();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            appPath = AppDomain.CurrentDomain.BaseDirectory;
            userProfiles = new List<UserProfile>();

            Read_ProfileList();
            lstProfiles.ItemsSource = userProfiles;  // list data binding
            if (userProfiles.Count > 0) lstProfiles.SelectedIndex = 0;

            sbScrollIn = TryFindResource("sbScrollIn") as Storyboard;
            sbScrollOut = TryFindResource("sbScrollOut") as Storyboard;
        }

        private void btnCreateNew_Click(object sender, RoutedEventArgs e)
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtTitle.Text = "Create New User:";
            gridProfileList.Visibility = Visibility.Collapsed;
            gridNewInput.Visibility = Visibility.Visible;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            selectedProfile = lstProfiles.SelectedItem as UserProfile;
            selectedProfile.LastLoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:tt");
            Save_ProfileList(userProfiles);
            sbScrollOut.Begin();
        }

        private void btnNewProfile_Click(object sender, RoutedEventArgs e)
        {
            Update_UserList();
            gridNewInput.Visibility = Visibility.Collapsed;
            gridProfileList.Visibility = Visibility.Visible;
            txtTitle.Text = "Select User:";
        }

        private void btnCancelNew_Click(object sender, RoutedEventArgs e)
        {
            gridNewInput.Visibility = Visibility.Collapsed;
            gridProfileList.Visibility = Visibility.Visible;
            txtTitle.Text = "Select User:";
        }

        private void Read_ProfileList()
        {
            lstProfiles.Items.Clear(); // clear list 
            string userFile = appPath + @"Profiles\UserProfiles.xml";

            if (!File.Exists(userFile))  // If user profiles do not exsit
                btnSelect.IsEnabled = false;
            else
            {
                try
                {
                    userProfiles = Validate_ProfileList(Utilities.ReadFromXML<List<UserProfile>>(userFile, true));  // Read user profiles from file
                    if (userProfiles == null)
                    {
                        btnSelect.IsEnabled = false;
                        return;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("User profile file has been corrupted!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    btnSelect.IsEnabled = false;
                    return;
                }

                btnSelect.IsEnabled = true;
            }

        }

        private void Save_ProfileList(List<UserProfile> userList)
        {
            string userFile = appPath + @"Profiles\UserProfiles.xml";
            Utilities.SaveToXML<List<UserProfile>>(userList, userFile);
        }

        private List<UserProfile> Validate_ProfileList(List<UserProfile> userList)
        {
            List<UserProfile> removeList = new List<UserProfile>();

            foreach (UserProfile profile in userList)
            {
                // Check user folder availability
                string userDir = appPath + @"Profiles\" + profile.LastName + "_" + profile.FirstName;
                if (!Directory.Exists(userDir)) removeList.Add(profile);
            }

            if (removeList.Count > 0)
            {
                foreach (UserProfile profile in removeList)
                {                    
                    userList.Remove(profile);
                }
                Save_ProfileList(userList);
            }
            return userList;
        }

        private void Update_UserList()
        {
            if (txtLastName.Text != "" || txtFirstName.Text != "")
            {
                UserProfile newProfile = new UserProfile(txtLastName.Text, txtFirstName.Text, sysJoint, DateTime.Now.ToString("yyyy-MM-dd HH:mm:tt"));
                string newDir = appPath + @"Profiles\" + newProfile.LastName + "_" + newProfile.FirstName;
                if (Directory.Exists(newDir))
                    MessageBox.Show("This profile has already been created!  Please try another name.");
                else
                {
                    Directory.CreateDirectory(newDir);  // Create user folder
                    userProfiles.Add(newProfile);  // Update user list
                    lstProfiles.Items.Refresh();
                    lstProfiles.SelectedIndex = userProfiles.Count - 1;

                    Save_ProfileList(userProfiles);
                    btnSelect.IsEnabled = true;
                }
            }
        }

        private void Load_SelectedProfile()
        {
            // Load settings
            string userDir = appPath + @"Profiles\" + selectedProfile.LastName + "_" + selectedProfile.FirstName + jointName;
            string defaultFile = userDir + "Default.xml";
            if (!File.Exists(defaultFile))
            {
                if (!Directory.Exists(userDir)) Directory.CreateDirectory(userDir);
                File.Copy(appPath + @"Profiles\Default.xml", defaultFile);
            }
            mainApp.CurrentUserDir = userDir;
            mainApp.CurrentDateDir = DateTime.Today.ToString("yyyy-MM-dd") + @"\";
            mainApp.DataFilePrefix = selectedProfile.LastName + "_" + selectedProfile.FirstName;
            intelliProtocol = Utilities.ReadFromXML<Protocols.IntelliProtocol>(defaultFile, true);  // Load protocol
        }

        private void aniScrollOut_Completed(object sender, EventArgs e)
        {
            bdrPopup.Visibility = Visibility.Collapsed;

            // Check joint
            switch (selectedProfile.Joint)
            {
                case Protocols.Joint.Ankle:
                    Joint_Selected(Protocols.Joint.Ankle, Protocols.JointSide.None);  // start directly if it's ankle version
                    break;
                    
                case Protocols.Joint.Knee:
                    Joint_Selected(Protocols.Joint.Knee, Protocols.JointSide.None);  // start directly if it's knee version
                    break;

                case Protocols.Joint.Elbow:
                    stackElbow.Visibility = Visibility.Visible;
                    stackWrist.Visibility = Visibility.Collapsed;
                    gridJointSide.Visibility = Visibility.Visible;
                    break;

                case Protocols.Joint.Wrist:
                    stackWrist.Visibility = Visibility.Visible;
                    stackElbow.Visibility = Visibility.Collapsed;
                    gridJointSide.Visibility = Visibility.Visible;
                    break;

                case Protocols.Joint.Arm:
                    btnAnkle.Visibility = Visibility.Collapsed;  // Collapse ankle button
                    btnKnee.Visibility = Visibility.Collapsed;  // Collapse knee button
                    gridJoints.Visibility = Visibility.Visible;  // Show joint selection screen
                    break;

                case Protocols.Joint.All:
                    btnAnkle.Visibility = Visibility.Visible;  // Show ankle button
                    btnKnee.Visibility = Visibility.Visible;  // knee button
                    btnElbow.Visibility = Visibility.Visible;  // Show elbow button
                    btnWrist.Visibility = Visibility.Visible; // wrist button
                    gridJoints.Visibility = Visibility.Visible;  // Show joint selection screen
                    break;

                default:
                    break;
            }
        }

        private void btnJoints_Click(object sender, RoutedEventArgs e)
        {
            ResourceDictionary rdJointTheme = new ResourceDictionary();

            switch ((e.Source as UserControls.ImageButton).Name)
            {
                case "btnAnkle":
                    Joint_Selected(Protocols.Joint.Ankle, Protocols.JointSide.None);
                    break;

                case "btnKnee":
                    Joint_Selected(Protocols.Joint.Knee, Protocols.JointSide.None);
                    break;

                case "btnElbow":
                    gridJoints.Visibility = Visibility.Collapsed;
                    stackElbow.Visibility = Visibility.Visible;
                    stackWrist.Visibility = Visibility.Collapsed;
                    gridJointSide.Visibility = Visibility.Visible;
                    break;

                case "btnWrist":
                    gridJoints.Visibility = Visibility.Collapsed;
                    stackWrist.Visibility = Visibility.Visible;
                    stackElbow.Visibility = Visibility.Collapsed;
                    gridJointSide.Visibility = Visibility.Visible;
                    break;
            }

            if ((bool)chkSaveJoint.IsChecked) Save_ProfileList(userProfiles);
        }

        private void btnJointSide_Click(object sender, RoutedEventArgs e)
        {
            switch ((e.Source as UserControls.ImageButton).Name)
            {
                case "btnElbowLeft":
                    Joint_Selected(Protocols.Joint.Elbow, Protocols.JointSide.Left);
                    break;

                case "btnElbowRight":
                    Joint_Selected(Protocols.Joint.Elbow, Protocols.JointSide.Right);
                    break;

                case "btnWristLeft":
                    Joint_Selected(Protocols.Joint.Wrist, Protocols.JointSide.Left);
                    break;

                case "btnWristRight":
                    Joint_Selected(Protocols.Joint.Wrist, Protocols.JointSide.Right);
                    break;
            }
        }

        private void Joint_Selected(Protocols.Joint selectedJoint, Protocols.JointSide jointSide)
        {
            ResourceDictionary rdJointTheme = new ResourceDictionary();

            switch (selectedJoint)
            {
                case Protocols.Joint.Ankle:
                    rdJointTheme.Source = new Uri(@"pack://application:,,/Resources/AnkleTheme.xaml");
                    jointName = @"\Ankle\";
                    selectedProfile.Joint = Protocols.Joint.Ankle;
                    break;

                case Protocols.Joint.Knee:
                    rdJointTheme.Source = new Uri(@"pack://application:,,/Resources/KneeTheme.xaml");
                    jointName = @"\Knee\";
                    selectedProfile.Joint = Protocols.Joint.Knee;
                    break;

                case Protocols.Joint.Elbow:
                    // Select theme for left/right elbow
                    if (jointSide == Protocols.JointSide.Left)
                        rdJointTheme.Source = new Uri(@"pack://application:,,/Resources/ElbowLeftTheme.xaml");
                    else
                        rdJointTheme.Source = new Uri(@"pack://application:,,/Resources/ElbowRightTheme.xaml");
                    
                    jointName = @"\Elbow\";
                    selectedProfile.Joint = Protocols.Joint.Elbow;
                    break;

                case Protocols.Joint.Wrist:
                    // Select theme for left/right wrist

                    if (jointSide == Protocols.JointSide.Left)
                        rdJointTheme.Source = new Uri(@"pack://application:,,/Resources/WristLeftTheme.xaml");
                    else
                        rdJointTheme.Source = new Uri(@"pack://application:,,/Resources/WristRightTheme.xaml");

                    jointName = @"\Wrist\";
                    selectedProfile.Joint = Protocols.Joint.Wrist;
                    break;
            }

            if (rdJointTheme != null) mainApp.Resources.MergedDictionaries.Add(rdJointTheme);
            Load_SelectedProfile();
            intelliProtocol.General.Joint = selectedProfile.Joint;
            intelliProtocol.General.JointSide = jointSide;

            Start_MainApp();
        }

        private void Start_MainApp()
        {
            mainApp.IntelliProtocol = intelliProtocol;  // Apply changes
            this.Visibility = Visibility.Collapsed;
            mainApp.Begin_Preparation(selectedProfile);
        }

    }
}
