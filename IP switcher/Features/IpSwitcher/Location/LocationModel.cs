using System;
using System.Text;

namespace TTech.IP_Switcher.Features.IpSwitcher.Location
{
    public class LocationModel
    {
        public LocationModel() { }

        public LocationModel(Location location)
        {
            Description = location.Description;
            DHCPEnabled = ActiveTextFromBool(location.DHCPEnabled);

            var ipBuilder = new StringBuilder();
            foreach (var ip in location.IPList)
            {
                ipBuilder.AppendFormat("{0}/{1}{2}", ip.IP, ip.NetMask, Environment.NewLine);
            }
            Ip = ipBuilder.ToString().Trim();

            var dnsBuilder = new StringBuilder();
            foreach (var dns in location.DNS)
            {
                dnsBuilder.AppendLine(dns.IP);
            }
            Dns = dnsBuilder.ToString().Trim();

            var gatewayBuilder = new StringBuilder();
            foreach (var gateway in location.Gateways)
            {
                gatewayBuilder.AppendLine(gateway.IP);
            }
            Gateways = gatewayBuilder.ToString().Trim();
        }

        public string Description { get; set; }
        public string DHCPEnabled { get; set; }
        public string Dns { get; set; }
        public string Gateways { get; set; }
        public string Ip { get; set; }

        private static string ActiveTextFromBool(bool state)
        {
            if (state)
                return Resources.LocationModelLoc.Active;
            else
                return Resources.LocationModelLoc.Inactive;
        }
    }
}