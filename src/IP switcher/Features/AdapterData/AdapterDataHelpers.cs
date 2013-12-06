using Deucalion.IP_Switcher.Helpers.ShowWindow;
using ROOT.CIMV2.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Deucalion.IP_Switcher.Features.AdapterData
{
    internal static class AdapterDataExtensions
    {
        internal static List<AdapterData> GetAdapters(bool GetOnlyPhysicalAdapters = true)
        {
            var data = new List<AdapterData>();

            var adapters = NetworkAdapter.GetInstances().Cast<NetworkAdapter>().Where(z => GetOnlyPhysicalAdapters ? z.PhysicalAdapter : true);
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var item in adapters)
            {
                var newAdapterData = new AdapterData() { networkAdapter = item, networkInterface = interfaces.FirstOrDefault(z => z.Id == item.GUID) };

                data.Add(newAdapterData);
            }
            return data;
        }

        internal static async Task ApplyLocation(this AdapterData adapter, Location.Location location)
        {
            if (location == null)
                return;
            bool result;
            if (location.DHCPEnabled)
            {
                await SetDHCP(adapter);
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

            result = await adapter.SetIP(IP, subNet, gateWay);
            if (!result)
                return;

            string[] Dns = new string[location.DNS.Count];
            for (byte b = 0; b < location.DNS.Count(); b++)
                Dns[b] = location.DNS[b].IP;
            if (!await adapter.SetDnsServers(null))
                return;
            if (!await adapter.SetDnsServers(Dns))
                return;
        }

        internal static async Task<bool> SetDHCP(this AdapterData adapter)
        {
            var adapterConfig = GetNetworkAdapter(adapter);
            if (adapterConfig != null)
            {
                var result = await adapterConfig.EnableDHCPAsync();

                if (result != 0)
                {
                    Show.Message(Resources.AdapterDataLoc.EnableDHCPFailed, string.Format(Resources.AdapterDataLoc.ErrorMessage, result, WMI.FormatMessage.GetMessage((int)result)));
                    return true;
                }
                result = await adapterConfig.RenewDHCPLeaseAsync();
                if (result != 0)
                {
                    Show.Message(Resources.AdapterDataLoc.RenewDHCPLeaseFailed, string.Format(Resources.AdapterDataLoc.ErrorMessage, result, WMI.FormatMessage.GetMessage((int)result)));
                    return true;
                }
            }

            return false;
        }

        internal static async Task<bool> SetIP(this  AdapterData adapter, string[] ipAddress, string[] subnetMask, string[] gateway)
        {
            var adapterConfig = GetNetworkAdapter(adapter);
            if (adapterConfig != null)
            {
                var result = await adapterConfig.EnableStaticAsync(ipAddress, subnetMask);
                if (result != 0)
                {
                    Show.Message(Resources.AdapterDataLoc.EnableStaticFailed, string.Format(Resources.AdapterDataLoc.ErrorMessage, result, WMI.FormatMessage.GetMessage((int)result)));
                    return false;
                }
                result = await adapterConfig.SetGatewaysAsync(gateway, new ushort[] { 1 });

                if (result != 0)
                {
                    Show.Message(Resources.AdapterDataLoc.SetGatewaysFailed, string.Format(Resources.AdapterDataLoc.ErrorMessage, result, WMI.FormatMessage.GetMessage((int)result)));
                    return false;
                }

                return true;
            }
            else
                return false;
        }

        internal static async Task<bool> SetDnsServers(this AdapterData adapter, string[] dnsServers)
        {
            var adapterConfig = GetNetworkAdapter(adapter);
            if (adapterConfig != null)
            {
                var result = await adapterConfig.SetDNSServerSearchOrderAsync(dnsServers);
                if (result != 0)
                {
                    Show.Message(Resources.AdapterDataLoc.SetDnsServersFailed, string.Format(Resources.AdapterDataLoc.ErrorMessage, result, WMI.FormatMessage.GetMessage((int)result)));
                    return false;
                }

                return true;
            }
            else
                return false;
        }

        internal static async Task<bool> Activate(this AdapterData adapter)
        {
            var couldEnable = await adapter.networkAdapter.EnableAsync();

            if (couldEnable != 0)
            {
                Show.Message(Resources.AdapterDataLoc.ActivationFailed, string.Format(Resources.AdapterDataLoc.ErrorMessage, couldEnable, WMI.FormatMessage.GetMessage((int)couldEnable)));
                return false;
            }
            return true;
        }

        internal static async Task<bool> Deactivate(this AdapterData adapter)
        {
            var couldDisable = await adapter.networkAdapter.DisableAsync();
            if (couldDisable != 0)
            {
                Show.Message(Resources.AdapterDataLoc.DeactivationFailed, string.Format(Resources.AdapterDataLoc.ErrorMessage, couldDisable, WMI.FormatMessage.GetMessage((int)couldDisable)));
                return false;
            }

            return true;
        }

        private static NetworkAdapterConfiguration GetNetworkAdapter(this AdapterData adapter)
        {
            return NetworkAdapterConfiguration.GetInstances().Cast<NetworkAdapterConfiguration>().Where(z => z.Description == adapter.networkAdapter.Description).FirstOrDefault();
        }
    }
}
