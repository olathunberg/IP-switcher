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
using TTech.IP_Switcher.Helpers.Logging;
using TTech.IP_Switcher.Helpers.ShowWindow;

namespace TTech.IP_Switcher.Features.IpSwitcher;

public class IpSwitcherViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly IAppLogger appLogger;
    private bool isDisposed;
    private bool isNetworkRefreshRunning;
    private bool hasPendingNetworkRefresh;
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
        appLogger = AppLogger.Instance;
        showOnlyPhysical = true;

        _ = ObserveCommandTaskAsync(InitializeAsync(), "Initialize adapter list and preset locations");

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
                () => _ = ObserveCommandTaskAsync(DoActivateAdapter(), "Activate adapter"),
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
                () => _ = ObserveCommandTaskAsync(DoApplyLocation(), "Apply selected location"),
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
                () => _ = ObserveCommandTaskAsync(DoDeactivateAdapter(), "Deactivate adapter"),
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
                () => _ = ObserveCommandTaskAsync(GetPublicIpAddress(), "Resolve external IP"));

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
                () => _ = ObserveCommandTaskAsync(DoManualSettings(), "Apply manual settings"),
                () => Current != null && Current.HasAdapter);

    public System.Windows.Controls.UserControl Owner
    {
        get => owner;
        set
        {
            if (owner == value)
                return;

            if (owner != null)
            {
                owner.Unloaded -= Owner_Unloaded;
            }

            owner = value;

            if (owner != null)
            {
                owner.Unloaded += Owner_Unloaded;
            }

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
                () => _ = ObserveCommandTaskAsync(DoRefreshDhcpLease(), "Refresh DHCP lease"),
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
            applyLocationCommand?.RaiseCanExecuteChanged();
        }
    }

    public bool ShowOnlyPhysical
    {
        get => showOnlyPhysical;
        set
        {
            showOnlyPhysical = value;
            updateAdaptersCommand?.Execute(null);
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
                () => _ = ObserveCommandTaskAsync(DoUpdateAdaptersListAsync(), "Update adapters list"),
                () => true);

    internal async Task DoActivateAdapter()
    {
        if (SelectedAdapter == null)
            return;

        await ExecuteWithStatusAsync(SwitcherStatus.ActivatingAdapter, () => SelectedAdapter.Activate(), "Activate adapter");
    }

    internal async Task DoDeactivateAdapter()
    {
        if (SelectedAdapter == null)
            return;

        await ExecuteWithStatusAsync(SwitcherStatus.DeactivatingAdapter, () => SelectedAdapter.Deactivate(), "Deactivate adapter");
    }

    internal async Task DoUpdateAdaptersListAsync()
    {
        await ExecuteWithStatusAsync(
            SwitcherStatus.UpdatingAdapters,
            async () =>
            {
                var adapterList = await Task.Run(() => AdapterDataExtensions.GetAdapters(ShowOnlyPhysical));
                FillAdapterLists(adapterList);
            },
            "Update adapters list");
    }

    private async Task DoApplyLocation()
    {
        if (SelectedAdapter == null || SelectedLocation == null)
            return;

        await ExecuteWithStatusAsync(
            SwitcherStatus.ApplyingLocation,
            async () =>
            {
                var location = SelectedLocation;

                if (!await SelectedAdapter.ApplyLocation(location))
                    throw new InvalidOperationException("Failed to apply location to selected adapter.");

                var networkAdapters = NetworkAdapter.GetInstances().Cast<NetworkAdapter>().ToList();
                var interfaces = NetworkInterface.GetAllNetworkInterfaces().ToList();

                if (Current != null)
                    Current.Update(SelectedAdapter, networkAdapters, interfaces);
            },
            "Apply selected location");
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

    private async Task DoManualSettings()
    {
        if (SelectedAdapter == null)
            return;

        Effect = true;
        dynamic parameters = new ExpandoObject();
        parameters.IsManualSettings = true;
        parameters.Location = SelectedAdapter.ExtractConfig(string.Empty);
        await Show.Dialog<LocationDetailView>(parameters, new Func<LocationDetailView, Task>(async (view) =>
              {
                  if (view.DialogResult ?? false)
                  {
                      Effect = false;
                      var location = (Location.Location)view.DataContext;

                      await ExecuteWithStatusAsync(
                          SwitcherStatus.ApplyingLocation,
                          async () =>
                          {
                              if (!await SelectedAdapter.ApplyLocation(location))
                                  throw new InvalidOperationException("Failed to apply manual location settings to selected adapter.");

                              Current.Update(SelectedAdapter, null, null);
                          },
                          "Apply manual settings");
                  }
              }));
        Effect = false;
    }

    private async Task DoRefreshDhcpLease()
    {
        SetStatus(SwitcherStatus.RefreshingDhcp);

        await SelectedAdapter.RenewDhcp();

        SetStatus(SwitcherStatus.Idle);
    }

    private void FillAdapterLists(List<AdapterData.AdapterData> adapterList)
    {
        adapterList ??= [];

        if (isUpdating)
        {
            hasPendingRefresh = true;
            return;
        }
        isUpdating = true;

        var networkAdapters = NetworkAdapter.GetInstances().Cast<NetworkAdapter>().ToList();
        var interfaces = NetworkInterface.GetAllNetworkInterfaces().ToList();

        Application.Current.Dispatcher.Invoke(() =>
            {
                var selected = SelectedAdapter?.GUID;
                if (Adapters == null)
                    Adapters = new ObservableCollection<AdapterData.AdapterData>(adapterList);
                else
                {
                    var itemsToRemove = Adapters
                        .Where(x => x.NetworkAdapter == null || !adapterList.Any(c => c.NetworkAdapter != null && c.NetworkAdapter.GUID == x.NetworkAdapter.GUID))
                        .ToArray();
                    foreach (var item in itemsToRemove)
                        Adapters.Remove(item);

                    var itemsToAdd = adapterList
                        .Where(x => x.NetworkAdapter != null && !Adapters.Any(c => c.NetworkAdapter != null && c.NetworkAdapter.GUID == x.NetworkAdapter.GUID))
                        .ToArray();
                    foreach (var item in itemsToAdd)
                        Adapters.Add(item);
                }

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

    private async Task GetPublicIpAddress()
    {
        IsSearchingIp = true;
        ExternalIp = IpSwitcherViewModelLoc.Searching;

        ExternalIp = await Helpers.PublicIpHelper.GetExternalIp();
        IsSearchingIp = false;
    }

    void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
    {
        try
        {
            var networkAdapters = NetworkAdapter.GetInstances().Cast<NetworkAdapter>().ToList();
            var interfaces = NetworkInterface.GetAllNetworkInterfaces().ToList();

            if (Current != null)
                Current.Update(SelectedAdapter, networkAdapters, interfaces);

            if (Adapters == null)
                return;

            foreach (var item in Adapters)
            {
                item.Update(networkAdapters, interfaces);
                item.NotifyPropertyChanged(nameof(item.NetEnabled));
            }
        }
        catch (Exception ex)
        {
            appLogger.LogError("Handle network address change", "Failed to refresh adapter details after a network address change event.", ex);
        }
    }

    async void NetworkChange_NetworkAvailabilityChanged(object sender, System.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
    {
        _ = ObserveCommandTaskAsync(HandleNetworkAvailabilityChangedAsync(), "Handle network availability change");
    }

    public void Dispose()
    {
        if (isDisposed)
            return;

        isDisposed = true;

        NetworkChange.NetworkAvailabilityChanged -= NetworkChange_NetworkAvailabilityChanged;
        NetworkChange.NetworkAddressChanged -= NetworkChange_NetworkAddressChanged;

        if (owner != null)
        {
            owner.Unloaded -= Owner_Unloaded;
        }
    }

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async Task ObserveCommandTaskAsync(Task task, string operationContext)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            HandleOperationFailure(operationContext, ex);
        }
    }

    private async Task ExecuteWithStatusAsync(SwitcherStatus activeStatus, Func<Task> operation, string operationContext)
    {
        SetStatus(activeStatus);

        try
        {
            await operation();
        }
        catch (Exception ex)
        {
            HandleOperationFailure(operationContext, ex);
        }
        finally
        {
            SetStatus(SwitcherStatus.Idle);
        }
    }

    private void HandleOperationFailure(string operationContext, Exception exception)
    {
        appLogger.LogError(operationContext, "Operation failed.", exception);

        Show.Message(IpSwitcherViewModelLoc.OperationFailed);
    }

    private async Task InitializeAsync()
    {
        await DoUpdateAdaptersListAsync();

        Locations = Settings.Default.Locations.ToList();
        SelectedLocation = Locations.FirstOrDefault();
    }

    private async Task HandleNetworkAvailabilityChangedAsync()
    {
        if (isDisposed)
            return;

        if (isNetworkRefreshRunning)
        {
            hasPendingNetworkRefresh = true;
            return;
        }

        isNetworkRefreshRunning = true;

        try
        {
            do
            {
                hasPendingNetworkRefresh = false;
                await DoUpdateAdaptersListAsync();
            }
            while (hasPendingNetworkRefresh && !isDisposed);
        }
        finally
        {
            isNetworkRefreshRunning = false;
        }
    }

    private void Owner_Unloaded(object sender, RoutedEventArgs e)
    {
        Dispose();
    }

    private void SetStatus(SwitcherStatus newStatus)
    {
        status = newStatus;
        IsWorking = newStatus != SwitcherStatus.Idle;
        IsEnabled = !IsWorking;

        NotifyPropertyChanged(nameof(StatusText));
    }
}