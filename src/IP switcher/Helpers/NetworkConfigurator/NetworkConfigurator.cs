using System.Linq;
using ROOT.CIMV2.Win32;
using System.Threading.Tasks;
using Deucalion.IP_Switcher.Features.Location;
using Deucalion.IP_Switcher.Features.AdapterData;
using Deucalion.IP_Switcher.Helpers.ShowWindow;

namespace Deucalion.IP_Switcher.Helpers.NetworkConfigurator
{
    /// <summary>
    /// Helper class to set networking configuration like IP address, DNS servers, etc.
    /// </summary>
    internal static class NetworkConfigurator
    {
        internal static async Task ApplyLocation(Location location, AdapterData adapter)
        {
            if (location == null)
                return;
            bool result;
            if (location.DHCPEnabled)
            {
                await NetworkConfigurator.SetDHCP(adapter);
                return;
            }

            string[] IP = new string[location.IPList.Count];
            string[] subNet = new string[location.IPList.Count];
            for (byte b = 0; b < location.IPList.Count(); b++)
            {
                IP[b] = location.IPList[b].IP;
                subNet[b] = location.IPList[b].NetMask;
            }

            string[] gateWay = new string[location.Gateways.Count];
            if (gateWay.Count() == 0)
                gateWay = null;
            else
                for (byte b = 0; b < location.Gateways.Count(); b++)
                    gateWay[b] = location.Gateways[b].IP;

            result = await NetworkConfigurator.SetIP(IP, subNet, gateWay, adapter);
            if (!result)
                return;

            string[] Dns = new string[location.DNS.Count];
            for (byte b = 0; b < location.DNS.Count(); b++)
                Dns[b] = location.DNS[b].IP;
            if (!await NetworkConfigurator.SetDnsServers(null, adapter))
                return;
            if (!await NetworkConfigurator.SetDnsServers(Dns, adapter))
                return;
        }

        internal static async Task<bool> SetDHCP(AdapterData adapter)
        {
            var adapterConfig = GetNetworkAdapter(adapter);
            if (adapterConfig != null)
            {
                var result = await adapterConfig.EnableDHCPAsync();

                if (result != 0)
                {
                    Show.Message(Resources.NetworkConfiguratorLoc.EnableDHCPFailed, string.Format(Resources.NetworkConfiguratorLoc.ErrorMessage, result, WMI.FormatMessage.GetMessage((int)result)));
                    return true;
                }
                result = await adapterConfig.RenewDHCPLeaseAsync();
                if (result != 0)
                {
                    Show.Message(Resources.NetworkConfiguratorLoc.RenewDHCPLeaseFailed, string.Format(Resources.NetworkConfiguratorLoc.ErrorMessage, result, WMI.FormatMessage.GetMessage((int)result)));
                    return true;
                }
            }

            return false;
        }

        internal static async Task<bool> SetIP(string[] ipAddress, string[] subnetMask, string[] gateway, AdapterData adapter)
        {
            var adapterConfig = GetNetworkAdapter(adapter);
            if (adapterConfig != null)
            {
                var result = await adapterConfig.EnableStaticAsync(ipAddress, subnetMask);
                if (result != 0)
                {
                    Show.Message(Resources.NetworkConfiguratorLoc.EnableStaticFailed, string.Format(Resources.NetworkConfiguratorLoc.ErrorMessage, result, WMI.FormatMessage.GetMessage((int)result)));
                    return false;
                }
                result = await adapterConfig.SetGatewaysAsync(gateway, new ushort[] { 1 });

                if (result != 0)
                {
                    Show.Message(Resources.NetworkConfiguratorLoc.SetGatewaysFailed, string.Format(Resources.NetworkConfiguratorLoc.ErrorMessage, result, WMI.FormatMessage.GetMessage((int)result)));
                    return false;
                }

                return true;
            }
            else
                return false;
        }

        internal static async Task<bool> SetDnsServers(string[] dnsServers, AdapterData adapter)
        {
            var adapterConfig = GetNetworkAdapter(adapter);
            if (adapterConfig != null)
            {
                var result = await adapterConfig.SetDNSServerSearchOrderAsync(dnsServers);
                if (result != 0)
                {
                    Show.Message(Resources.NetworkConfiguratorLoc.SetDnsServersFailed, string.Format(Resources.NetworkConfiguratorLoc.ErrorMessage, result, WMI.FormatMessage.GetMessage((int)result)));
                    return false;
                }

                return true;
            }
            else
                return false;
        }

        internal static async Task<bool> Activate(AdapterData adapter)
        {
            var couldEnable = await adapter.networkAdapter.EnableAsync();

            if (couldEnable != 0)
            {
                Show.Message(Resources.NetworkConfiguratorLoc.ActivationFailed, string.Format(Resources.NetworkConfiguratorLoc.ErrorMessage, couldEnable, WMI.FormatMessage.GetMessage((int)couldEnable)));
                return false;
            }
            return true;
        }


        internal static async Task<bool> Deactivate(AdapterData adapter)
        {
            var couldDisable = await adapter.networkAdapter.DisableAsync();
            if (couldDisable != 0)
            {
                Show.Message(Resources.NetworkConfiguratorLoc.DeactivationFailed, string.Format(Resources.NetworkConfiguratorLoc.ErrorMessage, couldDisable, WMI.FormatMessage.GetMessage((int)couldDisable)));
                return false;
            }

            return true;
        }

        private static NetworkAdapterConfiguration GetNetworkAdapter(AdapterData adapter)
        {
            return NetworkAdapterConfiguration.GetInstances().Cast<NetworkAdapterConfiguration>().Where(z => z.Description == adapter.networkAdapter.Description).FirstOrDefault();
        }
    }
}
