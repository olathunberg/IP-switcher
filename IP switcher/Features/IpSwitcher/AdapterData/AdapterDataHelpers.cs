using TTech.IP_Switcher.Features.IpSwitcher.AdapterData.Resources;
using TTech.IP_Switcher.Features.IpSwitcher.Location;
using TTech.IP_Switcher.Helpers.ShowWindow;
using ROOT.CIMV2.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System;

namespace TTech.IP_Switcher.Features.IpSwitcher.AdapterData;

internal static class AdapterDataExtensions
{
    internal static Location.Location ExtractConfig(this AdapterData adapter, string NewName)
    {
        var location = new Location.Location { Description = AdapterDataLoc.NewLocationDescription, ID = Settings.Default.GetNextID() };
        if (adapter.NetworkInterface == null)
            return location;

        var properties = adapter.NetworkInterface.GetIPProperties();

        // DHCP Enabled:
        location.DHCPEnabled = properties.GetIPv4Properties().IsDhcpEnabled;

        location.IPList.Clear();
        foreach (var uniCast in properties.UnicastAddresses)
        {
            // Ignore loop-back addresses & IPv6
            if (!IPAddress.IsLoopback(uniCast.Address) && uniCast.Address.AddressFamily != AddressFamily.InterNetworkV6 && uniCast.IPv4Mask != null)
            {
                var newIp = new IPDefinition { IP = uniCast.Address.ToString(), NetMask = uniCast.IPv4Mask.ToString() };

                location.IPList.Add(newIp);
            }
        }

        foreach (var gateWay in properties.GatewayAddresses)
            location.Gateways.Add(new IPv4Address { IP = gateWay.Address.ToString() });

        foreach (var dns in properties.DnsAddresses)
            location.DNS.Add(new IPv4Address { IP = dns.ToString() });

        location.Description = NewName;

        return location;
    }

    internal static List<AdapterData> GetAdapters(bool GetOnlyPhysicalAdapters = true)
    {
        var data = new List<AdapterData>();

        var adapters = NetworkAdapter.GetInstances().Cast<NetworkAdapter>().Where(z => !GetOnlyPhysicalAdapters || z.PhysicalAdapter);
        var interfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (var item in adapters)
        {
            var newAdapterData = new AdapterData { NetworkAdapter = item, NetworkInterface = interfaces.FirstOrDefault(z => z.Id == item.GUID) };

            data.Add(newAdapterData);
        }
        return data;
    }

    internal static async Task<bool> ApplyLocation(this AdapterData adapter, Location.Location location)
    {
        if (location == null || adapter?.NetworkAdapter == null)
            return false;

        if (location.DHCPEnabled)
        {
            if (!await adapter.SetDnsServers([]))
                return false;

            if (!await SetDHCP(adapter))
                return false;

            var tempLocation = adapter.ExtractConfig(string.Empty);
            if (tempLocation.Gateways.Count > 0)
            {
                return await adapter.SetGateway([tempLocation.Gateways[^1].IP]);
            }

            return true;
        }

        if (location.IPList.Count == 0 || location.IPList.Any(x => string.IsNullOrWhiteSpace(x.IP) || string.IsNullOrWhiteSpace(x.NetMask)))
            return false;

        string[] IP = new string[location.IPList.Count];
        string[] subNet = new string[location.IPList.Count];
        for (byte b = 0; b < location.IPList.Count; b++)
        {
            IP[b] = location.IPList[b].IP;
            subNet[b] = location.IPList[b].NetMask;
        }

        string[] gateWay = new string[location.Gateways.Count];
        if (gateWay.Length > 0)
        {
            for (byte b = 0; b < location.Gateways.Count(); b++)
                gateWay[b] = location.Gateways[b].IP;
        }
        if (!await adapter.SetIP(IP, subNet, gateWay))
            return false;

        string[] dns = new string[location.DNS.Count];
        for (byte b = 0; b < location.DNS.Count(); b++)
            dns[b] = location.DNS[b].IP;
        if (!await adapter.SetDnsServers([IP.FirstOrDefault()]))
            return false;

        if (!await adapter.SetDnsServers(dns))
            return false;

        return true;
    }

    internal static async Task<bool> SetDHCP(this AdapterData adapter)
    {
        var adapterConfig = GetNetworkAdapter(adapter);
        if (adapterConfig == null)
            return false;

        var result = await Task.Run(() => adapterConfig.EnableDHCP());

        if (result != 0)
        {
            Show.Message(AdapterDataLoc.EnableDHCPFailed, string.Format(AdapterDataLoc.ErrorMessage, result, Helpers.WMI.FormatMessage.GetMessage((int)result)));
            return false;
        }

        result = await Task.Run(() => adapterConfig.RenewDHCPLease());
        if (result != 0)
        {
            Show.Message(AdapterDataLoc.RenewDHCPLeaseFailed, string.Format(AdapterDataLoc.ErrorMessage, result, Helpers.WMI.FormatMessage.GetMessage((int)result)));
            return false;
        }

        return true;
    }

    internal static DateTime? GetDhcpLeaseExpires(this AdapterData adapter)
    {
        var adapterConfig = GetNetworkAdapter(adapter);
        if (adapterConfig != null)
        {
            return adapterConfig.DHCPLeaseExpires;
        }

        return null;
    }

    internal static DateTime? GetDhcpLeaseObtained(this AdapterData adapter)
    {
        var adapterConfig = GetNetworkAdapter(adapter);
        if (adapterConfig != null)
        {
            return adapterConfig.DHCPLeaseObtained;
        }

        return null;
    }

