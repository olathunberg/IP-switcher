using NativeWifi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deucalion.IP_Switcher.Features.WiFiManager
{
    public class InterfaceModel
    {
        private WlanClient.WlanInterface interFace;

        public InterfaceModel(WlanClient.WlanInterface interFace)
        {
            this.interFace = interFace;
        }

        private bool IsConnected { get { return interFace.InterfaceState != Wlan.WlanInterfaceState.Disconnected; } }

        public string ProfileName { get { return IsConnected ? interFace.CurrentConnection.profileName : null; } }

        public uint SignalQuality { get { return IsConnected ? interFace.CurrentConnection.wlanAssociationAttributes.wlanSignalQuality : 0; } }



        public override string ToString()
        {
            return ProfileName + SignalQuality.ToString();
        }
    }
}
