using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TTech.IP_Switcher.Features.IpSwitcher.Location;
using TTech.IP_Switcher.Features.IpSwitcher.LocationDetail.Resources;

namespace TTech.IP_Switcher.Features.IpSwitcher.LocationDetail
{
    /// <summary>
    /// Interaction logic for LocationDetailView.xaml
    /// </summary>
    public partial class LocationDetailView : Window
    {

        public LocationDetailView(dynamic parameters)
        {
            InitializeComponent();

            DataContext = parameters.Location;

            if (DataContext == null)
                DataContext = new Location.Location();
            if (parameters.IsManualSettings)
            {
                DescriptionLabel.Visibility = System.Windows.Visibility.Collapsed;
                DescriptionTextBox.Visibility = System.Windows.Visibility.Collapsed;
                btnAbort.Content = LocationDetailViewLoc.Cancel;
                btnSave.Content = LocationDetailViewLoc.Use;
            }
        }

        public LocationDetailView()
        {
            InitializeComponent();

            DataContext = new Location.Location();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "<Pending>")]
        protected void SelectCurrentItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            var item = (ListBoxItem)sender;
            item.IsSelected = true;
        }

        private void AddIp_Click(object sender, RoutedEventArgs e)
        {
            ((Location.Location)DataContext).IPList.Add(new IPDefinition());
        }

        private void RemoveIp_Click(object sender, RoutedEventArgs e)
        {
            if (lvIp.SelectedItem != null)
                ((Location.Location)DataContext).IPList.Remove((IPDefinition)lvIp.SelectedItem);
        }

        private void AddGateway_Click(object sender, RoutedEventArgs e)
        {
            ((Location.Location)DataContext).Gateways.Add(new IPv4Address());
        }

        private void RemoveGateway_Click(object sender, RoutedEventArgs e)
        {
            if (lvGateway.SelectedItem != null)
                ((Location.Location)DataContext).Gateways.Remove((IPv4Address)lvGateway.SelectedItem);
        }

        private void RemoveDns_Click(object sender, RoutedEventArgs e)
        {
            if (lvDns.SelectedItem != null)
                ((Location.Location)DataContext).DNS.Remove((IPv4Address)lvDns.SelectedItem);
        }

        private void AddDns_Click(object sender, RoutedEventArgs e)
        {
            ((Location.Location)DataContext).DNS.Add(new IPv4Address());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DescriptionTextBox.Focus();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
