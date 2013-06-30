using System.Linq;
using ROOT.CIMV2.Win32;
using System.Threading.Tasks;

namespace Deucalion.IP_Switcher.Classes
{
    /// <summary>
    /// Helper class to set networking configuration like IP address, DNS servers, etc.
    /// </summary>
    internal static class NetworkConfigurator
    {
        internal static async Task ApplyLocation(Location location, AdapterData adapter)
        {
            await Task.Factory.StartNew(() =>
                  {
                      if (location == null)
                          return;

                      if (location.DHCPEnabled)
                      {
                          NetworkConfigurator.SetDHCP(adapter);
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
                      for (byte b = 0; b < location.Gateways.Count(); b++)
                          gateWay[b] = location.Gateways[b].IP;
                      if (gateWay.Count() == 0)
                          gateWay = null;

                      NetworkConfigurator.SetIP(null, null, new string[] { }, adapter);
                      NetworkConfigurator.SetIP(IP, subNet, gateWay, adapter);

                      string[] Dns = new string[location.DNS.Count];
                      for (byte b = 0; b < location.DNS.Count(); b++)
                          Dns[b] = location.DNS[b].IP;
                      NetworkConfigurator.SetDnsServers(null, adapter);
                      NetworkConfigurator.SetDnsServers(Dns, adapter);
                  });
        }

        internal static void SetDHCP(AdapterData adapter)
        {
            uint Result = 0;

            foreach (var adapterConfig in NetworkAdapterConfiguration.GetInstances().Cast<NetworkAdapterConfiguration>().Where(z => z.Description == adapter.networkAdapter.Description))
            {
                Result = SetIP(null, null, new string[] { }, adapter);
                SetDnsServers(null, adapter);

                adapterConfig.EnableDHCP();
                adapterConfig.RenewDHCPLease();
            }
        }

        internal static uint SetIP(string[] ipAddress, string[] subnetMask, string[] gateway, AdapterData adapter)
        {
            uint Result = 0;
            foreach (var adapterConfig in NetworkAdapterConfiguration.GetInstances().Cast<NetworkAdapterConfiguration>().Where(z => z.Description == adapter.networkAdapter.Description))
            {
                Result = adapterConfig.EnableStatic(ipAddress, subnetMask);

                Result = adapterConfig.SetGateways(gateway, new ushort[] { 1 });
            }

            return Result;
        }

        internal static void SetDnsServers(string[] dnsServers, AdapterData adapter)
        {
            foreach (var adapterConfig in NetworkAdapterConfiguration.GetInstances().Cast<NetworkAdapterConfiguration>().Where(z => z.Description == adapter.networkAdapter.Description))
                adapterConfig.SetDNSServerSearchOrder(dnsServers);
        }
    }
}
