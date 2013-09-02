using Deucalion.IP_Switcher.Classes;
using Deucalion.IP_Switcher.Features.IpSwitcher.Resources;
using Deucalion.IP_Switcher.Features.Location;
using Deucalion.IP_Switcher.Features.LocationDetail;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using Deucalion.IP_Switcher.Features;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Deucalion.IP_Switcher.Helpers.Show;
using System.Dynamic;

namespace Deucalion.IP_Switcher.Features.IpSwitcher
{
    public class IpSwitcherViewModel : INotifyPropertyChanged
    {
        #region Fields
        private System.Windows.Controls.UserControl _Owner;
        private string statusText = IpSwitcherViewModelLoc.Status_None;
        private bool isWorking = false;
        private AdapterData.AdapterData selectedActiveAdapter;
        private List<AdapterData.AdapterData> activeAdapters;
        private AdapterData.AdapterData selectedInactiveAdapter;
        private List<AdapterData.AdapterData> inactiveAdapters;
        private bool isEnabled = true;
        private AdapterData.AdapterDataModel _Current;
        private LocationModel _CurrentLocation;
        private Location.Location selectedLocation;
        private List<Location.Location> locations;
        private string externalIp;
        #endregion

        #region Constructors
        public IpSwitcherViewModel()
        {
            updateAdaptersCommand = new RelayCommand(async () => { await DoUpdateAdaptersListAsync(); },
                () => true);
            activateAdapterCommand = new RelayCommand(() => DoActivateAdapter(),
                () => Current == null ? false : !Current.IsActive);
            deactivateAdapterCommand = new RelayCommand(() => DoDeactivateAdapter(),
                () => Current == null ? false : Current.IsActive);
            applyLocationCommand = new RelayCommand(() => DoApplyLocation(),
                () => SelectedLocation != null && Current != null);
            extractConfigToNewLocationCommand = new RelayCommand(() => DoExtractConfigToNewLocation(),
                () => Current != null && Current.HasAdapter);
            editLocationCommand = new RelayCommand(() => DoEditLocation(),
                () => SelectedLocation != null);
            deleteLocationCommand = new RelayCommand(() => DoDeleteLocation(),
                () => SelectedLocation != null);
            manualSettingsCommand = new RelayCommand(async () => await DoManualSettings(),
                () => Current != null && Current.HasAdapter);
            createLocationCommand = new RelayCommand(() => DoCreateLocation(),
                () => true);
            importPresetsCommand = new RelayCommand(() => DoImportPresets(),
                () => true);
            exportPresetsCommand = new RelayCommand(() => DoExportPresets(),
                () => Locations.Count > 0);

            getExternalIpCommand = new RelayCommand(() => GetPublicIpAddress());

            var tmpTask = DoUpdateAdaptersListAsync();

            Locations = Settings.Default.Locations.ToList();
            SelectedLocation = Locations.FirstOrDefault();

            GetPublicIpAddress();

            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
        }
        #endregion

        #region Public Properties
        public string StatusText
        {
            get { return statusText; }
            set
            {
                if (statusText == value)
                    return;
                statusText = value; NotifyPropertyChanged();
            }
        }

        public bool IsWorking
        {
            get { return isWorking; }
            set
            {
                if (isWorking == value)
                    return;
                isWorking = value; NotifyPropertyChanged();
            }
        }

        public AdapterData.AdapterData SelectedActiveAdapter
        {
            get { return selectedActiveAdapter; }
            set
            {
                if (selectedActiveAdapter == value)
                    return;
                selectedActiveAdapter = value;

                if (SelectedActiveAdapter != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                        {
                            SelectedInactiveAdapter = null;
                            Current = new AdapterData.AdapterDataModel(value);
                        });
                }

                NotifyPropertyChanged();
            }
        }

        public List<AdapterData.AdapterData> ActiveAdapters
        {
            get { return activeAdapters; }
            set
            {
                if (activeAdapters == value)
                    return;
                activeAdapters = value; NotifyPropertyChanged();
            }
        }

