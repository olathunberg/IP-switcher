using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using ROOT.CIMV2.Win32;

namespace TTech.IP_Switcher.Features.IpSwitcher.AdapterData;

public class AdapterData : INotifyPropertyChanged
{
    public NetworkAdapter NetworkAdapter { get; set; }
    public NetworkInterface NetworkInterface { get; set; }

    public bool NetEnabled
    {
        get
        {
            if (NetworkAdapter == null)
                return false;
            return !(new ushort[] { 0, 4, 5, 6, 7 }.Contains(NetworkAdapter.NetConnectionStatus));
        }
    }

    public string Description => NetworkAdapter.Description;

    public string Name
    {
        get
        {
            if (NetworkInterface != null)
                return NetworkInterface.Name;
            else
                return NetworkAdapter.Description;
        }
    }

    public string GUID => NetworkAdapter.GUID;

    public void Update(List<NetworkAdapter> adapters, List<NetworkInterface> interfaces)
    {
        if (NetworkAdapter != null)
            NetworkAdapter = adapters.FirstOrDefault(z => z.GUID == NetworkAdapter.GUID);
        if (NetworkAdapter != null)
            NetworkInterface = interfaces.FirstOrDefault(z => z.Id == NetworkAdapter.GUID);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}