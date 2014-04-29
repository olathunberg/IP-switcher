using NativeWifi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;

namespace Deucalion.IP_Switcher.Features.WiFiManager
{
    public class WiFiManagerViewModel : INotifyPropertyChanged
    {
        private System.Windows.Controls.UserControl owner;
        private InterfaceModel selectedInterface;
        private string selectedProfile;
        private NetworkModel selectedNetwork;
        private ObservableCollection<InterfaceModel> interfaces; 

        #region Constructors
        public WiFiManagerViewModel()
        {
            Client = new WlanClient(); 
            Interfaces = new ObservableCollection<InterfaceModel>(Client.Interfaces.Select(x => new InterfaceModel(x)).ToList());
            SelectedInterface = Interfaces.First();
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

                owner = value;

                NotifyPropertyChanged();
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
                            Profiles = new ObservableCollection<string>(selectedInterface.GetProfiles());
                            SelectedProfile = Profiles.FirstOrDefault();
                            var list = Client.Interfaces.First().GetAvailableNetworkList(Wlan.WlanGetAvailableNetworkFlags.IncludeAllAdhocProfiles).Where(x=>x.flags != 0);
                            Networks = new ObservableCollection<NetworkModel>(list.Select(x => new NetworkModel(x)));

                            SelectedNetwork = Networks.FirstOrDefault(x => x.IsConnected);
                        });
                }
                NotifyPropertyChanged("Profiles");
                NotifyPropertyChanged();

                //SelectedInterface.WlanConnectionNotification += SelectedInterface_WlanConnectionNotification;
                //SelectedInterface.WlanNotification += selectedInterface_WlanNotification;
                //SelectedInterface.WlanReasonNotification += SelectedInterface_WlanReasonNotification;
            }
        }

        void SelectedInterface_WlanReasonNotification(Wlan.WlanNotificationData notifyData, Wlan.WlanReasonCode reasonCode)
        {
            //      Networks = new ObservableCollection<NetworkModel>(Client.Interfaces.First().GetAvailableNetworkList(Wlan.WlanGetAvailableNetworkFlags.IncludeAllAdhocProfiles).AsEnumerable().Select(x => new NetworkModel(x)));
        }

        void selectedInterface_WlanNotification(Wlan.WlanNotificationData notifyData)
        {
            NotifyPropertyChanged("SelectedInterface");
        }

        void SelectedInterface_WlanConnectionNotification(Wlan.WlanNotificationData notifyData, Wlan.WlanConnectionNotificationData connNotifyData)
        {
            NotifyPropertyChanged("SelectedInterface");
        }

        public string SelectedProfile
        {
            get { return selectedProfile; }
            set
            {
                selectedProfile = value;
                NotifyPropertyChanged();
                Task.Run(() =>
                    {
                        PropertyXml = ParsePropertyXml(selectedInterface.GetProfileXml(value)).Aggregate((current, next) => current + Environment.NewLine + next);
                        NotifyPropertyChanged("PropertyXml");
                    });
            }
        }

        public ObservableCollection<string> Profiles { get; set; }

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

        public string PropertyXml { get; set; }

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private string[] ParsePropertyXml(string propertyXml)
        {
            var xd = new XmlDocument();
            xd.LoadXml(propertyXml);
            foreach (XmlNode item in xd.ChildNodes)
            {
                if (item.LocalName == "WLANProfile")
                    return GetChildNodes(item);
            }
            return null;
        }

        private string[] GetChildNodes(XmlNode node)
        {
            List<string> result = new List<string>();
            foreach (XmlNode item in node.ChildNodes)
            {
                if (item.HasChildNodes)
                {
                    if (item.ChildNodes.Count > 1)
                        result.Add(item.Name);
                    result.AddRange(GetChildNodes(item));
                }
                else
                    result.Add(item.ParentNode.LocalName + ": " + item.Value);
            }

            return result.ToArray();
        }
    }
}
