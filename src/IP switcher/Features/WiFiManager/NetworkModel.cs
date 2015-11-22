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
        private readonly Wlan.WlanAvailableNetwork network;

        public NetworkModel(Wlan.WlanAvailableNetwork network)
        {
            this.network = network;
        }

        public string ProfileName { get { return network.profileName; } }

        public uint SignalQuality { get { return network.wlanSignalQuality; } }

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

        public string Dot11BssType { get { return network.dot11BssType.ToString(); } }

        public uint NumberOfBssids { get { return network.numberOfBssids; } }

        public bool IsConnected { get { return network.flags.HasFlag(Wlan.WlanAvailableNetworkFlags.Connected); } }

        public bool NetworkConnectable { get { return network.networkConnectable; } }

        public bool NetworkNotConnectable { get { return !network.networkConnectable; } }

        public string NotConnectableReason { get { return network.wlanNotConnectableReason.ToString(); } }

        //private uint numberOfPhyTypes;

        //public Dot11PhyType[] Dot11PhyTypes
        //{
        //    get
        //    {
        //        Dot11PhyType[] ret = new Dot11PhyType[numberOfPhyTypes];
        //        Array.Copy(dot11PhyTypes, ret, numberOfPhyTypes);
        //        return ret;
        //    }
        //}
        /// <summary>
        /// Specifies if there are more than 8 PHY types supported.
        /// When this member is set to <c>true</c>, an application must call <see cref="WlanClient.WlanInterface.GetNetworkBssList"/> to get the complete list of PHY types.
        /// <see cref="WlanBssEntry.phyId"/> contains the PHY type for an entry.
        /// </summary>
        //public bool morePhyTypes;

        public bool SecurityEnabled { get { return network.securityEnabled; } }

        public string Dot11DefaultAuthAlgorithm { get { return network.dot11DefaultAuthAlgorithm.ToString(); } }

        public string Dot11DefaultChiperAlgorithm { get { return network.dot11DefaultCipherAlgorithm.ToString(); } }

        public string Flags { get { return network.flags.ToString(); } }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(ProfileName))
                return Dot11SSID;

            return ProfileName;
        }
    }
}
