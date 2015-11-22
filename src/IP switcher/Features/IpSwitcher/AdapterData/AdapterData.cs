using System;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using ROOT.CIMV2.Win32;

namespace Deucalion.IP_Switcher.Features.IpSwitcher.AdapterData
{
    public class AdapterData : INotifyPropertyChanged
    {
        private bool netEnabled;

        public NetworkAdapter networkAdapter { get; set; }
        public NetworkInterface networkInterface { get; set; }

        public bool NetEnabled
        {
            get { return netEnabled; }
            set
            {
                netEnabled = value;
                NotifyPropertyChanged();
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

    }
}