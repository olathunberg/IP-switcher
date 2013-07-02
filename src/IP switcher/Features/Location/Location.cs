using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Deucalion.IP_Switcher.Features.Location
{
    public class Location
    {
        private string _Description = string.Empty;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private uint _ID = 0;
        public uint ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private bool _DHCPEnabled = false;
        public bool DHCPEnabled
        {
            get { return _DHCPEnabled; }
            set { _DHCPEnabled = value; }
        }

        private ObservableCollection<IPDefinition> _IPList = new ObservableCollection<IPDefinition>();
        public ObservableCollection<IPDefinition> IPList
        {
            get { return _IPList; }
            set { _IPList = value; }
        }

        private ObservableCollection<IPv4Address> _Gateways = new ObservableCollection<IPv4Address>();
        public ObservableCollection<IPv4Address> Gateways
        {
            get { return _Gateways; }
            set { _Gateways = value; }
        }

        private ObservableCollection<IPv4Address> _DNS = new ObservableCollection<IPv4Address>();
        public ObservableCollection<IPv4Address> DNS
        {
            get { return _DNS; }
            set { _DNS = value; }
        }

        public Location Clone()
        {
            return (Location)this.MemberwiseClone();
        }
    }
}