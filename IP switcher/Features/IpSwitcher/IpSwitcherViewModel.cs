using TTech.IP_Switcher.Features.IpSwitcher.AdapterData;
using TTech.IP_Switcher.Features.IpSwitcher.Location;
using TTech.IP_Switcher.Features.IpSwitcher.LocationDetail;
using TTech.IP_Switcher.Features.IpSwitcher.Resources;
using TTech.IP_Switcher.Helpers.ShowWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics.CodeAnalysis;
using ROOT.CIMV2.Win32;
using System.Net.NetworkInformation;

namespace TTech.IP_Switcher.Features.IpSwitcher
{
    public class IpSwitcherViewModel : INotifyPropertyChanged
    {
        #region Fields
        private System.Windows.Controls.UserControl owner;
        private SwitcherStatus status;
        private bool isWorking;
        private AdapterData.AdapterData selectedAdapter;
        private ObservableCollection<AdapterData.AdapterData> adapters;
        private bool isEnabled = true;
        private AdapterDataModel currentAdapter;
        private LocationModel currentLocation;
        private Location.Location selectedLocation;
        private List<Location.Location> locations;
        private string externalIp;
        private string title;
        private bool showOnlyPhysical;
        private bool effect;
        private bool hasPendingRefresh;
        private bool isUpdating;
        private bool isSearchingIp;
        #endregion

        #region Constructors
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
        #endregion

        #region Public Properties
        private void SetStatus(SwitcherStatus newStatus)
        {
            status = newStatus;
            IsWorking = newStatus != SwitcherStatus.Idle;
            IsEnabled = !IsWorking;

            NotifyPropertyChanged("StatusText");
        }

        public bool IsSearchingIp
        {
            get { return isSearchingIp; }
            set
            {
                isSearchingIp = value;
                NotifyPropertyChanged();
            }
        }

        public string StatusText
        {
            get
            {
                switch (status)
                {
                    case SwitcherStatus.Idle:
                        return IpSwitcherViewModelLoc.Status_Idle;
                    case SwitcherStatus.ActivatingAdapter:
                        return IpSwitcherViewModelLoc.Status_ActivatingAdapter;
                    case SwitcherStatus.DeactivatingAdapter:
                        return IpSwitcherViewModelLoc.Status_DeactivatingAdapter;
                    case SwitcherStatus.ApplyingLocation:
                        return IpSwitcherViewModelLoc.Status_ApplyingLocation;
                    case SwitcherStatus.UpdatingAdapters:
                        return IpSwitcherViewModelLoc.Status_UpdatingAdapters;
                    case SwitcherStatus.RefreshingDhcp:
                        return IpSwitcherViewModelLoc.Status_RefreshingDhcp;
                    default:
                        return IpSwitcherViewModelLoc.Status_None;
                }
            }
        }

        public bool IsWorking
        {
            get { return isWorking; }
            set
            {
                if (isWorking == value)
                    return;

                isWorking = value;
                NotifyPropertyChanged();
            }
        }

        public AdapterData.AdapterData SelectedAdapter
        {
            get { return selectedAdapter; }
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
            }
        }

