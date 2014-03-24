using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
using System.Management;
using NativeWifi;
using System.Xml;

namespace Deucalion.IP_Switcher.Features.WiFiManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WiFiManagerView : UserControl
    {
        public WiFiManagerView()
        {
            InitializeComponent();

            var mainViewModel = MainGrid.DataContext as WiFiManagerViewModel;
            if (mainViewModel != null)
                mainViewModel.Owner = this;
        }

        //public WiFiManagerView Model { get;set;}
        //private void UpdateNetworkListbox()
        //{
        //    var client = new WlanClient();

        //   // lbConsoleStream.Items.Clear();

        //    foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
        //    {
        //        // Retrieves XML configurations of existing profiles.
        //        // This can assist you in constructing your own XML configuration
        //        // (that is, it will give you an example to follow).

        //        foreach (Wlan.WlanProfileInfo profileInfo in wlanIface.GetProfiles())
        //        {
                                       
        //           // lbConsoleStream.Items.Add(profileInfo.profileName);
                    
                    
        //        }
        //    }
        //    client=null;
        //}

        //private void UpdateSecurityInfoField(string input)
        //{

        //    foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
        //    {
        //        // Retrieves XML configurations of existing profiles.
        //        foreach (Wlan.WlanProfileInfo profileInfo in wlanIface.GetProfiles())
        //        {
        //            if (profileInfo.profileName == input)
        //            {
        //                //txtData.Text = wlanIface.GetProfileXml(profileInfo.profileName);
        //                txtData.Text = FormatXml(wlanIface.GetProfileXml(profileInfo.profileName));
        //            }
        //        }
        //    }


        //    //txtHeaders.Text = "";
        //    //txtData.Text = "";

        //    //if (FindDataInStrString(input, "Type") == "Wireless LAN")
        //    //{

        //    //    txtHeaders.Text += "Mode:\r\n";
        //    //    txtData.Text += FindDataInStrString(input, "Connection mode") + "\r\n";

        //    //    txtHeaders.Text += "Broadcast:\r\n";
        //    //    txtData.Text += FindDataInStrString(input, "Network broadcast") + "\r\n";

        //    //    txtHeaders.Text += "SSID:\r\n";
        //    //    SSID = FindDataInStrString(input, "SSID name");
        //    //    SSID = SSID.Remove(SSID.Length - 1, 1);
        //    //    SSID = SSID.Remove(0, 1);
        //    //    txtData.Text += SSID + "\r\n";

        //    //    txtHeaders.Text += "\r\n";
        //    //    txtData.Text += "\r\n";

        //    //    txtHeaders.Text += "Network type:\r\n";
        //    //    txtData.Text += FindDataInStrString(input, "Network type") + "\r\n";

        //    //    txtHeaders.Text += "Radio type:\r\n";
        //    //    txtData.Text += FindDataInStrString(input, "Radio type") + "\r\n";

        //    //    txtHeaders.Text += "\r\n";
        //    //    txtData.Text += "\r\n";

        //    //    txtHeaders.Text += "Authentication:\r\n";
        //    //    txtData.Text += FindDataInStrString(input, "Authentication") + "\r\n";

        //    //    txtHeaders.Text += "Cipher:\r\n";
        //    //    txtData.Text += FindDataInStrString(input, "Cipher") + "\r\n";

        //    //    txtHeaders.Text += "Security key:\r\n";
        //    //    txtData.Text += FindDataInStrString(input, "Security key") + "\r\n";

        //    //    txtHeaders.Text += "Key content:\r\n";
        //    //    txtData.Text += FindDataInStrString(input, "Key Content") + "\r\n";
        //    //}
        //    //else
        //    //{ txtData.Text = "The choosen network is not a wireless LAN network"; }
        //}

        //private string FindDataInStrString(string SourceString, string findthis)
        //{
        //    string pattern = findthis;

        //    string[] substrings = Regex.Split(SourceString, pattern);

        //    int count = 0;

        //    substrings.GetLength(0);
        //    string local = "";
        //    foreach (string match in substrings)
        //    {
        //        if (count > 0)
        //        {
        //            int index = substrings[count].IndexOf("\r\n");
        //            if (index > 0)
        //                local = substrings[count].Substring(0, index);
        //            local = local.Trim();
        //            local = local.Remove(0, 1).TrimStart();
        //            return local;
        //        }

        //        count++;
        //    }
        //    return "";
        //}
    }
}
