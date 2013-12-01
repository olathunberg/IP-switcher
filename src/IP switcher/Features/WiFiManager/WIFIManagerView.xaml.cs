using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Text.RegularExpressions;
using Wireless_Network_Manager.Classes;
using System.Management;
using NativeWifi;

namespace Wireless_Network_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WiFiManagerView : UserControl
    {
        HandleNetworks HandleNetworks = new HandleNetworks();
        WlanClient client = new WlanClient();

        string SSID = "";

        public WiFiManagerView()
        {
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1) System.Diagnostics.Process.GetCurrentProcess().Kill();
            InitializeComponent();
            UpdateNetworkListbox();
        }

        private void Formlaod(object sender, RoutedEventArgs e)
        {
            UpdateNetworkListbox();
        }

        private void SSIDSelect(object sender, SelectionChangedEventArgs e)
        {

            if (lbConsoleStream.SelectedItem != null)
            {
                string selecteditem = lbConsoleStream.SelectedItem.ToString();

                UpdateSecurityInfoField(HandleNetworks.ReadNetworkDetails(@selecteditem));
                btnDelete.IsEnabled = true;
            }
        }

        private void UpdateNetworkListbox()
        {

            lbConsoleStream.Items.Clear();

            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                // Retrieves XML configurations of existing profiles.
                // This can assist you in constructing your own XML configuration
                // (that is, it will give you an example to follow).

                foreach (Wlan.WlanProfileInfo profileInfo in wlanIface.GetProfiles())
                {
                    //string xml = wlanIface.GetProfileXml(profileInfo.profileName);                   
                    lbConsoleStream.Items.Add(profileInfo.profileName);
                }
            }

        }

        private void UpdateSecurityInfoField(string input)
        {

            txtHeaders.Text = "";
            txtData.Text = "";

            if (FindDataInStrString(input, "Type") == "Wireless LAN")
            {

                txtHeaders.Text += "Mode:\r\n";
                txtData.Text += FindDataInStrString(input, "Connection mode") + "\r\n";

                txtHeaders.Text += "Broadcast:\r\n";
                txtData.Text += FindDataInStrString(input, "Network broadcast") + "\r\n";

                txtHeaders.Text += "SSID:\r\n";
                SSID = FindDataInStrString(input, "SSID name");
                SSID = SSID.Remove(SSID.Length - 1, 1);
                SSID = SSID.Remove(0, 1);
                txtData.Text += SSID + "\r\n";

                txtHeaders.Text += "\r\n";
                txtData.Text += "\r\n";

                txtHeaders.Text += "Network type:\r\n";
                txtData.Text += FindDataInStrString(input, "Network type") + "\r\n";

                txtHeaders.Text += "Radio type:\r\n";
                txtData.Text += FindDataInStrString(input, "Radio type") + "\r\n";

                txtHeaders.Text += "\r\n";
                txtData.Text += "\r\n";

                txtHeaders.Text += "Authentication:\r\n";
                txtData.Text += FindDataInStrString(input, "Authentication") + "\r\n";

                txtHeaders.Text += "Cipher:\r\n";
                txtData.Text += FindDataInStrString(input, "Cipher") + "\r\n";

                txtHeaders.Text += "Security key:\r\n";
                txtData.Text += FindDataInStrString(input, "Security key") + "\r\n";

                txtHeaders.Text += "Key content:\r\n";
                txtData.Text += FindDataInStrString(input, "Key Content") + "\r\n";
            }
            else
            { txtData.Text = "The choosen network is not a wireless LAN network"; }
        }

        private string FindDataInStrString(string SourceString, string findthis)
        {
            string pattern = findthis;

            string[] substrings = Regex.Split(SourceString, pattern);

            int count = 0;

            substrings.GetLength(0);
            string local = "";
            foreach (string match in substrings)
            {
                if (count > 0)
                {
                    int index = substrings[count].IndexOf("\r\n");
                    if (index > 0)
                        local = substrings[count].Substring(0, index);
                    local = local.Trim();
                    local = local.Remove(0, 1).TrimStart();
                    return local;
                }

                count++;
            }
            return "";
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string SelectedNetwork = lbConsoleStream.SelectedItem.ToString();

            if (lbConsoleStream.SelectedItem != null)
            {

                MessageBoxResult result = MessageBox.Show("Deleting network " + SelectedNetwork + " , this can not be undone.\r\n Are you sure?", "Delete selected network", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {


                    if (HandleNetworks.DeleteNetwork(SelectedNetwork))
                    {
                        using (new WaitCursor())
                        {
                            Thread.Sleep(2000);
                            UpdateNetworkListbox();
                            txtData.Text = "";
                            txtHeaders.Text = "";
                        }

                        MessageBox.Show("Network: " + SelectedNetwork + " Deleted", "Deleted OK", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Error while deleting!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    btnDelete.IsEnabled = false;

                }
                else if (result == MessageBoxResult.No)
                {
                    //do something else
                }
            }
        }
    }
}
