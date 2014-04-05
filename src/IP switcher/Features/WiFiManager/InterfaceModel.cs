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

        public bool IsConnected { get { return interFace.InterfaceState != Wlan.WlanInterfaceState.Disconnected; } }

        public string ProfileName { get { return IsConnected ? interFace.CurrentConnection.profileName : null; } }

        public uint SignalQuality { get { return IsConnected ? interFace.CurrentConnection.wlanAssociationAttributes.wlanSignalQuality : 0; } }

        public Wlan.WlanInterfaceState InterfaceState { get { return interFace.InterfaceState; } }

        public int? Channel { get { return IsConnected ? interFace.Channel : default(int?); } }

        public Wlan.Dot11OperationMode CurrentOperationMode { get { return IsConnected ? interFace.CurrentOperationMode : Wlan.Dot11OperationMode.Unknown; } }

        public int? RSSI { get { return IsConnected ? interFace.RSSI : default(int?); } }

        public Wlan.Dot11BssType BssType { get { return interFace.BssType; } }

        public bool Autoconf { get { return interFace.Autoconf; } }

        public string InterfaceName { get { return interFace.InterfaceName; } }

        public string InterfaceDescription { get { return interFace.InterfaceDescription; } }

        public string[] GetProfiles()
        {
            if (interFace == null)
                return new string[] { };
            else
                return interFace.GetProfiles().Select(x => x.profileName).ToArray();
        }

        public string GetProfileXml(string profileName)
        {
            if (interFace == null)
                return string.Empty;
            else
                return interFace.GetProfileXml(profileName);
        }

        public override string ToString()
        {
            if (interFace == null)
                return string.Empty;
            else
                return interFace.InterfaceName;
        }
    }
}
