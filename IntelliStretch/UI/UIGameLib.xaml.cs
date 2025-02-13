using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using IntelliStretch.Games;
using System.Windows.Media.Animation;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Diagnostics;
using ScottPlot.TickGenerators;

namespace IntelliStretch.UI
{
    public partial class UIGameLib : System.Windows.Controls.UserControl
    {
        public UIGameLib()
        {
            InitializeComponent();
        }

        List<Games.GameInfo> gameLibrary, onlineGameLib;
        MainApp mainApp;
        Storyboard sbSlideIn, sbSlideOut;

        private void uiGameLib_Loaded(object sender, RoutedEventArgs e)
        {
            gameLibrary = mainApp.uiGames.GameLibrary;
            lstGames.ItemsSource = gameLibrary;  // list data binding
            if (gameLibrary.Count > 0) lstGames.SelectedIndex = 0;

            sbSlideIn = TryFindResource("sbSlideIn") as Storyboard;
            sbSlideOut = TryFindResource("sbSlideOut") as Storyboard;
        }

        public void Initialize_Gamelist(MainApp app)
        {
            mainApp = app;
        }

        public void SlideIn()
        {
            if (Globals.Sound.pageSound != null) Globals.Sound.pageSound.Play();
            this.Visibility = Visibility.Visible;
            if (sbSlideIn != null) sbSlideIn.Begin();
        }

        private void SlideOut()
        {
            if (Globals.Sound.pageSound != null) Globals.Sound.pageSound.Play();
            if (sbSlideOut != null) sbSlideOut.Begin();
        }

        private void aniSlideOut_Completed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void btnCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            mainApp.uiGames.Save_GameLibrary();
            mainApp.uiGames.Refresh_GameList();
            SlideOut();
        }

        private void btnItemUp_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = lstGames.SelectedIndex;
            if (currentIndex != 0)
            {
                Switch_ListItems(currentIndex, currentIndex - 1);
                lstGames.SelectedIndex = currentIndex - 1;
            }
        }

        private void btnItemDown_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = lstGames.SelectedIndex;
            if (currentIndex != lstGames.Items.Count - 1)
            {
                Switch_ListItems(currentIndex, currentIndex + 1);
                lstGames.SelectedIndex = currentIndex + 1;
            }
        }

        private void Switch_ListItems(int index1, int index2)
        {
            Games.GameInfo tempGame = gameLibrary[index1];
            gameLibrary[index1] = gameLibrary[index2];
            gameLibrary[index2] = tempGame;
            lstGames.Items.Refresh();
            mainApp.uiGames.IsChanged = true;
        }

        private void tabGameLib_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabGameLib.SelectedItem == tabOnlineGames)
            {
                onlineGameLib = Utilities.ReadFromXML<List<Games.GameInfo>>("http://www.rehabtek.com/IntelliStretch/Games/gamelist.xml", false);
                if (lstOnlineGames.ItemsSource == null) lstOnlineGames.ItemsSource = onlineGameLib;  // list data binding
                lstOnlineGames.Items.Refresh();
            }
        }

        private void btnAddGame_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder";

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedGamePath = folderDialog.SelectedPath;

                    int gameExists = Update_GameInfoList(selectedGamePath);

                    if (gameExists == 0)
                    {
                        // Get the base name of the path
                        string gameName = Path.GetFileName(selectedGamePath);

                        string appPath = AppDomain.CurrentDomain.BaseDirectory;
                        string gamesFolder = appPath + $@"Games\{gameName}";
                        CopyDirectory(selectedGamePath, gamesFolder);
                    }
                    else if ( gameExists == 1)
                    {
                        System.Windows.MessageBox.Show("The game you are trying to add is already in the library!", "Hint", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("The game you are trying to add has a corrupted or missing GameInfo file!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }


                }
                else
                {
                    System.Windows.MessageBox.Show("No folder selected.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

            private void btnDeleteGame_Click(object sender, RoutedEventArgs e)
        {
            mainApp.uiGames.IsChanged = true;
            gameLibrary.Remove(lstGames.SelectedItem as Games.GameInfo);
            lstGames.Items.Refresh();
        }

        private int Update_GameInfoList(string gamePath)
        {
            string infoFile = gamePath + @"\GameInfo.xml";
            int gameExists = 0;
            if (File.Exists(infoFile))
            {
                GameInfo gameInfo;
                gameInfo = Utilities.ReadFromXML<GameInfo>(infoFile, true);

                foreach (GameInfo game in gameLibrary)
                {
                    if (gameInfo.Name == game.Name) gameExists = 1;
                }

                if (gameExists == 0)
                {
                    gameLibrary.Add(gameInfo); // Update game info List

                    lstGames.Items.Refresh();
                    lstGames.SelectedIndex = gameLibrary.Count - 1;
                }
                return gameExists;
            }


            return -1;
        }

        static void CopyDirectory(string sourceDir, string destDir)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDir);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it. 
            Directory.CreateDirectory(destDir);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDir, file.Name);
                file.CopyTo(tempPath, true);
            }

            // Copy subdirectories and their contents to new location.
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDir, subdir.Name);
                CopyDirectory(subdir.FullName, tempPath);
            }
        }

        private void chkInUse_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mainApp.uiGames.IsChanged = true;
        }

    }
}
