using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NativeWifi;

namespace Deucalion.IP_Switcher.Features.WiFiManager
{
    public class InterfaceModel : INotifyPropertyChanged
    {
        internal WlanClient.WlanInterface interFace;

        public InterfaceModel(WlanClient.WlanInterface interFace)
        {
            this.interFace = interFace;

            UpdateInformation();
        }

        public void UpdateInformation()
        {
            try
            {
                ProfileName = interFace.NetworkInterface != null && IsConnected ? interFace.CurrentConnection.profileName : null;
                SignalQuality = interFace.NetworkInterface != null && IsConnected ? interFace.CurrentConnection.wlanAssociationAttributes.wlanSignalQuality : 0;
                InterfaceState = interFace.NetworkInterface != null ? interFace.InterfaceState : Wlan.WlanInterfaceState.NotReady;
                Channel = interFace.NetworkInterface != null ? getChannel() : null;
                CurrentOperationMode = interFace.NetworkInterface != null && IsConnected ? interFace.CurrentOperationMode : Wlan.Dot11OperationMode.Unknown;
                RSSI = interFace.NetworkInterface != null ? getRSSI() : null;
                BssType = interFace.NetworkInterface != null ? interFace.BssType : Wlan.Dot11BssType.Any;
                Autoconf = interFace.NetworkInterface != null && interFace.Autoconf;
                InterfaceName = interFace.InterfaceName;
                InterfaceDescription = interFace.InterfaceDescription;

                foreach (var item in this.GetType().GetProperties())
                    NotifyPropertyChanged(item.Name);
            }
            catch (System.Exception ex)
            {
                Helpers.ShowWindow.Show.Message(ex.Message);
            }
        }

        private int? getChannel()
        {
            try
            {
                return IsConnected ? interFace.Channel : default(int?);
            }
            catch
            {
                return default(int?);
            }
        }

        private int? getRSSI()
        {
            try
            {
                return IsConnected ? interFace.RSSI : default(int?);
            }
            catch
            {
                return default(int?);
            }
        }

        public bool IsConnected { get { return interFace.InterfaceState != Wlan.WlanInterfaceState.Disconnected; } }

        public string ProfileName { get; private set; }

        public uint SignalQuality { get; private set; }

        public Wlan.WlanInterfaceState InterfaceState { get; private set; }

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

            return interFace.GetProfiles().Select(x => x.profileName).ToArray();
        }

        public List<Wlan.WlanProfileInfo> GetProfileInfos()
        {
            if (interFace == null)
                return new List<Wlan.WlanProfileInfo>();

            return interFace.GetProfiles().ToList();
        }

        public string GetProfileXml(string profileName)
        {
            if (interFace == null)
                return string.Empty;

            return interFace.GetProfileXml(profileName);
        }

        public IEnumerable<Wlan.WlanAvailableNetwork> GetAvailableNetworkList()
        {
            try
            {
                return interFace.GetAvailableNetworkList(Wlan.WlanGetAvailableNetworkFlags.IncludeAllAdhocProfiles).Where(x => x.flags != 0);
            }
            catch
            {
                return new List<Wlan.WlanAvailableNetwork>();
            }
        }

        public override string ToString()
        {
            if (interFace == null)
                return string.Empty;

            return InterfaceName;
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
