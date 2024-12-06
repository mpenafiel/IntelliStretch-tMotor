using System.Windows;
using System.Windows.Controls;

namespace IntelliStretch.UI
{
    /// <summary>
    /// Interaction logic for UIReport.xaml
    /// </summary>
    public partial class UIReport : UserControl
    {
        public UIReport()
        {
            InitializeComponent();
        }

        //UserProfile userProfile;

        public void Load_Report(UserProfile profile, Protocols.IntelliProtocol protocol)
        {
            rowsInfo.DataContext = profile;
            Protocols.GeneralSettings newGeneral = new Protocols.GeneralSettings();
            newGeneral.ExtensionMax = protocol.General.ExtensionMax;
            newGeneral.FlexionMax = protocol.General.FlexionMax;
            newGeneral.ActiveExtensionMax = protocol.General.ActiveExtensionMax;
            newGeneral.ActiveFlexionMax = protocol.General.ActiveFlexionMax;
            rowsROM.DataContext = newGeneral;

            chartPROM.Refresh();
            chartAROM.Refresh();
            //userProfile = profile;
            //docReport.DataContext = userProfile;
        }

        private void uiReport_Loaded(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < tbReport.RowGroups.Count; i++)
            //{
            //    TableRowGroup rowGroup = tbReport.RowGroups[i];
            //    for (int j = 0; j < rowGroup.Rows.Count; j++)
            //    {
            //        rowGroup.Rows[j].Background = (j % 2 == 0) ? new SolidColorBrush(Colors.AntiqueWhite) : new SolidColorBrush(Colors.White);
            //    }
            //}
        }
    }
}
