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
    public class WiFiProfilesViewModel : INotifyPropertyChanged
    {
        private System.Windows.Controls.UserControl owner;
        private InterfaceModel selectedInterface;
        private string selectedProfile;
        private ObservableCollection<InterfaceModel> interfaces;

        #region Constructors
        public WiFiProfilesViewModel()
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
                            NotifyPropertyChanged("Profiles");
                        });
                }
                NotifyPropertyChanged();

                selectedInterface.interFace.WlanConnectionNotification += SelectedInterface_WlanConnectionNotification;
                selectedInterface.interFace.WlanNotification += selectedInterface_WlanNotification;
                selectedInterface.interFace.WlanReasonNotification += SelectedInterface_WlanReasonNotification;
            }
        }

        void SelectedInterface_WlanReasonNotification(Wlan.WlanNotificationData notifyData, Wlan.WlanReasonCode reasonCode)
        {
            SelectedInterface.UpdateInformation();
        }

        void selectedInterface_WlanNotification(Wlan.WlanNotificationData notifyData)
        {
            if (notifyData.notificationSource == Wlan.WlanNotificationSource.MSM)
            {
                SelectedInterface.UpdateInformation();
            }
        }

        void SelectedInterface_WlanConnectionNotification(Wlan.WlanNotificationData notifyData, Wlan.WlanConnectionNotificationData connNotifyData)
        {
            SelectedInterface.UpdateInformation();
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
                        ProfileTree = ParsePropertyXml(selectedInterface.GetProfileXml(value));
                        NotifyPropertyChanged("ProfileTree");
                    });
            }
        }

        public ObservableCollection<string> Profiles { get; set; }

        public List<ProfileInfo> ProfileTree { get; set; }

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private List<ProfileInfo> ParsePropertyXml(string propertyXml)
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

        private List<ProfileInfo> GetChildNodes(XmlNode node)
        {
            var result = new List<ProfileInfo>();
            foreach (XmlNode item in node.ChildNodes)
            {
                if (item.HasChildNodes)
                    result.Add(new ProfileInfo(item.Name) { Children = GetChildNodes(item) });
                else
                    result.Add(new ProfileInfo(item.Value));
            }

            return result;
        }
    }
}
