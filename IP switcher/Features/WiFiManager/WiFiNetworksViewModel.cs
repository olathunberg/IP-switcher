using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NativeWifi;

namespace TTech.IP_Switcher.Features.WiFiManager
{
    public class WiFiNetworksViewModel : INotifyPropertyChanged
    {
        private System.Windows.Controls.UserControl owner;
        private InterfaceModel selectedInterface;
        private NetworkModel selectedNetwork;
        private ObservableCollection<InterfaceModel> interfaces;

        #region Constructors
        public WiFiNetworksViewModel()
        {
            Client = new WlanClient();
            Interfaces = new ObservableCollection<InterfaceModel>(Client.Interfaces.Select(x => new InterfaceModel(x)).ToList());
            SelectedInterface = Interfaces.FirstOrDefault();
        }
        #endregion

        public static WlanClient Client { get; private set; }

        public System.Windows.Controls.UserControl Owner
        {
            get { return owner; }
            set
            {
                if (owner == value)
                    return;

                if (owner != null)
                    owner.IsVisibleChanged -= Owner_IsVisibleChanged;

                owner = value;

                if (owner != null)
                    owner.IsVisibleChanged += Owner_IsVisibleChanged;

                NotifyPropertyChanged();
            }
        }

        private void Owner_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (owner.IsVisible)
            {
                selectedInterface.interFace.WlanConnectionNotification += SelectedInterface_WlanConnectionNotification;
                selectedInterface.interFace.WlanNotification += SelectedInterface_WlanNotification;
                selectedInterface.interFace.WlanReasonNotification += SelectedInterface_WlanReasonNotification;

                SelectedInterface?.UpdateInformation();
            }
            else
            {
                selectedInterface.interFace.WlanConnectionNotification -= SelectedInterface_WlanConnectionNotification;
                selectedInterface.interFace.WlanNotification -= SelectedInterface_WlanNotification;
                selectedInterface.interFace.WlanReasonNotification -= SelectedInterface_WlanReasonNotification;
            }
        }

        public ObservableCollection<InterfaceModel> Interfaces
        {
            get { return interfaces; }
            set
            {
                if (interfaces == value)
                    return;
                interfaces = value; NotifyPropertyChanged();
            }
        }

        public InterfaceModel SelectedInterface
        {
            get { return selectedInterface; }
            set
            {
                selectedInterface = value;
                if (selectedInterface != null)
                {
                    Task.Run(() =>
                        {
                            Networks = new ObservableCollection<NetworkModel>(selectedInterface.GetAvailableNetworkList().Select(x => new NetworkModel(x)));
                            SelectedNetwork = Networks.FirstOrDefault(x => x.IsConnected);
                            NotifyPropertyChanged(nameof(Networks));
                        });
                }

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<NetworkModel> Networks { get; set; }

        public NetworkModel SelectedNetwork
        {
            get { return selectedNetwork; }
            set
            {
                selectedNetwork = value;
                NotifyPropertyChanged();
            }
        }

        void SelectedInterface_WlanReasonNotification(Wlan.WlanNotificationData notifyData, Wlan.WlanReasonCode reasonCode)
        {
            if (owner != null && owner.IsVisible)
            {
                SelectedInterface?.UpdateInformation();
                UpdateNetworkSignalQuality();
            }
        }

        void SelectedInterface_WlanNotification(Wlan.WlanNotificationData notifyData)
        {
            if (owner != null && owner.IsVisible && notifyData.notificationSource == Wlan.WlanNotificationSource.MSM)
            {
                SelectedInterface?.UpdateInformation();
                UpdateNetworkSignalQuality();
            }
        }

        void SelectedInterface_WlanConnectionNotification(Wlan.WlanNotificationData notifyData, Wlan.WlanConnectionNotificationData connNotifyData)
        {
            if (owner != null && owner.IsVisible)
            {
                SelectedInterface?.UpdateInformation();
                UpdateNetworkSignalQuality();

                Task.Run(() =>
                {
                    Networks = new ObservableCollection<NetworkModel>(selectedInterface.GetAvailableNetworkList().Select(x => new NetworkModel(x)));
                    SelectedNetwork = Networks.FirstOrDefault(x => x.IsConnected);
                    NotifyPropertyChanged(nameof(Networks));
                });
            }
        }

        void UpdateNetworkSignalQuality()
        {
            SelectedNetwork = new NetworkModel(selectedInterface.GetAvailableNetworkList().FirstOrDefault(x => x.profileName == SelectedNetwork?.ProfileName));
            NotifyPropertyChanged(nameof(SelectedNetwork.SignalQuality));
        }

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
