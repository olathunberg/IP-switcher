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

            IsConnected = interFace.InterfaceState != Wlan.WlanInterfaceState.Disconnected;
            ProfileName = IsConnected ? interFace.CurrentConnection.profileName : null;
            SignalQuality = IsConnected ? interFace.CurrentConnection.wlanAssociationAttributes.wlanSignalQuality : 0;
            InterfaceState = interFace.InterfaceState;
            Channel = IsConnected ? interFace.Channel : default(int?);
            CurrentOperationMode = IsConnected ? interFace.CurrentOperationMode : Wlan.Dot11OperationMode.Unknown;
            RSSI =  IsConnected ? interFace.RSSI : default(int?);
            BssType = interFace.BssType;
            Autoconf = interFace.Autoconf;
            InterfaceName = interFace.InterfaceName;
            InterfaceDescription = interFace.InterfaceDescription;
        }

        public bool IsConnected { get; private set; }

        public string ProfileName  { get; private set; }

        public uint SignalQuality  { get; private set; }

        public Wlan.WlanInterfaceState InterfaceState  { get; private set; }

        public int? Channel { get; private set; }

        public Wlan.Dot11OperationMode CurrentOperationMode { get; private set; }

        public int? RSSI { get; private set; }

        public Wlan.Dot11BssType BssType { get; private set; }

        public bool Autoconf { get; private set; }

        public string InterfaceName { get; private set; }

        public string InterfaceDescription { get; private set; }

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
                return InterfaceName;
        }
    }
}