        public AdapterData.AdapterData SelectedInactiveAdapter
        {
            get { return selectedInactiveAdapter; }
            set
            {
                if (selectedInactiveAdapter == value)
                    return;
                selectedInactiveAdapter = value;

                if (SelectedInactiveAdapter != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                        {
                            SelectedActiveAdapter = null;
                            Current = new AdapterData.AdapterDataModel(value);
                        });
                }

                NotifyPropertyChanged();
            }
        }

        public List<AdapterData.AdapterData> InactiveAdapters
        {
            get { return inactiveAdapters; }
            set
            {
                if (inactiveAdapters == value)
                    return;
                inactiveAdapters = value; NotifyPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (isEnabled == value)
                    return;
                isEnabled = value; NotifyPropertyChanged();
            }
        }

        public AdapterData.AdapterDataModel Current
        {
            get { return _Current; }
            set
            {
                if (value == null || _Current == value)
                    return;
                _Current = value;

                NotifyPropertyChanged();
                activateAdapterCommand.RaiseCanExecuteChanged();
                deactivateAdapterCommand.RaiseCanExecuteChanged();
                extractConfigToNewLocationCommand.RaiseCanExecuteChanged();
                applyLocationCommand.RaiseCanExecuteChanged();
                manualSettingsCommand.RaiseCanExecuteChanged();
            }
        }

        public LocationModel CurrentLocation
        {
            get { return _CurrentLocation; }
            set
            {
                if (value == null || _CurrentLocation == value)
                    return;
                _CurrentLocation = value;

                NotifyPropertyChanged();
            }
        }

        public List<Location.Location> Locations
        {
            get { return locations; }
            set
            {
                if (locations == value)
                    return;
                locations = value;
                NotifyPropertyChanged();
            }
        }

        public Location.Location SelectedLocation
        {
            get { return selectedLocation; }
            set
            {
                selectedLocation = value;
                FillLocationDetails();
                NotifyPropertyChanged();
                applyLocationCommand.RaiseCanExecuteChanged();
            }
        }

        private string title;
        public string Title { get { return title; } set { title = value; NotifyPropertyChanged(); } }

        private bool effect;
        public bool Effect
        {
            get { return effect; }
            set
            {
                if (effect == value)
                    return;

                effect = value;

                NotifyPropertyChanged();
            }
        }

        public string ExternalIp
        {
            get { return externalIp; }
            set
            {
                if (externalIp == value)
                    return;
                externalIp = value;

                NotifyPropertyChanged();
            }
        }

        public System.Windows.Controls.UserControl Owner
        {
            get { return _Owner; }
            set
            {
                if (_Owner == value)
                    return;

                _Owner = value;

                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Private / Protected
        private enum Status
        {
            Idle,
            ActivatingAdapter,
            DeactivatingAdapter,
            ApplyingLocation,
            UpdatingAdapters
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        private void DoExportPresets()
        {
            Effect = true;

            var dialog = new Microsoft.Win32.SaveFileDialog()
            {
                DefaultExt = ".xml",
                Filter = IpSwitcherViewModelLoc.ExportFilter,
                CheckPathExists = true,
                AddExtension = true,
                FileName = IpSwitcherViewModelLoc.ExportDefaultFilename
            };

            if (!dialog.ShowDialog() ?? false)
            {
                Effect = false;
                return;
            }

            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(LocationExport));

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(dialog.FileName))
            {
                writer.Serialize(file, new LocationExport()
                {
                    Locations = Locations,
                    Version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
                });
            }

            Effect = false;
        }

        private void DoImportPresets()
        {
            Effect = true;

            var dialog = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = ".xml",
                Filter = IpSwitcherViewModelLoc.ExportFilter,
                CheckPathExists = true,
                AddExtension = true,
                FileName = IpSwitcherViewModelLoc.ExportDefaultFilename
            };

            if (!dialog.ShowDialog() ?? false)
            {
                Effect = false;
                return;
            }


            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(LocationExport));

            var importedLocations = new LocationExport();
            try
            {
                if (System.IO.File.Exists(dialog.FileName))
                {
                    using (System.IO.StreamReader file = new System.IO.StreamReader(dialog.FileName))
                    {
                        importedLocations = (LocationExport)reader.Deserialize(file);
                    }
                }

                Settings.Default.Locations.AddRange(importedLocations.Locations);
                Settings.Save();
                Locations = Settings.Default.Locations.ToList();
            }
            catch (Exception ex)
            {
                Show.Message(String.Format(IpSwitcherViewModelLoc.ErrorImportingLocations, Environment.NewLine, dialog.FileName, ex.Message));
            }

            Effect = false;
        }

        private void DoCreateLocation()
        {
            Effect = true;

            LocationDetailView editLocationForm = new LocationDetailView(new Location.Location()) { Owner = Window.GetWindow(_Owner), WindowStartupLocation = WindowStartupLocation.CenterOwner };
            editLocationForm.ShowDialog();
            if (editLocationForm.DialogResult ?? false)
            {
                Settings.Default.Locations.Add((Location.Location)editLocationForm.DataContext);
                Settings.Save();
                Locations = Settings.Default.Locations.ToList();
                SelectedLocation = Locations.Last();
            }

            Effect = false;
        }

        private async Task DoManualSettings()
        {
            Effect = true;
            //dynamic parameters = new ExpandoObject();
            //parameters.IsManualSettings = true;
            //parameters.Location = GetSelectedAdapter();
            //var editLocationResult = Show.Dialog<LocationDetailView>(parameters);
            var editLocationForm = new LocationDetailView(ExtractConfig(GetSelectedAdapter(), string.Empty), true) { Owner = Window.GetWindow(_Owner), WindowStartupLocation = WindowStartupLocation.CenterOwner };
            editLocationForm.ShowDialog();
            Effect = false;

            if (editLocationForm.DialogResult ?? false)
            {
                SetStatus(Status.ApplyingLocation);

                var adapter = GetSelectedAdapter();
                var location = (Location.Location)editLocationForm.DataContext;

                await NetworkConfigurator.ApplyLocation(location, adapter);
                await DoUpdateAdaptersListAsync();

                SetStatus(Status.Idle);
            }
        }

        private void DoDeleteLocation()
        {
            var selectedIndex = Settings.Default.Locations.IndexOf(SelectedLocation);
            Settings.Default.Locations.Remove(SelectedLocation);
            Settings.Save();

            Locations = Settings.Default.Locations.ToList();
            if (selectedIndex >= Locations.Count && Locations.Count > 0)
                SelectedLocation = Locations.FirstOrDefault();
            else if (Locations.Count > 0)
                SelectedLocation = Locations[selectedIndex];
            else
                SelectedLocation = null;
        }

        private async void DoApplyLocation()
        {
            SetStatus(Status.ApplyingLocation);

            var adapter = GetSelectedAdapter();
            var location = SelectedLocation;
            await NetworkConfigurator.ApplyLocation(location, adapter);
            await DoUpdateAdaptersListAsync();

            SetStatus(Status.Idle);
        }

        private void DoEditLocation()
        {
            Effect = true;

            var editLocationForm = new LocationDetailView(SelectedLocation.Clone()) { Owner = Window.GetWindow(_Owner), WindowStartupLocation = WindowStartupLocation.CenterOwner };
            editLocationForm.ShowDialog();

            if (editLocationForm.DialogResult ?? false)
            {
                var index = Settings.Default.Locations.IndexOf(SelectedLocation);
                Settings.Default.Locations[index] = (Location.Location)editLocationForm.DataContext;
                Settings.Save();
                Locations = Settings.Default.Locations;
                SelectedLocation = Settings.Default.Locations[index];
            }

            Effect = false;
        }

        private void DoExtractConfigToNewLocation()
        {
            Effect = true;
            var inputBox = new Deucalion.IP_Switcher.Features.InputBox.InputBoxView() { Owner = Window.GetWindow(_Owner), WindowStartupLocation = WindowStartupLocation.CenterOwner };
            inputBox.ShowDialog();

            // If user saved, replace original
            if (inputBox.DialogResult ?? false)
            {
                Settings.Default.Locations.Add(ExtractConfig(GetSelectedAdapter(), inputBox.Result));
                Settings.Save();
                Locations = Settings.Default.Locations.ToList();
                SelectedLocation = Locations.Last();
            }

            Effect = false;
        }

        private async void GetPublicIpAddress()
        {
            ExternalIp = IpSwitcherViewModelLoc.Searching;
            var request = WebRequest.Create("http://ifconfig.me") as HttpWebRequest;

            request.UserAgent = "curl"; // this simulate curl linux command

            string publicIPAddress;

            request.Method = "GET";

            try
            {
                using (var response = await request.GetResponseAsync())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        publicIPAddress = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {
                publicIPAddress = IpSwitcherViewModelLoc.SearchFailed;
            }

            ExternalIp = publicIPAddress.Replace("\n", "");
        }

        private AdapterData.AdapterData GetSelectedAdapter()
        {
            if (SelectedActiveAdapter != null)
                return SelectedActiveAdapter;
            if (SelectedInactiveAdapter != null)
                return SelectedInactiveAdapter;

            return null;
        }

        private static Location.Location ExtractConfig(AdapterData.AdapterData adapter, string NewName)
        {
            var location = new Location.Location() { Description = IpSwitcherViewModelLoc.NewLocationDescription, ID = Settings.Default.GetNextID() };
            if (adapter.networkInterface == null)
                return location;

            var properties = adapter.networkInterface.GetIPProperties();

            // DHCP Enabled:
            location.DHCPEnabled = properties.GetIPv4Properties().IsDhcpEnabled;

            location.IPList.Clear();
            foreach (var uniCast in properties.UnicastAddresses)
            {
                // Ignore loop-back addresses & IPv6
                if (!IPAddress.IsLoopback(uniCast.Address) && uniCast.Address.AddressFamily != AddressFamily.InterNetworkV6 && uniCast.IPv4Mask != null)
                {
                    var newIp = new IPDefinition() { IP = uniCast.Address.ToString(), NetMask = uniCast.IPv4Mask.ToString() };

                    location.IPList.Add(newIp);
                }
            }

            foreach (var gateWay in properties.GatewayAddresses)
                location.Gateways.Add(new IPv4Address() { IP = gateWay.Address.ToString() });

            foreach (var dns in properties.DnsAddresses)
                location.DNS.Add(new IPv4Address() { IP = dns.ToString() });

            location.Description = NewName;

            return location;
        }

        private void FillLocationDetails()
        {
            if (SelectedLocation == null)
                CurrentLocation = new LocationModel();
            else
                CurrentLocation = new LocationModel(SelectedLocation);
        }

        private void FillAdapterLists(List<AdapterData.AdapterData> adapterList)
        {
            // Remember selected adapter
            var tmpAdapter = GetSelectedAdapter();
            string selectedAdapter = tmpAdapter != null ? tmpAdapter.networkAdapter.GUID : string.Empty;

            // Update list with current adapters
            var activeAdapters = new List<AdapterData.AdapterData>();
            var inactiveAdapters = new List<AdapterData.AdapterData>();
            foreach (AdapterData.AdapterData item in adapterList)
            {
                if (item.networkAdapter.NetEnabled)
                    activeAdapters.Add(item);
                else
                    inactiveAdapters.Add(item);
            }

            // Get previously selected adapter, if any
            var activeSelected = activeAdapters.FirstOrDefault(z => z.networkAdapter.GUID == selectedAdapter);
            var inactiveSelected = inactiveAdapters.FirstOrDefault(z => z.networkAdapter.GUID == selectedAdapter);

            ActiveAdapters = activeAdapters;
            InactiveAdapters = inactiveAdapters;

            // Reselect previous adapter or first
            if (activeSelected != null)
                SelectedActiveAdapter = activeSelected;
            else if (inactiveSelected != null)
                SelectedInactiveAdapter = inactiveSelected;
            else if (ActiveAdapters.Count > 0)
                SelectedActiveAdapter = ActiveAdapters.First();
            else if (InactiveAdapters.Count > 0)
                SelectedInactiveAdapter = InactiveAdapters.First();
        }

        private void SetStatus(Status status)
        {
            switch (status)
            {
                case Status.Idle:
                    StatusText = IpSwitcherViewModelLoc.Status_Idle;
                    IsWorking = false;
                    break;
                case Status.ActivatingAdapter:
                    StatusText = IpSwitcherViewModelLoc.Status_ActivatingAdapter;
                    IsWorking = true;
                    break;
                case Status.DeactivatingAdapter:
                    StatusText = IpSwitcherViewModelLoc.Status_DeactivatingAdapter;
                    IsWorking = true;
                    break;
                case Status.ApplyingLocation:
                    StatusText = IpSwitcherViewModelLoc.Status_ApplyingLocation;
                    IsWorking = true;
                    break;
                case Status.UpdatingAdapters:
                    StatusText = IpSwitcherViewModelLoc.Status_UpdatingAdapters;
                    IsWorking = true;
                    break;
                default:
                    break;
            }
            IsEnabled = !IsWorking;
        }

        internal async void DoActivateAdapter()
        {
            var adapter = GetSelectedAdapter();

            SetStatus(Status.ActivatingAdapter);
            var couldEnable = await adapter.networkAdapter.EnableAsync();

            if (couldEnable != 0)
                Show.Message(IpSwitcherViewModelLoc.ActivationFailed, string.Format(IpSwitcherViewModelLoc.SystemMessage, WMI.FormatMessage.GetMessage((int)couldEnable)));

            SetStatus(Status.Idle);
        }

        internal async void DoDeactivateAdapter()
        {
            SetStatus(Status.DeactivatingAdapter);

            var couldDisable = await GetSelectedAdapter().networkAdapter.DisableAsync();
            if (couldDisable != 0)
                Show.Message(IpSwitcherViewModelLoc.DeactivationFailed, string.Format(IpSwitcherViewModelLoc.SystemMessage, WMI.FormatMessage.GetMessage((int)couldDisable)));

            FillAdapterLists(AdapterData.AdapterDataHelpers.GetAdapters());

            SetStatus(Status.Idle);
        }

        internal async Task DoUpdateAdaptersListAsync()
        {
            SetStatus(Status.UpdatingAdapters);

            FillAdapterLists(await Task.Factory.StartNew(() => AdapterData.AdapterDataHelpers.GetAdapters()));

            SetStatus(Status.Idle);
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Event Handlers
        async void NetworkChange_NetworkAvailabilityChanged(object sender, System.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
        {
            await DoUpdateAdaptersListAsync();
        }
        #endregion

        #region Commands
        private RelayCommand updateAdaptersCommand;
        public ICommand UpdateAdapters
        {
            get { return updateAdaptersCommand; }
        }

        private RelayCommand activateAdapterCommand;
        public ICommand ActivateAdapter { get { return activateAdapterCommand; } }

        private RelayCommand deactivateAdapterCommand;
        public ICommand DeactivateAdapter { get { return deactivateAdapterCommand; } }

        private RelayCommand extractConfigToNewLocationCommand;
        public ICommand ExtractConfigToNewLocation { get { return extractConfigToNewLocationCommand; } }

        private RelayCommand applyLocationCommand;
        public ICommand ApplyLocation { get { return applyLocationCommand; } }

        private RelayCommand editLocationCommand;
        public ICommand EditLocation { get { return editLocationCommand; } }

        private RelayCommand manualSettingsCommand;
        public ICommand ManualSettings { get { return manualSettingsCommand; } }

        private RelayCommand deleteLocationCommand;
        public ICommand DeleteLocation { get { return deleteLocationCommand; } }

        private RelayCommand createLocationCommand;
        public ICommand CreateLocation { get { return createLocationCommand; } }

        private RelayCommand importPresetsCommand;
        public ICommand ImportPresets { get { return importPresetsCommand; } }

        private RelayCommand exportPresetsCommand;
        public ICommand ExportPresets { get { return exportPresetsCommand; } }

        private RelayCommand getExternalIpCommand;
        public ICommand GetExternalIp { get { return getExternalIpCommand; } }
        #endregion
    }
}