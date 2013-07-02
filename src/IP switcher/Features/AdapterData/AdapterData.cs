using ROOT.CIMV2.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Deucalion.IP_Switcher.Features.AdapterData
{
    public class AdapterData
    {
        public NetworkAdapter networkAdapter { get; set; }
        public NetworkInterface networkInterface { get; set; }
    }
}