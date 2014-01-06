using ROOT.CIMV2.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Deucalion.IP_Switcher.Features.AdapterData
{
    public class AdapterDataModel
    {
        public string Status { get; set; }

        public string Name { get; set; }

        public string Speed { get; set; }

        public string WinsEnabled { get; set; }

        public string DhcpEnabled { get { return IsDhcpEnabled.ToActiveText(); } }

        public bool IsDhcpEnabled { get;set; }

        public string Ip { get; set; }

        public string DnsServers { get; set; }

        public string Gateways { get; set; }

        public string DhcpServers { get; set; }

        public string WinsServers { get; set; }

        public string AnyCast { get; set; }

        public string Multicast { get; set; }

        public string Mac { get; set; }

        public bool IsActive { get; set; }

        public bool HasAdapter { get; set; }

        public AdapterDataModel(AdapterData adapter)
        {
            Name = adapter.networkAdapter.Name;
            Mac = adapter.networkAdapter.MACAddress;

            Status = adapter.GetStatusText();
            IsActive = adapter.networkAdapter.ConfigManagerErrorCode != NetworkAdapter.ConfigManagerErrorCodeValues.This_device_is_disabled_;

            HasAdapter = adapter.networkInterface != null;
            if (adapter.networkInterface == null)
                return;

            var networkInterfaceIPProperties = adapter.networkInterface.GetIPProperties();
            var networkInterfaceIPv4Properties = networkInterfaceIPProperties.GetIPv4Properties();

            if (adapter.networkAdapter.NetConnectionStatus == 2)
                Speed = (adapter.networkAdapter.Speed / (1000*1000)).ToString("F1") + " Mbps";

            IsDhcpEnabled = networkInterfaceIPv4Properties.IsDhcpEnabled;
            WinsEnabled = networkInterfaceIPv4Properties.UsesWins.ToActiveText();

            // Ignore loop-back addresses & IPv6
            Ip = string.Empty;
            foreach (var uniCast in networkInterfaceIPProperties.UnicastAddresses.Where(z => z.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(z.Address) && z.IPv4Mask != null && z.Address != null))
                Ip += String.Format(@"{0}\{1}{2}", uniCast.Address, uniCast.IPv4Mask, Environment.NewLine);
            Ip = Ip.Trim();

            DnsServers = string.Empty;
            foreach (var Dns in networkInterfaceIPProperties.DnsAddresses.Where(z => z.AddressFamily == AddressFamily.InterNetwork))
                DnsServers += Dns + Environment.NewLine;
            DnsServers = DnsServers.Trim();

            Gateways = string.Empty;
            foreach (var gateWay in networkInterfaceIPProperties.GatewayAddresses)
                Gateways += gateWay.Address + Environment.NewLine;
            Gateways = Gateways.Trim();

            DhcpServers = string.Empty;
            foreach (var DHCP in networkInterfaceIPProperties.DhcpServerAddresses.Where(z => z.AddressFamily == AddressFamily.InterNetwork))
                DhcpServers += DHCP + Environment.NewLine;
            DhcpServers = DhcpServers.Trim();

            WinsServers = string.Empty;
            foreach (var item in networkInterfaceIPProperties.WinsServersAddresses.Where(z => z.AddressFamily == AddressFamily.InterNetwork))
                WinsServers += item + Environment.NewLine;
            WinsServers = WinsServers.Trim();

            AnyCast = string.Empty;
            foreach (var anyCast in networkInterfaceIPProperties.AnycastAddresses.Where(z => z.Address.AddressFamily == AddressFamily.InterNetwork))
                AnyCast += anyCast.Address + Environment.NewLine;
            AnyCast = AnyCast.Trim();

            Multicast = string.Empty;
            foreach (var multiCast in networkInterfaceIPProperties.MulticastAddresses.Where(z => z.Address.AddressFamily == AddressFamily.InterNetwork))
                Multicast += multiCast.Address + Environment.NewLine;
            Multicast = Multicast.Trim();
        }
    }

    public static class AdapterDataModelExtensions
    {
        public static string GetStatusText(this AdapterData adapter)
        {
            switch (adapter.networkAdapter.NetConnectionStatus)
            {
                case 0: return Resources.AdapterDataModelLoc.Disconnected;
                case 1: return  Resources.AdapterDataModelLoc.Connecting;
                case 2: return  Resources.AdapterDataModelLoc.Connected;
                case 3: return  Resources.AdapterDataModelLoc.Disconnecting;
                case 4: return  Resources.AdapterDataModelLoc.HardwareNotPresent;
                case 5: return  Resources.AdapterDataModelLoc.HardwareDisabled;
                case 6: return  Resources.AdapterDataModelLoc.HardwareMalfunction;
                case 7: return  Resources.AdapterDataModelLoc.MediaDisconnected;
                case 8: return  Resources.AdapterDataModelLoc.Authenticating;
                case 9: return  Resources.AdapterDataModelLoc.AuthenticationSucceeded;
                case 10: return  Resources.AdapterDataModelLoc.AuthenticationFailed;
                case 11: return  Resources.AdapterDataModelLoc.InvalidAddress;
                case 12: return  Resources.AdapterDataModelLoc.CredentialsRequired;
                default:
                    return  Resources.AdapterDataModelLoc.Unknown;
            }
        }

        public static string ToActiveText(this bool state)
        {
            if (state)
                return Resources.AdapterDataModelLoc.Active;
            else
                return Resources.AdapterDataModelLoc.Inactive;
        }
    }
}
