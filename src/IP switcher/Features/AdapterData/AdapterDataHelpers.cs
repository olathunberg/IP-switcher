using ROOT.CIMV2.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Deucalion.IP_Switcher.Features.AdapterData
{
    internal class AdapterDataHelpers
    {
        internal static List<AdapterData> GetAdapters()
        {
            var data = new List<AdapterData>();

            var adapters = NetworkAdapter.GetInstances().Cast<NetworkAdapter>().Where(z => z.PhysicalAdapter);
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var item in adapters)
            {
                var newAdapterData = new AdapterData() { networkAdapter = item, networkInterface = interfaces.FirstOrDefault(z => z.Id == item.GUID) };

                data.Add(newAdapterData);
            }
            return data;
        }
    }
}
