using ROOT.CIMV2.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using TTech.IP_Switcher.Features.About;
using TTech.IP_Switcher.Features.IpSwitcher.AdapterData;
using TTech.IP_Switcher.Features.IpSwitcher.Location;
using TTech.IP_Switcher.Features.IpSwitcher.LocationDetail;
using TTech.IP_Switcher.Features.IpSwitcher.Resources;
using TTech.IP_Switcher.Helpers.ShowWindow;

namespace TTech.IP_Switcher.Features.IpSwitcher
{
    public class IpSwitcherViewModel : INotifyPropertyChanged
    {
        private RelayCommand activateAdapterCommand;
        private ObservableCollection<AdapterData.AdapterData> adapters;
        private RelayCommand applyLocationCommand;
        private RelayCommand clearPresetsCommand;
        private RelayCommand createLocationCommand;
        private AdapterDataModel currentAdapter;
        private LocationModel currentLocation;
        private RelayCommand deactivateAdapterCommand;
        private RelayCommand deleteLocationCommand;
        private RelayCommand showAboutCommand;
        private RelayCommand editLocationCommand;
        private bool effect;
        private RelayCommand exportPresetsCommand;
        private string externalIp;
        private RelayCommand extractConfigToNewLocationCommand;
        private RelayCommand getExternalIpCommand;
        private bool hasPendingRefresh;
        private RelayCommand importPresetsCommand;
        private bool isEnabled = true;
        private bool isSearchingIp;
        private bool isUpdating;
        private bool isWorking;
        private List<Location.Location> locations;
        private RelayCommand manualSettingsCommand;
        private System.Windows.Controls.UserControl owner;
        private RelayCommand refreshDhcpLease;
        private AdapterData.AdapterData selectedAdapter;
        private Location.Location selectedLocation;
        private bool showOnlyPhysical;
        private SwitcherStatus status;
        private string title;
        private RelayCommand updateAdaptersCommand;

        [DllImport("user32.dll")]
        static extern IntPtr LoadImage(
            IntPtr hinst,
            string lpszName,
            uint uType,
            int cxDesired,
            int cyDesired,
            uint fuLoad);

