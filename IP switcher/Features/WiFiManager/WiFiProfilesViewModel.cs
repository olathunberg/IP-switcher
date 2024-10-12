using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using NativeWifi;

namespace TTech.IP_Switcher.Features.WiFiManager
{
    public class WiFiProfilesViewModel : INotifyPropertyChanged
    {
        private System.Windows.Controls.UserControl owner;
        private InterfaceModel selectedInterface;
        private string selectedProfile;
        private ObservableCollection<InterfaceModel> interfaces;
        private bool effect;

        public WiFiProfilesViewModel()
        {
            Client = new WlanClient();
            Interfaces = new ObservableCollection<InterfaceModel>(Client.Interfaces.Select(x => new InterfaceModel(x)).ToList());
            SelectedInterface = Interfaces.FirstOrDefault();
        }

        void SelectedInterface_WLanReasonNotification(WLan.WlanNotificationData notifyData, WLan.WlanReasonCode reasonCode)
        {
            SelectedInterface.RefreshConnected();
        }

        void SelectedInterface_WLanNotification(WLan.WlanNotificationData notifyData)
        {
            if (notifyData.notificationSource == WLan.WlanNotificationSource.MSM)
            {
                SelectedInterface.RefreshConnected();
            }
        }

        void SelectedInterface_WLanConnectionNotification(WLan.WlanNotificationData notifyData, WLan.WlanConnectionNotificationData connNotifyData)
        {
            SelectedInterface.RefreshConnected();
        }

        public static WlanClient Client { get; private set; }

        public System.Windows.Controls.UserControl Owner
        {
            get => owner;
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
            get => interfaces;
            set
            {
                if (interfaces == value)
                    return;

                interfaces = value;
                NotifyPropertyChanged();
            }
        }

        public InterfaceModel SelectedInterface
        {
            get => selectedInterface;
            set
            {
                selectedInterface = value;
                if (selectedInterface != null)
                {
                    RefreshProfiles();

                    selectedInterface.interFace.WlanConnectionNotification += SelectedInterface_WLanConnectionNotification;
                    selectedInterface.interFace.WlanNotification += SelectedInterface_WLanNotification;
                    selectedInterface.interFace.WlanReasonNotification += SelectedInterface_WLanReasonNotification;
                }

                NotifyPropertyChanged();
            }
        }

        public string SelectedProfile
        {
            get => selectedProfile;
            set
            {
                selectedProfile = value;
                NotifyPropertyChanged();
                Task.Run(() =>
                    {
                        ProfileTree = ParsePropertyXml(selectedInterface.GetProfileXml(value));
                        NotifyPropertyChanged(nameof(ProfileTree));
                    });
            }
        }

        public bool Effect
        {
            get => effect;
            set
            {
                if (effect == value)
                    return;

                effect = value;

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<string> Profiles { get; set; }

        public List<ProfileInfo> ProfileTree { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RefreshProfiles()
        {
            Task.Run(() =>
            {
                Profiles = new ObservableCollection<string>(selectedInterface.GetProfiles());
                SelectedProfile = Profiles.FirstOrDefault(x => x.Equals(selectedInterface.ProfileName));
                NotifyPropertyChanged(nameof(Profiles));
            });
        }

        private List<ProfileInfo> ParsePropertyXml(string propertyXml)
        {
            if (string.IsNullOrEmpty(propertyXml))
                return [];
            var xd = new XmlDocument();
            xd.LoadXml(propertyXml);
            foreach (var item in xd.ChildNodes.OfType<XmlNode>())
            {
                if (item.LocalName == "WLANProfile")
                    return GetChildNodes(item);
            }
            return [];
        }

        private List<ProfileInfo> GetChildNodes(XmlNode node)
        {
            var result = new List<ProfileInfo>();
            foreach (var item in node.ChildNodes.OfType<XmlNode>())
            {
                if (item.HasChildNodes)
                    result.Add(new ProfileInfo(item.Name) { Children = GetChildNodes(item) });
                else
                    result.Add(new ProfileInfo(item.Value));
            }

            return result;
        }

        private void DoExportProfiles()
        {
            Effect = true;

            try
            {
                ProfileInfoExportExtension.WriteToFile(selectedInterface);
            }
            finally
            {
                Effect = false;
            }
        }

        private void DoImportPresets()
        {
            Effect = true;

            try
            {
                ProfileInfoExportExtension.ReadFromFile(selectedInterface);

                RefreshProfiles();
            }
            finally
            {
                Effect = false;
            }
        }

        private void DoDeleteSelected()
        {
            Effect = true;

            try
            {
                selectedInterface.interFace.DeleteProfile(selectedProfile);

                RefreshProfiles();
            }
            finally
            {
                Effect = false;
            }
        }

        private RelayCommand importProfilesCommand;
        public ICommand Import => importProfilesCommand ?? (importProfilesCommand = new RelayCommand(
                    () => DoImportPresets(),
                    () => selectedInterface != null));

        private RelayCommand exportProfilesCommand;
        public ICommand Export => exportProfilesCommand ?? (exportProfilesCommand = new RelayCommand(
                    () => DoExportProfiles(),
                    () => selectedInterface != null));
        private RelayCommand deleteSelectedCommand;
        public ICommand DeleteSelected => deleteSelectedCommand ?? (deleteSelectedCommand = new RelayCommand(
                    () => DoDeleteSelected(),
                    () => SelectedProfile != null));
    }
}
