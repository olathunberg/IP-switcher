using Deucalion.IP_Switcher.Features.IpSwitcher.AdapterData;
using Deucalion.IP_Switcher.Features.IpSwitcher.Location;
using Deucalion.IP_Switcher.Features.IpSwitcher.LocationDetail;
using Deucalion.IP_Switcher.Features.IpSwitcher.Resources;
using Deucalion.IP_Switcher.Helpers.ShowWindow;
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

namespace Deucalion.IP_Switcher.Features.IpSwitcher
{
    public class IpSwitcherViewModel : INotifyPropertyChanged
    {
        #region Fields
        private System.Windows.Controls.UserControl owner;
        private SwitcherStatus status;
        private bool isWorking = false;
        private AdapterData.AdapterData selectedAdapter;
        private ObservableCollection<AdapterData.AdapterData> adapters;
        private bool isEnabled = true;
        private AdapterData.AdapterDataModel currentAdapter;
        private LocationModel currentLocation;
        private Location.Location selectedLocation;
        private List<Location.Location> locations;
        private string externalIp;
        private string title;
        private bool showOnlyPhysical = false;
        private bool effect;
        private bool hasPendingRefresh = false;
        private bool isUpdating = false;
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
            manualSettingsCommand = new RelayCommand(() => DoManualSettings(),
                () => Current != null && Current.HasAdapter);
            createLocationCommand = new RelayCommand(() => DoCreateLocation(),
                () => true);
            importPresetsCommand = new RelayCommand(() => DoImportPresets(),
                () => true);
            exportPresetsCommand = new RelayCommand(() => DoExportPresets(),
                () => Locations.Count > 0);

            getExternalIpCommand = new RelayCommand(() => GetPublicIpAddress());

            refreshDhcpLease = new RelayCommand(() => DoRefreshDhcpLease(),
                () => Current == null ? false : Current.IsDhcpEnabled);

            showOnlyPhysical = true;

            var tmpTask = Task.Factory.StartNew(async () =>
                {
                    await DoUpdateAdaptersListAsync();

                    Locations = Settings.Default.Locations.ToList();
                    SelectedLocation = Locations.FirstOrDefault();
                });
            GetPublicIpAddress();

            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
        }
        #endregion

        #region Public Properties
        private SwitcherStatus Status
        {
            set
            {
                status = value;
                IsWorking = value != SwitcherStatus.Idle;
                IsEnabled = !IsWorking;

                NotifyPropertyChanged("StatusText");
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
                isWorking = value; NotifyPropertyChanged();
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
                        Current.Update(SelectedAdapter);
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
                adapters = value; NotifyPropertyChanged();
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
            get { return currentAdapter; }
            set
            {
                currentAdapter = value;

                NotifyPropertyChanged();
                Application.Current.Dispatcher.Invoke(() =>
                    {
                        activateAdapterCommand.RaiseCanExecuteChanged();
                        deactivateAdapterCommand.RaiseCanExecuteChanged();
                        extractConfigToNewLocationCommand.RaiseCanExecuteChanged();
                        applyLocationCommand.RaiseCanExecuteChanged();
                        manualSettingsCommand.RaiseCanExecuteChanged();
                    });
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

        public string Title { get { return title; } set { title = value; NotifyPropertyChanged(); } }

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
                          Status = SwitcherStatus.ApplyingLocation;

                          var location = (Location.Location)view.DataContext;

                          await SelectedAdapter.ApplyLocation(location);
                          Current.Update(SelectedAdapter);

                          Status = SwitcherStatus.Idle;
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

        private async void DoApplyLocation()
        {
            Status = SwitcherStatus.ApplyingLocation;

            var location = SelectedLocation;
            await SelectedAdapter.ApplyLocation(location);
            Status = SwitcherStatus.Idle;
        }

        private async void DoRefreshDhcpLease()
        {
            Status = SwitcherStatus.RefreshingDhcp;

            await SelectedAdapter.RenewDhcp();

            Status = SwitcherStatus.Idle;
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
            var inputBox = new Deucalion.IP_Switcher.Features.InputBox.InputBoxView() { Owner = Window.GetWindow(owner), WindowStartupLocation = WindowStartupLocation.CenterOwner };
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
                        var itemsToRemove = Adapters.Where(x => !adapters.Any(c => c.networkAdapter.GUID == x.networkAdapter.GUID)).ToArray();
                        foreach (var item in itemsToRemove)
                            Adapters.Remove(item);

                        var itemsToAdd = adapters.Where(x => !Adapters.Any(c => c.networkAdapter.GUID == x.networkAdapter.GUID)).ToArray();
                        foreach (var item in itemsToAdd)
                            Adapters.Add(item);
                    }
                    foreach (var item in Adapters)
                        item.NetEnabled = item.networkAdapter.NetEnabled;

                    if (SelectedAdapter == null)
                    {
                        SelectedAdapter = Adapters.Where(x => x.NetEnabled).FirstOrDefault();
                        if (SelectedAdapter == null)
                            SelectedAdapter = Adapters.FirstOrDefault();
                    }
                    else
                        SelectedAdapter = SelectedAdapter;
                });

            isUpdating = false;
            if (hasPendingRefresh)
            {
                hasPendingRefresh = false;
                FillAdapterLists(AdapterDataExtensions.GetAdapters(ShowOnlyPhysical));
            }
        }

        internal async void DoActivateAdapter()
        {
            Status = SwitcherStatus.ActivatingAdapter;
            await SelectedAdapter.Activate();
            Status = SwitcherStatus.Idle;
        }

        internal async void DoDeactivateAdapter()
        {
            Status = SwitcherStatus.DeactivatingAdapter;
            await SelectedAdapter.Deactivate();
            Status = SwitcherStatus.Idle;
        }

        internal async Task DoUpdateAdaptersListAsync()
        {
            Status = SwitcherStatus.UpdatingAdapters;
            FillAdapterLists(await Task.Factory.StartNew(() => AdapterDataExtensions.GetAdapters(ShowOnlyPhysical)));
            Status = SwitcherStatus.Idle;
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

        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            Current.Update(SelectedAdapter);
            foreach (var item in Adapters)
                item.NetEnabled = item.networkAdapter.NetEnabled;
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

        private RelayCommand refreshDhcpLease;
        public ICommand RefreshDhcpLease { get { return refreshDhcpLease; } }
        #endregion
    }
}