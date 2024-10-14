using System.Collections.ObjectModel;

namespace TTech.IP_Switcher.Features.IpSwitcher.Location
{
    public class Location
    {
        public string Description { get; set; } = string.Empty;

        public uint ID { get; set; }

        public bool DHCPEnabled { get; set; }

        public ObservableCollection<IPDefinition> IPList { get; set; } = [];

        public ObservableCollection<IPv4Address> Gateways { get; set; } = [];

        public ObservableCollection<IPv4Address> DNS { get; set; } = [];

        public Location Clone() => (Location)this.MemberwiseClone();
    }
}