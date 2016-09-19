using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using ROOT.CIMV2.Win32;

namespace TTech.IP_Switcher.Features.IpSwitcher.AdapterData
{
    public class AdapterData : INotifyPropertyChanged
    {
        public NetworkAdapter networkAdapter { get; set; }
        public NetworkInterface networkInterface { get; set; }

        public bool NetEnabled
        {
            //   get { return networkAdapter.NetEnabled; }
            get
            {
                if (networkAdapter == null)
                    return false;
                return !(new ushort[] { 0, 4, 5, 6, 7 }.Contains(networkAdapter.NetConnectionStatus));
            }
        }

        public string Description
        {
            get
            {
                return networkAdapter.Description;
            }
        }

        public string Name
        {
            get
            {
                if (networkInterface != null)
                    return networkInterface.Name;
                else
                    return networkAdapter.Description;
            }
        }

        string GUID
        {
            get { return networkAdapter.GUID; }
        }

        public void Update(List<NetworkAdapter> adapters, List<NetworkInterface> interfaces)
        {
            if (networkAdapter != null)
                networkAdapter = adapters.FirstOrDefault(z => z.GUID == networkAdapter.GUID);
            if (networkAdapter != null)
                networkInterface = interfaces.FirstOrDefault(z => z.Id == networkAdapter.GUID);
        }

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}