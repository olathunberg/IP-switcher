using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using NativeWifi;
using TTech.IP_Switcher.Helpers;

namespace TTech.IP_Switcher.Features.WiFiManager
{
    public class InterfaceModel : INotifyPropertyChanged
    {
        internal WlanClient.WlanInterface interFace;
        private bool isUpdating;

        public InterfaceModel(WlanClient.WlanInterface interFace)
        {
            this.interFace = interFace;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            UpdateInformation();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public void RefreshConnected()
        {
            NotifyPropertyChanged(nameof(IsConnected));
        }

        public async Task UpdateInformation()
        {
            if (isUpdating)
                return;
            isUpdating = true;

            // Cool down updatetimes
            await Task.Delay(200);

            try
            {
                if (interFace.NetworkInterface != null)
                {
                    ProfileName = IsConnected ? interFace.CurrentConnection.profileName : null;
                    SignalQuality = IsConnected ? interFace.CurrentConnection.wlanAssociationAttributes.wlanSignalQuality : 0;
                    InterfaceState = interFace.InterfaceState;
                    Channel = getChannel();
                    CurrentOperationMode = IsConnected ? interFace.CurrentOperationMode : Wlan.Dot11OperationMode.Unknown;
                    RSSI = getRSSI();
                    BssType = interFace.BssType;
                    Autoconf = interFace.Autoconf;
                }

                InterfaceName = interFace.InterfaceName;
                InterfaceDescription = interFace.InterfaceDescription;

                foreach (var item in this.GetType().GetProperties())
                    NotifyPropertyChanged(item.Name);
            }
            catch (System.Exception ex)
            {
                SimpleMessenger.Default.SendMessage("ErrorText", ex.Message);
            }
            isUpdating = false;
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