        public IpSwitcherViewModel()
        {
            showOnlyPhysical = true;

            GetPublicIpAddress();

            DoUpdateAdaptersListAsync().ContinueWith(a =>
            {
                Locations = Settings.Default.Locations.ToList();
                SelectedLocation = Locations.FirstOrDefault();
            }, TaskScheduler.FromCurrentSynchronizationContext());

            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private enum SwitcherStatus
        {
            Idle,
            ActivatingAdapter,
            DeactivatingAdapter,
            ApplyingLocation,
            UpdatingAdapters,
            RefreshingDhcp
        }


        public ICommand ShowAbout => showAboutCommand ??= new RelayCommand(() =>
                {
                    Effect = true;

                    Show.Dialog<AboutView>();

                    Effect = false;
                }, () => true);


        public ICommand ActivateAdapter => activateAdapterCommand ??= new RelayCommand(
                    () => DoActivateAdapter(),
                    () => Current != null && !Current.IsActive);

        public ObservableCollection<AdapterData.AdapterData> Adapters
        {
            get => adapters;
            set
            {
                if (adapters == value)
                    return;
                adapters = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand ApplyLocation => applyLocationCommand ??= new RelayCommand(
                    () => DoApplyLocation(),
                    () => SelectedLocation != null && Current != null);

        public ICommand ClearPresets => clearPresetsCommand ??= new RelayCommand(
                    () => DoClearPresets(),
                    () => true);

        public ICommand CreateLocation => createLocationCommand ??= new RelayCommand(
                    () => DoCreateLocation(),
                    () => true);

        public AdapterDataModel Current
        {
            get => currentAdapter;
            set
            {
                currentAdapter = value;

                NotifyPropertyChanged();
            }
        }

        public LocationModel CurrentLocation
        {
            get => currentLocation;
            set
            {
                if (value == null || currentLocation == value)
                    return;
                currentLocation = value;

                NotifyPropertyChanged();
            }
        }

        public ICommand DeactivateAdapter => deactivateAdapterCommand ??= new RelayCommand(
                    () => DoDeactivateAdapter(),
                    () => Current != null && Current.IsActive);

        public ICommand DeleteLocation => deleteLocationCommand ??= new RelayCommand(
                    () => DoDeleteLocation(),
                    () => SelectedLocation != null);

        public ICommand EditLocation => editLocationCommand ??= new RelayCommand(
                    () => DoEditLocation(),
                    () => SelectedLocation != null);

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

        public ICommand ExportPresets => exportPresetsCommand ??= new RelayCommand(
                    () => DoExportPresets(),
                    () => Locations.Count > 0);

        public string ExternalIp
        {
            get => externalIp;
            set
            {
                if (externalIp == value)
                    return;
                externalIp = value;

                NotifyPropertyChanged();
            }
        }

        public ICommand ExtractConfigToNewLocation => extractConfigToNewLocationCommand ??= new RelayCommand(
                    () => DoExtractConfigToNewLocation(),
                    () => Current != null && Current.HasAdapter);

        public ICommand GetExternalIp => getExternalIpCommand ??= new RelayCommand(
                    () => GetPublicIpAddress());

        public ICommand ImportPresets => importPresetsCommand ??= new RelayCommand(
                    () => DoImportPresets(),
                    () => true);

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (isEnabled == value)
                    return;
                isEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSearchingIp
        {
            get => isSearchingIp;
            set
            {
                isSearchingIp = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsWorking
        {
            get => isWorking;
            set
            {
                if (isWorking == value)
                    return;

                isWorking = value;
                NotifyPropertyChanged();
            }
        }

        public List<Location.Location> Locations
        {
            get => locations;
            set
            {
                if (locations == value)
                    return;
                locations = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand ManualSettings => manualSettingsCommand ??= new RelayCommand(
                    () => DoManualSettings(),
                    () => Current != null && Current.HasAdapter);

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

        public static bool IsElevated => Helpers.PrivilegesHelper.IsAdministrator();

        public static BitmapSource AdministratorBadge => GetAdministratorBadge();

        private static BitmapSource GetAdministratorBadge()
        {
            var image = LoadImage(IntPtr.Zero, "#106", 1, (int)SystemParameters.SmallIconWidth, (int)SystemParameters.SmallIconHeight, 0);
            return Imaging.CreateBitmapSourceFromHIcon(image, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public ICommand RefreshDhcpLease => refreshDhcpLease ??= new RelayCommand(
                    () => DoRefreshDhcpLease(),
                    () => Current != null && Current.IsDhcpEnabled);

        public AdapterData.AdapterData SelectedAdapter
        {
            get => selectedAdapter;
            set
            {
                selectedAdapter = value;

                if (SelectedAdapter != null)
                {
                    if (Current == null)
                        Current = new AdapterDataModel(SelectedAdapter);
                    else
                        Current.Update(SelectedAdapter, null, null);
                }

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Current));
            }
        }

        public Location.Location SelectedLocation
        {
            get => selectedLocation;
            set
            {
                selectedLocation = value;
                FillLocationDetails();
                NotifyPropertyChanged();
                applyLocationCommand.RaiseCanExecuteChanged();
            }
        }

        public bool ShowOnlyPhysical
        {
            get => showOnlyPhysical;
            set
            {
                showOnlyPhysical = value;
                updateAdaptersCommand.Execute(null);
            }
        }

        public string StatusText
        {
            get
            {
                return status switch
                {
                    SwitcherStatus.Idle => IpSwitcherViewModelLoc.Status_Idle,
                    SwitcherStatus.ActivatingAdapter => IpSwitcherViewModelLoc.Status_ActivatingAdapter,
                    SwitcherStatus.DeactivatingAdapter => IpSwitcherViewModelLoc.Status_DeactivatingAdapter,
                    SwitcherStatus.ApplyingLocation => IpSwitcherViewModelLoc.Status_ApplyingLocation,
                    SwitcherStatus.UpdatingAdapters => IpSwitcherViewModelLoc.Status_UpdatingAdapters,
                    SwitcherStatus.RefreshingDhcp => IpSwitcherViewModelLoc.Status_RefreshingDhcp,
                    _ => IpSwitcherViewModelLoc.Status_None,
                };
            }
        }

        public string Title
        {
            get => title;
            set
            {
                title = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand UpdateAdapters => updateAdaptersCommand ??= new RelayCommand(
                    async () => await DoUpdateAdaptersListAsync(),
                    () => true);

        internal async void DoActivateAdapter()
        {
            SetStatus(SwitcherStatus.ActivatingAdapter);
            await SelectedAdapter.Activate();
            SetStatus(SwitcherStatus.Idle);
        }

        internal async void DoDeactivateAdapter()
        {
            SetStatus(SwitcherStatus.DeactivatingAdapter);
            await SelectedAdapter.Deactivate();
            SetStatus(SwitcherStatus.Idle);
        }

        internal async Task DoUpdateAdaptersListAsync()
        {
            SetStatus(SwitcherStatus.UpdatingAdapters);
            FillAdapterLists(await Task.Factory.StartNew(() => AdapterDataExtensions.GetAdapters(ShowOnlyPhysical)));
            SetStatus(SwitcherStatus.Idle);
        }

        private async void DoApplyLocation()
        {
            SetStatus(SwitcherStatus.ApplyingLocation);

            var location = SelectedLocation;
            await SelectedAdapter.ApplyLocation(location);
            SetStatus(SwitcherStatus.Idle);
        }

        private void DoClearPresets()
        {
            Effect = true;

            if (Show.Message(IpSwitcherViewLoc.ClearLocationsQuestion, AllowCancel: true))
            {
                Settings.Default.Locations.Clear();
                Settings.Save();
                Locations = Settings.Default.Locations.ToList();
            }
            Effect = false;
        }

        private void DoCreateLocation()
        {
            Effect = true;

            Show.Dialog<LocationDetailView>((sender) =>
                {
                    if (sender.DialogResult ?? false)
                    {
                        Settings.Default.Locations.Add((Location.Location)sender.DataContext);
                        Settings.Save();
                        Locations = Settings.Default.Locations.ToList();
                        SelectedLocation = Locations[^1];
                    }
                    Effect = false;
                });
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

        private void DoEditLocation()
        {
            Effect = true;

            dynamic parameters = new ExpandoObject();
            parameters.IsManualSettings = true;
            parameters.Location = SelectedLocation.Clone();
            Show.Dialog<LocationDetailView>(parameters, new Action<LocationDetailView>((view) =>
                  {
                      if (view.DialogResult ?? false)
                      {
                          var index = Settings.Default.Locations.IndexOf(SelectedLocation);
                          Settings.Default.Locations[index] = (Location.Location)view.DataContext;
                          Settings.Save();
                          Locations = Settings.Default.Locations;
                          SelectedLocation = Settings.Default.Locations[index];
                      }

                      Effect = false;
                  }));
        }

        private void DoExportPresets()
        {
            Effect = true;

            try
            {
                LocationExportExtension.WriteToFile(Locations);
            }
            finally
            {
                Effect = false;
            }
        }

        private void DoExtractConfigToNewLocation()
        {
            Effect = true;
            var inputBox = new InputBox.InputBoxView { Owner = Window.GetWindow(owner), WindowStartupLocation = WindowStartupLocation.CenterOwner };
            inputBox.ShowDialog();

            // If user saved, replace original
            if (inputBox.DialogResult ?? false)
            {
                Settings.Default.Locations.Add(SelectedAdapter.ExtractConfig(inputBox.Result));
                Settings.Save();
                Locations = Settings.Default.Locations.ToList();
                SelectedLocation = Locations[^1];
            }

            Effect = false;
        }

        private void DoImportPresets()
        {
            Effect = true;

            try
            {
                var importedLocations = LocationExportExtension.ReadFromFile();
                if (importedLocations != null)
                {
                    Locations = importedLocations;
                }
            }
            finally
            {
                Effect = false;
            }
        }

        private async void DoManualSettings()
        {
            Effect = true;
            dynamic parameters = new ExpandoObject();
            parameters.IsManualSettings = true;
            parameters.Location = SelectedAdapter.ExtractConfig(string.Empty);
            await Show.Dialog<LocationDetailView>(parameters, new Func<LocationDetailView, Task>(async (view) =>
                  {
                      if (view.DialogResult ?? false)
                      {
                          Effect = false;
                          SetStatus(SwitcherStatus.ApplyingLocation);

                          var location = (Location.Location)view.DataContext;

                          await SelectedAdapter.ApplyLocation(location);
                          Current.Update(SelectedAdapter, null, null);

                          SetStatus(SwitcherStatus.Idle);
                      }
                  }));
            Effect = false;
        }

        private async void DoRefreshDhcpLease()
        {
            SetStatus(SwitcherStatus.RefreshingDhcp);

            await SelectedAdapter.RenewDhcp();

            SetStatus(SwitcherStatus.Idle);
        }

        private void FillAdapterLists(List<AdapterData.AdapterData> adapterList)
        {
            if (isUpdating)
            {
                hasPendingRefresh = true;
                return;
            }
            isUpdating = true;

            Application.Current.Dispatcher.Invoke(() =>
                {
                    var selected = SelectedAdapter?.GUID;
                    if (Adapters == null)
                        Adapters = new ObservableCollection<AdapterData.AdapterData>(adapterList);
                    else
                    {
                        var itemsToRemove = Adapters.Where(x => !adapters.Any(c => x.NetworkAdapter != null && c.NetworkAdapter.GUID == x.NetworkAdapter.GUID)).ToArray();
                        foreach (var item in itemsToRemove)
                            Adapters.Remove(item);

                        var itemsToAdd = adapters.Where(x => !Adapters.Any(c => c.NetworkAdapter.GUID == x.NetworkAdapter.GUID)).ToArray();
                        foreach (var item in itemsToAdd)
                            Adapters.Add(item);
                    }
                    var networkAdapters = NetworkAdapter.GetInstances().Cast<NetworkAdapter>().ToList();
                    var interfaces = NetworkInterface.GetAllNetworkInterfaces().ToList();

                    foreach (var item in Adapters)
                    {
                        item.Update(networkAdapters, interfaces);
                        item.NotifyPropertyChanged(nameof(item.NetEnabled));
                    }

                    if (selected != null)
                        SelectedAdapter = Adapters.FirstOrDefault(x => x.GUID == selected);
                    if (SelectedAdapter == null)
                    {
                        SelectedAdapter = Adapters.FirstOrDefault(x => x.NetEnabled);
                        SelectedAdapter ??= Adapters.FirstOrDefault();
                    }
                });

            isUpdating = false;
            if (hasPendingRefresh)
            {
                hasPendingRefresh = false;
                FillAdapterLists(AdapterDataExtensions.GetAdapters(ShowOnlyPhysical));
            }
        }

        private void FillLocationDetails()
        {
            if (SelectedLocation == null)
                CurrentLocation = new LocationModel();
            else
                CurrentLocation = new LocationModel(SelectedLocation);
        }

        private async void GetPublicIpAddress()
        {
            IsSearchingIp = true;
            ExternalIp = IpSwitcherViewModelLoc.Searching;

            ExternalIp = await Helpers.PublicIpHelper.GetExternalIp();
            IsSearchingIp = false;
        }

        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            var networkAdapters = NetworkAdapter.GetInstances().Cast<NetworkAdapter>().ToList();
            var interfaces = NetworkInterface.GetAllNetworkInterfaces().ToList();

            if (Current != null)
                Current.Update(SelectedAdapter, networkAdapters, interfaces);

            foreach (var item in Adapters)
            {
                item.Update(networkAdapters, interfaces);
                item.NotifyPropertyChanged("NetEnabled");
            }
        }

        async void NetworkChange_NetworkAvailabilityChanged(object sender, System.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
        {
            await DoUpdateAdaptersListAsync();
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetStatus(SwitcherStatus newStatus)
        {
            status = newStatus;
            IsWorking = newStatus != SwitcherStatus.Idle;
            IsEnabled = !IsWorking;

            NotifyPropertyChanged(nameof(StatusText));
        }
    }
}