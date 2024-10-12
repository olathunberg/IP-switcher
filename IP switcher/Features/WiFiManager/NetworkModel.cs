using System;
using System.ComponentModel;
using System.Text;
using NativeWifi;

namespace TTech.IP_Switcher.Features.WiFiManager
{
    public class NetworkModel
    {
        private readonly WLan.WlanAvailableNetwork network;

        public NetworkModel(WLan.WlanAvailableNetwork network)
        {
            this.network = network;
        }

        public string ProfileName => network.profileName;

        public uint SignalQuality => network.wlanSignalQuality;

        public string Dot11SSID
        {
            get
            {
                var result = new StringBuilder();
                for (int i = 0; i < network.dot11Ssid.SSIDLength; i++)
                    result.Append(Convert.ToChar(network.dot11Ssid.SSID[i]));

                return result.ToString();
            }
        }

        public string Dot11BssType => network.dot11BssType.ToString();

        public uint NumberOfBssids => network.numberOfBssids;

        public bool IsConnected => network.flags.HasFlag(WLan.WlanAvailableNetworkFlags.Connected);

        public bool NetworkConnectable => network.networkConnectable;

        public bool NetworkNotConnectable => !network.networkConnectable;

        public string NotConnectableReason => network.wlanNotConnectableReason.ToString();

        public bool SecurityEnabled => network.securityEnabled;

        public string Dot11DefaultAuthAlgorithm => network.dot11DefaultAuthAlgorithm.ToString();

        public string Dot11DefaultChiperAlgorithm => network.dot11DefaultCipherAlgorithm.ToString();

        public string Flags => network.flags.ToString();

        public override string ToString()
        {
            if (string.IsNullOrEmpty(ProfileName))
                return Dot11SSID;

            return ProfileName;
        }
    }
}
