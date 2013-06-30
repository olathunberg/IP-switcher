﻿using Deucalion.IP_Switcher.Classes;
using System.Windows;

namespace Deucalion.IP_Switcher
{
    /// <summary>
    /// Interaction logic for LocationDetailView.xaml
    /// </summary>
    public partial class LocationDetailView : Window
    {
        public LocationDetailView(Location location, bool IsManualSettings = false)
        {
            InitializeComponent();

            DataContext = location;

            if (IsManualSettings)
            {
                DescriptionLabel.Visibility = System.Windows.Visibility.Hidden;
                DescriptionTextBox.Visibility = System.Windows.Visibility.Hidden;
                btnAbort.Content = "Ångra";
                btnSave.Content = "Använd";
            }
        }

        public LocationDetailView()
        {

        }

        private void AddIp_Click(object sender, RoutedEventArgs e)
        {
            ((Location)DataContext).IPList.Add(new IPDefinition());
        }

        private void RemoveIp_Click(object sender, RoutedEventArgs e)
        {
            if (lvIp.SelectedItem != null)
                ((Location)DataContext).IPList.Remove((IPDefinition)lvIp.SelectedItem);
        }

        private void AddGateway_Click(object sender, RoutedEventArgs e)
        {
            ((Location)DataContext).Gateways.Add(new IPv4Address());
        }

        private void RemoveGateway_Click(object sender, RoutedEventArgs e)
        {
            if (lvGateway.SelectedItem != null)
                ((Location)DataContext).Gateways.Remove((IPv4Address)lvGateway.SelectedItem);
        }

        private void RemoveDns_Click(object sender, RoutedEventArgs e)
        {
            if (lvDns.SelectedItem != null)
                ((Location)DataContext).DNS.Remove((IPv4Address)lvDns.SelectedItem);
        }

        private void AddDns_Click(object sender, RoutedEventArgs e)
        {
            ((Location)DataContext).DNS.Add(new IPv4Address());
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
