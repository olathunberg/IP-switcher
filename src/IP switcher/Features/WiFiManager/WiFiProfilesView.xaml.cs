using System.Windows.Controls;

namespace Deucalion.IP_Switcher.Features.WiFiManager
{
    /// <summary>
    /// Interaction logic for WiFiProfilesViewModel.xaml
    /// </summary>
    public partial class WiFiProfilesView : UserControl
    {
        public WiFiProfilesView()
        {
            InitializeComponent();

            var mainViewModel = MainGrid.DataContext as WiFiProfilesViewModel;
            if (mainViewModel != null)
                mainViewModel.Owner = this;
        }
   }
}
