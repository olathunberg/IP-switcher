using System.Linq;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading.Tasks;
using Deucalion.IP_Switcher.Classes;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ROOT.CIMV2.Win32;
using System.Reflection;
using System.Windows;

namespace Deucalion.IP_Switcher.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields
        private System.Windows.Window _Owner;
        private string statusText = Resources.MainViewModelLoc.Status_None;
        private bool isWorking = false;
        private AdapterData selectedActiveAdapter;
        private List<AdapterData> activeAdapters;
        private AdapterData selectedInactiveAdapter;
        private List<AdapterData> inactiveAdapters;
        private bool isEnabled = true;
        private AdapterDataModel _Current;
        private LocationModel _CurrentLocation;
        private Location selectedLocation;
        private List<Location> locations;
        private string externalIp;
        #endregion

        #region Constructors
        public MainViewModel()
        {
            var assembly = Assembly.GetExecutingAssembly().GetName();
            Title = String.Format("{0} v{1} - Ola Thunberg 2012-2013",
                    assembly.Name,
                    assembly.Version.ToString(3));

            if (!GetDotNetVersions.InstalledDotNetVersions().Any(x => x >= new Version(4, 5)))
                MessageBox.Show(Resources.MainViewModelLoc.IncorrectDotNetVersion_Message, Resources.MainViewModelLoc.IncorrectDotNetVersion_Caption, MessageBoxButton.OK, MessageBoxImage.Error);

            activateAdapterCommand = new RelayCommand(() => DoActivateAdapter(), () => Current == null ? false : !Current.IsActive);
            deactivateAdapterCommand = new RelayCommand(() => DoDeactivateAdapter(), () => Current == null ? false : Current.IsActive);
            applyLocationCommand = new RelayCommand(() => DoApplyLocation(), () => SelectedLocation != null && Current != null);
            extractConfigToNewLocationCommand = new RelayCommand(() =>
            {
                Effect = true;
                InputBox inputBox = new InputBox() { Owner = _Owner, WindowStartupLocation = WindowStartupLocation.CenterOwner };
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
            }, () => Current != null);

            editLocationCommand = new RelayCommand(() =>
                {
                    Effect = true;

                    var editLocationForm = new LocationDetailView(SelectedLocation.Clone()) { Owner = _Owner, WindowStartupLocation = WindowStartupLocation.CenterOwner };
                    editLocationForm.ShowDialog();

                    if (editLocationForm.DialogResult ?? false)
                    {
                        var index = Settings.Default.Locations.IndexOf(SelectedLocation);
                        Settings.Default.Locations[index] = (Location)editLocationForm.DataContext;
                        Settings.Save();

                        SelectedLocation = Settings.Default.Locations[index];
                    }

                    Effect = false;
                }, () => SelectedLocation != null);

            deleteLocationCommand = new RelayCommand(() =>
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
                }, () => SelectedLocation != null);

            manualSettingsCommand = new RelayCommand(async () =>
                {
                    Effect = true;
                    var editLocationForm = new LocationDetailView(ExtractConfig(GetSelectedAdapter(), string.Empty), true) { Owner = _Owner, WindowStartupLocation = WindowStartupLocation.CenterOwner };
                    editLocationForm.ShowDialog();
                    Effect = false;

                    if (editLocationForm.DialogResult ?? false)
                    {
                        SetStatus(Status.ApplyingLocation);

                        var adapter = GetSelectedAdapter();
                        var location = (Location)editLocationForm.DataContext;

                        await NetworkConfigurator.ApplyLocation(location, adapter);
                        await DoUpdateAdaptersListAsync();

                        SetStatus(Status.Idle);
                    }
                }, () => Current != null);

            createLocationCommand = new RelayCommand(() =>
                {
                    Effect = true;

                    LocationDetailView editLocationForm = new LocationDetailView(new Location()) { Owner = _Owner, WindowStartupLocation = WindowStartupLocation.CenterOwner };
                    editLocationForm.ShowDialog();
                    if (editLocationForm.DialogResult ?? false)
                    {
                        Settings.Default.Locations.Add((Location)editLocationForm.DataContext);
                        Settings.Save();
                        Locations = Settings.Default.Locations.ToList();
                        SelectedLocation = Locations.Last();
                    }

                    Effect = false;
                }, () => SelectedLocation != null);

            var t1 = DoUpdateAdaptersListAsync();

            Locations = Settings.Default.Locations.ToList();
            SelectedLocation = Locations.FirstOrDefault();

            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
        }

        async void NetworkChange_NetworkAvailabilityChanged(object sender, System.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
        {
            await DoUpdateAdaptersListAsync();
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

        public AdapterData SelectedActiveAdapter
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
                            Current = new AdapterDataModel(value);
                        });
                }

                NotifyPropertyChanged();
            }
        }

        public List<AdapterData> ActiveAdapters
        {
            get { return activeAdapters; }
            set
            {
                if (activeAdapters == value)
                    return;
                activeAdapters = value; NotifyPropertyChanged();
            }
        }

        public AdapterData SelectedInactiveAdapter
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
                            Current = new AdapterDataModel(value);
                        });
                }

                NotifyPropertyChanged();
            }
        }

        public List<AdapterData> InactiveAdapters
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

        public AdapterDataModel Current
        {
            get { return _Current; }
            set
            {
                if (value == null || _Current == value)
                    return;
                _Current = value;

                GetPublicIpAddress();
                NotifyPropertyChanged();
                activateAdapterCommand.RaiseCanExecuteChanged();
                deactivateAdapterCommand.RaiseCanExecuteChanged();
                extractConfigToNewLocationCommand.RaiseCanExecuteChanged();
                applyLocationCommand.RaiseCanExecuteChanged();
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

        public List<Location> Locations
        {
            get { return locations; }
            set
            {
                if (locations == value)
                    return;
                locations = value; NotifyPropertyChanged();
            }
        }

        public Location SelectedLocation
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

        public bool CanSaveAs { get { return Current == null ? false : Current.HasAdapter; } }

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

        public System.Windows.Window Owner
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

        #region Methods

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
        #endregion

        #region Commands
        public ICommand UpdateAdapters
        {
            get
            {
                return new RelayCommand(async () =>
                    {
                        await DoUpdateAdaptersListAsync();
                    }, () => true);
            }
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
        #endregion

        private async void DoApplyLocation()
        {
            SetStatus(Status.ApplyingLocation);

            var adapter = GetSelectedAdapter();
            var location = SelectedLocation;
            await NetworkConfigurator.ApplyLocation(location, adapter);
            await DoUpdateAdaptersListAsync();

            SetStatus(Status.Idle);
        }

        private async void GetPublicIpAddress()
        {
            ExternalIp = Resources.MainViewModelLoc.Searching;
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
                publicIPAddress = Resources.MainViewModelLoc.SearchFailed;
            }

            ExternalIp = publicIPAddress.Replace("\n", "");
        }

        private AdapterData GetSelectedAdapter()
        {
            if (SelectedActiveAdapter != null)
                return SelectedActiveAdapter;
            if (SelectedInactiveAdapter != null)
                return SelectedInactiveAdapter;

            return null;
        }

        internal static Location ExtractConfig(AdapterData adapter, string NewName)
        {
            Location location = new Location() { Description = Resources.MainViewModelLoc.NewLocationDescription, ID = Settings.Default.GetNextID() };

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

        internal void FillLocationDetails()
        {
            if (SelectedLocation == null)
                CurrentLocation = new LocationModel();
            else
                CurrentLocation = new LocationModel(SelectedLocation);
        }

        private void FillAdapterLists(List<AdapterData> adapterList)
        {
            // Remember selected adapter
            var tmpAdapter = GetSelectedAdapter();
            string selectedAdapter = tmpAdapter != null ? tmpAdapter.networkAdapter.GUID : string.Empty;

            // Update list with current adapters
            var activeAdapters = new List<AdapterData>();
            var inactiveAdapters = new List<AdapterData>();
            foreach (AdapterData item in adapterList)
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
                    StatusText = Resources.MainViewModelLoc.Status_Idle;
                    IsWorking = false;
                    break;
                case Status.ActivatingAdapter:
                    StatusText = Resources.MainViewModelLoc.Status_ActivatingAdapter;
                    IsWorking = true;
                    break;
                case Status.DeactivatingAdapter:
                    StatusText = Resources.MainViewModelLoc.Status_DeactivatingAdapter;
                    IsWorking = true;
                    break;
                case Status.ApplyingLocation:
                    StatusText = Resources.MainViewModelLoc.Status_ApplyingLocation;
                    IsWorking = true;
                    break;
                case Status.UpdatingAdapters:
                    StatusText = Resources.MainViewModelLoc.Status_UpdatingAdapters;
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
            await adapter.networkAdapter.EnableAsync();

            SetStatus(Status.Idle);
        }

        internal async void DoDeactivateAdapter()
        {
            SetStatus(Status.DeactivatingAdapter);

            await GetSelectedAdapter().networkAdapter.DisableAsync();
            FillAdapterLists(AdapterDataHelpers.GetAdapters());

            SetStatus(Status.Idle);
        }

        internal void DoUpdateAdaptersList()
        {
            SetStatus(Status.UpdatingAdapters);

            FillAdapterLists(AdapterDataHelpers.GetAdapters());

            SetStatus(Status.Idle);
        }

        internal async Task DoUpdateAdaptersListAsync()
        {
            SetStatus(Status.UpdatingAdapters);

            FillAdapterLists(await Task.Factory.StartNew(() => AdapterDataHelpers.GetAdapters()));

            SetStatus(Status.Idle);
        }
    }
}
