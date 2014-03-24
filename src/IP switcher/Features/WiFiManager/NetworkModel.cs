using NativeWifi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deucalion.IP_Switcher.Features.WiFiManager
{
    public class NetworkModel
    {
        private Wlan.WlanAvailableNetwork network;

        public NetworkModel(Wlan.WlanAvailableNetwork network)
        {
            this.network = network;
        }

        public string ProfileName { get { return network.profileName; } }

        public uint SignalQuality { get { return network.wlanSignalQuality; } }

        public override string ToString()
        {
            return ProfileName + SignalQuality.ToString();
        }
    }
}
