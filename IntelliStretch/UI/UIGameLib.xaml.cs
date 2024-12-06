using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using IntelliStretch.Games;
using System.Windows.Media.Animation;

namespace IntelliStretch.UI
{
    /// <summary>
    /// Interaction logic for UIGameLib.xaml
    /// </summary>
    public partial class UIGameLib : UserControl
    {
        public UIGameLib()
        {
            InitializeComponent();
        }

        List<GameInfo> gameLibrary, onlineGameLib;
        MainApp mainApp;
        Storyboard sbSlideIn, sbSlideOut;

        private void uiGameLib_Loaded(object sender, RoutedEventArgs e)
        {
            gameLibrary = mainApp.uiGames.GameLibrary;
            lstGames.ItemsSource = gameLibrary;  // list data binding

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
            GameInfo tempGame = gameLibrary[index1];
            gameLibrary[index1] = gameLibrary[index2];
            gameLibrary[index2] = tempGame;
            lstGames.Items.Refresh();
            mainApp.uiGames.IsChanged = true;
        }

        private void tabGameLib_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabGameLib.SelectedItem == tabOnlineGames)
            {
                onlineGameLib = Utilities.ReadFromXML<List<GameInfo>>("http://www.rehabtek.com/IntelliStretch/Games/gamelist.xml", false);
                if (lstOnlineGames.ItemsSource == null) lstOnlineGames.ItemsSource = onlineGameLib;  // list data binding
                lstOnlineGames.Items.Refresh();
            }
        }


        private void btnDeleteGame_Click(object sender, RoutedEventArgs e)
        {
            mainApp.uiGames.IsChanged = true;
            gameLibrary.Remove(lstGames.SelectedItem as GameInfo);
            lstGames.Items.Refresh();
        }

        private void chkInUse_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mainApp.uiGames.IsChanged = true;
        }

    }
}
