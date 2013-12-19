using Deucalion.IP_Switcher.Features.AdapterData;
using Deucalion.IP_Switcher.Features.IpSwitcher.Resources;
using Deucalion.IP_Switcher.Features.Location;
using Deucalion.IP_Switcher.Features.LocationDetail;
using Deucalion.IP_Switcher.Helpers.ShowWindow;
using System;
using System.Collections.Generic;
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
        private string statusText = IpSwitcherViewModelLoc.Status_None;
        private bool isWorking = false;
        private AdapterData.AdapterData selectedActiveAdapter;
        private List<AdapterData.AdapterData> activeAdapters;
        private AdapterData.AdapterData selectedInactiveAdapter;
        private List<AdapterData.AdapterData> inactiveAdapters;
        private bool isEnabled = true;
        private AdapterData.AdapterDataModel currentAdapter;
        private LocationModel currentLocation;
        private Location.Location selectedLocation;
        private List<Location.Location> locations;
        private string externalIp;
        private string title;
        private bool showOnlyPhysical = false;
        private bool effect;
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

            ShowOnlyPhysical = true;

            var tmpTask = DoUpdateAdaptersListAsync();

            Locations = Settings.Default.Locations.ToList();
            SelectedLocation = Locations.FirstOrDefault();

            GetPublicIpAddress();

            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
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
                            Current = new AdapterData.AdapterDataModel(SelectedActiveAdapter);
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
                            Current = new AdapterData.AdapterDataModel(SelectedInactiveAdapter);
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
            get { return currentAdapter; }
            set
            {
                if (value == null || currentAdapter == value)
                    return;
                currentAdapter = value;

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
                DoUpdateAdaptersListAsync();
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
            parameters.Location = GetSelectedAdapter().ExtractConfig(string.Empty);
            await Show.Dialog<LocationDetailView>(parameters, new Func<LocationDetailView, Task>(async (view) =>
                  {
                      Effect = false;

                      if (view.DialogResult ?? false)
                      {
                          SetStatus(Status.ApplyingLocation);

                          var adapter = GetSelectedAdapter();
                          var location = (Location.Location)view.DataContext;

                          await adapter.ApplyLocation(location);
                          await DoUpdateAdaptersListAsync();

                          SetStatus(Status.Idle);
                      }
                  }));
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
            Effect = true;
            SetStatus(Status.ApplyingLocation);

            var adapter = GetSelectedAdapter();
            var location = SelectedLocation;
            await adapter.ApplyLocation(location);
            await DoUpdateAdaptersListAsync();

            SetStatus(Status.Idle);
            Effect = false;
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
                Settings.Default.Locations.Add(GetSelectedAdapter().ExtractConfig(inputBox.Result));
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
            SetStatus(Status.ActivatingAdapter);
            await GetSelectedAdapter().Activate();
            SetStatus(Status.Idle);
        }

        internal async void DoDeactivateAdapter()
        {
            SetStatus(Status.DeactivatingAdapter);
            await GetSelectedAdapter().Deactivate();
            SetStatus(Status.Idle);
        }

        internal async Task DoUpdateAdaptersListAsync()
        {
            SetStatus(Status.UpdatingAdapters);
            FillAdapterLists(await Task.Factory.StartNew(() => AdapterDataExtensions.GetAdapters(ShowOnlyPhysical)));
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

        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            // TODO: Implement addressupdate
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