        public ObservableCollection<AdapterData.AdapterData> Adapters
        {
            get { return adapters; }
            set
            {
                if (adapters == value)
                    return;
                adapters = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (isEnabled == value)
                    return;
                isEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public AdapterData.AdapterDataModel Current
        {
            get { return currentAdapter; }
            set
            {
                currentAdapter = value;

                NotifyPropertyChanged();
            }
        }

        public LocationModel CurrentLocation
        {
            get { return currentLocation; }
            set
            {
                if (value == null || currentLocation == value)
                    return;
                currentLocation = value;

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

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                NotifyPropertyChanged();
            }
        }

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
            get { return owner; }
            set
            {
                if (owner == value)
                    return;

                owner = value;

                NotifyPropertyChanged();
            }
        }

        public bool ShowOnlyPhysical
        {
            get { return showOnlyPhysical; }
            set
            {
                showOnlyPhysical = value;
                updateAdaptersCommand.Execute(null);
            }
        }

        #endregion

        #region Private / Protected
        private enum SwitcherStatus
        {
            Idle,
            ActivatingAdapter,
            DeactivatingAdapter,
            ApplyingLocation,
            UpdatingAdapters,
            RefreshingDhcp
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
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

        private void DoImportPresets()
        {
            Effect = true;

            try
            {
                Locations = LocationExportExtension.ReadFromFile();
            }
            finally
            {
                Effect = false;
            }
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
                        SelectedLocation = Locations.Last();
                    }
                    Effect = false;
                });

        }

        [SuppressMessage("Potential Code Quality Issues", "RECS0165:Asynchronous methods should return a Task instead of void", Justification = "Eventhandler")]
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

        [SuppressMessage("Potential Code Quality Issues", "RECS0165:Asynchronous methods should return a Task instead of void", Justification = "Eventhandler")]
        private async void DoApplyLocation()
        {
            SetStatus(SwitcherStatus.ApplyingLocation);

            var location = SelectedLocation;
            await SelectedAdapter.ApplyLocation(location);
            SetStatus(SwitcherStatus.Idle);
        }

        [SuppressMessage("Potential Code Quality Issues", "RECS0165:Asynchronous methods should return a Task instead of void", Justification = "Eventhandler")]
        private async void DoRefreshDhcpLease()
        {
            SetStatus(SwitcherStatus.RefreshingDhcp);

            await SelectedAdapter.RenewDhcp();

            SetStatus(SwitcherStatus.Idle);
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
                SelectedLocation = Locations.Last();
            }

            Effect = false;
        }

        [SuppressMessage("Potential Code Quality Issues", "RECS0165:Asynchronous methods should return a Task instead of void", Justification = "Eventhandler")]
        private async void GetPublicIpAddress()
        {
            IsSearchingIp = true;
            ExternalIp = IpSwitcherViewModelLoc.Searching;
            var request = WebRequest.Create("http://ifconfig.me") as HttpWebRequest;

            request.UserAgent = "curl"; // this simulate curl linux command

            string publicIPAddress;

            request.Method = "GET";

            try
            {
                using (var response = await request.GetResponseAsync())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    publicIPAddress = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                publicIPAddress = IpSwitcherViewModelLoc.SearchFailed;
            }

            if (!ValidateStringAsIpAddress(publicIPAddress))
                ExternalIp = IpSwitcherViewModelLoc.SearchFailed;
            else
                ExternalIp = publicIPAddress.Replace("\n", "");

            IsSearchingIp = false;
        }

        private bool ValidateStringAsIpAddress(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            if (value.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Length == 4)
            {
                IPAddress ipAddr;
                if (IPAddress.TryParse(value, out ipAddr))
                    return true;
            }

            return false;
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
            if (isUpdating)
            {
                hasPendingRefresh = true;
                return;
            }
            isUpdating = true;

            Application.Current.Dispatcher.Invoke(() =>
                {
                    if (Adapters == null)
                        Adapters = new ObservableCollection<AdapterData.AdapterData>(adapterList);
                    else
                    {
                        var itemsToRemove = Adapters.Where(x => !adapters.Any(c => x.networkAdapter != null && c.networkAdapter.GUID == x.networkAdapter.GUID)).ToArray();
                        foreach (var item in itemsToRemove)
                            Adapters.Remove(item);

                        var itemsToAdd = adapters.Where(x => !Adapters.Any(c => c.networkAdapter.GUID == x.networkAdapter.GUID)).ToArray();
                        foreach (var item in itemsToAdd)
                            Adapters.Add(item);
                    }
                    var networkAdapters = NetworkAdapter.GetInstances().Cast<NetworkAdapter>().ToList();
                    var interfaces = NetworkInterface.GetAllNetworkInterfaces().ToList();

                    foreach (var item in Adapters)
                    {
                        item.Update(networkAdapters, interfaces);
                        item.NotifyPropertyChanged("NetEnabled");
                    }

                    if (SelectedAdapter == null)
                    {
                        SelectedAdapter = Adapters.FirstOrDefault(x => x.NetEnabled);
                        if (SelectedAdapter == null)
                            SelectedAdapter = Adapters.FirstOrDefault();
                    }
                });

            isUpdating = false;
            if (hasPendingRefresh)
            {
                hasPendingRefresh = false;
                FillAdapterLists(AdapterDataExtensions.GetAdapters(ShowOnlyPhysical));
            }
        }

        [SuppressMessage("Potential Code Quality Issues", "RECS0165:Asynchronous methods should return a Task instead of void", Justification = "Eventhandler")]
        internal async void DoActivateAdapter()
        {
            SetStatus(SwitcherStatus.ActivatingAdapter);
            await SelectedAdapter.Activate();
            SetStatus(SwitcherStatus.Idle);
        }

        [SuppressMessage("Potential Code Quality Issues", "RECS0165:Asynchronous methods should return a Task instead of void", Justification = "Eventhandler")]
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
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
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
        #endregion

        #region Commands
        private RelayCommand updateAdaptersCommand;
        [SuppressMessage("Potential Code Quality Issues", "RECS0165:Asynchronous methods should return a Task instead of void", Justification = "Eventhandler")]
        public ICommand UpdateAdapters
        {
            get
            {
                return updateAdaptersCommand ?? (updateAdaptersCommand = new RelayCommand(
                    async () => await DoUpdateAdaptersListAsync(),
                    () => true));
            }
        }

        private RelayCommand activateAdapterCommand;
        public ICommand ActivateAdapter
        {
            get
            {
                return activateAdapterCommand ?? (activateAdapterCommand = new RelayCommand(
                    () => DoActivateAdapter(),
                    () => Current != null && !Current.IsActive));
            }
        }

        private RelayCommand deactivateAdapterCommand;
        public ICommand DeactivateAdapter
        {
            get
            {
                return deactivateAdapterCommand ?? (deactivateAdapterCommand = new RelayCommand(
                    () => DoDeactivateAdapter(),
                    () => Current != null && Current.IsActive));
            }
        }

        private RelayCommand extractConfigToNewLocationCommand;
        public ICommand ExtractConfigToNewLocation
        {
            get
            {
                return extractConfigToNewLocationCommand ?? (extractConfigToNewLocationCommand = new RelayCommand(
                    () => DoExtractConfigToNewLocation(),
                    () => Current != null && Current.HasAdapter));
            }
        }

        private RelayCommand applyLocationCommand;
        public ICommand ApplyLocation
        {
            get
            {
                return applyLocationCommand ?? (applyLocationCommand = new RelayCommand(
                    () => DoApplyLocation(),
                    () => SelectedLocation != null && Current != null));
            }
        }

        private RelayCommand editLocationCommand;
        public ICommand EditLocation
        {
            get
            {
                return editLocationCommand ?? (editLocationCommand = new RelayCommand(
                    () => DoEditLocation(),
                    () => SelectedLocation != null));
            }
        }

        private RelayCommand manualSettingsCommand;
        public ICommand ManualSettings
        {
            get
            {
                return manualSettingsCommand ?? (manualSettingsCommand = new RelayCommand(
                    () => DoManualSettings(),
                    () => Current != null && Current.HasAdapter));
            }
        }

        private RelayCommand deleteLocationCommand;
        public ICommand DeleteLocation
        {
            get
            {
                return deleteLocationCommand ?? (deleteLocationCommand = new RelayCommand(
                    () => DoDeleteLocation(),
                    () => SelectedLocation != null));
            }
        }

        private RelayCommand createLocationCommand;
        public ICommand CreateLocation
        {
            get
            {
                return createLocationCommand ?? (createLocationCommand = new RelayCommand(
                    () => DoCreateLocation(),
                    () => true));
            }
        }

        private RelayCommand importPresetsCommand;
        public ICommand ImportPresets
        {
            get
            {
                return importPresetsCommand ?? (importPresetsCommand = new RelayCommand(
                    () => DoImportPresets(),
                    () => true));
            }
        }

        private RelayCommand exportPresetsCommand;
        public ICommand ExportPresets
        {
            get
            {
                return exportPresetsCommand ?? (exportPresetsCommand = new RelayCommand(
                    () => DoExportPresets(),
                    () => Locations.Count > 0));
            }
        }

        private RelayCommand getExternalIpCommand;
        public ICommand GetExternalIp
        {
            get
            {
                return getExternalIpCommand ?? (getExternalIpCommand = new RelayCommand(
                    () => GetPublicIpAddress()));
            }
        }

        private RelayCommand refreshDhcpLease;
        public ICommand RefreshDhcpLease
        {
            get
            {
                return refreshDhcpLease ?? (refreshDhcpLease = new RelayCommand(
                    () => DoRefreshDhcpLease(),
                    () => Current != null && Current.IsDhcpEnabled));
            }
        }
        #endregion
    }
}