    internal static async Task<bool> RenewDhcp(this AdapterData adapter)
    {
        var adapterConfig = GetNetworkAdapter(adapter);
        if (adapterConfig != null)
        {
            var result = await Task.Run(() => adapterConfig.ReleaseDHCPLease());
            if (result != 0)
            {
                Show.Message(AdapterDataLoc.RenewDHCPLeaseFailed, string.Format(AdapterDataLoc.ErrorMessage, result, Helpers.WMI.FormatMessage.GetMessage((int)result)));
                return false;
            }
            await Task.Delay(2000);
            result = await Task.Run(() => adapterConfig.RenewDHCPLease());
            if (result != 0)
            {
                Show.Message(AdapterDataLoc.RenewDHCPLeaseFailed, string.Format(AdapterDataLoc.ErrorMessage, result, Helpers.WMI.FormatMessage.GetMessage((int)result)));
                return false;
            }
            return true;
        }
        return false;
    }

    internal static async Task<bool> ReleaseDhcp(this AdapterData adapter)
    {
        var adapterConfig = GetNetworkAdapter(adapter);
        if (adapterConfig != null)
        {
            var result = await Task.Run(() => adapterConfig.ReleaseDHCPLease());
            if (result != 0)
            {
                Show.Message(AdapterDataLoc.RenewDHCPLeaseFailed, string.Format(AdapterDataLoc.ErrorMessage, result, Helpers.WMI.FormatMessage.GetMessage((int)result)));
                return false;
            }

            return true;
        }
        return false;
    }

    internal static async Task<bool> EnableDchp(this AdapterData adapter)
    {
        var adapterConfig = GetNetworkAdapter(adapter);
        if (adapterConfig != null)
        {
            var result = await Task.Run(() => adapterConfig.EnableDHCP());
            if (result != 0)
            {
                Show.Message(AdapterDataLoc.EnableDHCPFailed, string.Format(AdapterDataLoc.ErrorMessage, result, Helpers.WMI.FormatMessage.GetMessage((int)result)));
                return false;
            }

            return true;
        }
        return false;
    }

    internal static async Task<bool> SetIP(this AdapterData adapter, string[] ipAddress, string[] subnetMask, string[] gateway)
    {
        if (ipAddress == null || subnetMask == null || ipAddress.Length == 0 || ipAddress.Length != subnetMask.Length)
            return false;

        var adapterConfig = GetNetworkAdapter(adapter);
        if (adapterConfig != null)
        {
            var result = await Task.Run(() => adapterConfig.EnableStatic(ipAddress, subnetMask));
            if (result != 0)
            {
                Show.Message(AdapterDataLoc.EnableStaticFailed, string.Format(AdapterDataLoc.ErrorMessage, result, Helpers.WMI.FormatMessage.GetMessage((int)result)));
                return false;
            }

            result = await Task.Run(() => adapterConfig.SetGateways(gateway));

            if (result != 0)
            {
                Show.Message(AdapterDataLoc.SetGatewaysFailed, string.Format(AdapterDataLoc.ErrorMessage, result, Helpers.WMI.FormatMessage.GetMessage((int)result)));
                return false;
            }

            return true;
        }
        else
            return false;
    }

    internal static async Task<bool> SetGateway(this AdapterData adapter, string[] gateway)
    {
        var adapterConfig = GetNetworkAdapter(adapter);
        if (adapterConfig != null)
        {
            var result = await Task.Run(() => adapterConfig.SetGateways(gateway));

            if (result != 0)
            {
                Show.Message(AdapterDataLoc.SetGatewaysFailed, string.Format(AdapterDataLoc.ErrorMessage, result, Helpers.WMI.FormatMessage.GetMessage((int)result)));
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
            var result = await Task.Run(() => adapterConfig.SetDNSServerSearchOrder(dnsServers));
            if (result != 0)
            {
                Show.Message(AdapterDataLoc.SetDnsServersFailed, string.Format(AdapterDataLoc.ErrorMessage, result, Helpers.WMI.FormatMessage.GetMessage((int)result)));
                return false;
            }

            return true;
        }
        else
            return false;
    }

    internal static async Task<bool> Activate(this AdapterData adapter)
    {
        var couldEnable = await adapter.NetworkAdapter.EnableAsync();

        if (couldEnable != 0)
        {
            Show.Message(AdapterDataLoc.ActivationFailed, string.Format(AdapterDataLoc.ErrorMessage, couldEnable, Helpers.WMI.FormatMessage.GetMessage((int)couldEnable)));
            return false;
        }
        return true;
    }

    internal static async Task<bool> Deactivate(this AdapterData adapter)
    {
        var couldDisable = await adapter.NetworkAdapter.DisableAsync();
        if (couldDisable != 0)
        {
            Show.Message(AdapterDataLoc.DeactivationFailed, string.Format(AdapterDataLoc.ErrorMessage, couldDisable, Helpers.WMI.FormatMessage.GetMessage((int)couldDisable)));
            return false;
        }

        return true;
    }

    private static NetworkAdapterConfiguration GetNetworkAdapter(this AdapterData adapter)
    {
        return NetworkAdapterConfiguration.GetInstances().Cast<NetworkAdapterConfiguration>().FirstOrDefault(z => z.InterfaceIndex == adapter.NetworkAdapter.InterfaceIndex);
    }
}
