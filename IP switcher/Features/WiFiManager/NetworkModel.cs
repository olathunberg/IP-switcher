using System;
using System.ComponentModel;
using NativeWifi;

namespace TTech.IP_Switcher.Features.WiFiManager
{
    public class NetworkModel: INotifyPropertyChanged
    {
        private readonly Wlan.WlanAvailableNetwork network;

        public NetworkModel(Wlan.WlanAvailableNetwork network)
        {
            this.network = network;
        }

        public string ProfileName => network.profileName;

        public uint SignalQuality => network.wlanSignalQuality;

        public string Dot11SSID
        {
            get
            {
                string result = string.Empty;
                for (int i = 0; i < network.dot11Ssid.SSIDLength; i++)
                    result += Convert.ToChar(network.dot11Ssid.SSID[i]);

                return result;
            }
        }

        public string Dot11BssType => network.dot11BssType.ToString();

        public uint NumberOfBssids => network.numberOfBssids;

        public bool IsConnected => network.flags.HasFlag(Wlan.WlanAvailableNetworkFlags.Connected);

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

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
