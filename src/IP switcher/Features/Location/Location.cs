using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Deucalion.IP_Switcher.Features.Location
{
    public class Location
    {
        #region Fields
        private string _Description = string.Empty;
        private uint _ID = 0;
        private bool _DHCPEnabled = false;
        private ObservableCollection<IPDefinition> _IPList = new ObservableCollection<IPDefinition>();
        private ObservableCollection<IPv4Address> _Gateways = new ObservableCollection<IPv4Address>();
        private ObservableCollection<IPv4Address> _DNS = new ObservableCollection<IPv4Address>();
        #endregion

        #region Properties
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public uint ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public bool DHCPEnabled
        {
            get { return _DHCPEnabled; }
            set { _DHCPEnabled = value; }
        }

        public ObservableCollection<IPDefinition> IPList
        {
            get { return _IPList; }
            set { _IPList = value; }
        }

        public ObservableCollection<IPv4Address> Gateways
        {
            get { return _Gateways; }
            set { _Gateways = value; }
        }

        public ObservableCollection<IPv4Address> DNS
        {
            get { return _DNS; }
            set { _DNS = value; }
        } 
        #endregion

        public Location Clone()
        {
            return (Location)this.MemberwiseClone();
        }
    }
}