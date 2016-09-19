using System.Windows.Controls;

namespace TTech.IP_Switcher.Features.WiFiManager
{
    /// <summary>
    /// Interaction logic for WiFiNetworksView.xaml
    /// </summary>
    public partial class WiFiNetworksView : UserControl
    {
        public WiFiNetworksView()
        {
            InitializeComponent();

            var mainViewModel = MainGrid.DataContext as WiFiNetworksViewModel;
            if (mainViewModel != null)
                mainViewModel.Owner = this;
        }
   }
}
