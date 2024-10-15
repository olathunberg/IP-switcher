using ROOT.CIMV2.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using TTech.IP_Switcher.Helpers;

namespace TTech.IP_Switcher.Features.IpSwitcher.AdapterData
{
    public class AdapterDataModel : INotifyPropertyChanged
    {
        private string status;
        private string name;
        private string speed;
        private string winsEnabled;
        private bool isDhcpEnabled;
        private string ip;
        private string dnsServers;
        private string gateways;
        private string dhcpServers;
        private string winsServers;
        private string anyCast;
        private string multicast;
        private string mac;
        private bool isActive;
        private bool hasAdapter;

        public AdapterDataModel()
        { }

        public AdapterDataModel(AdapterData adapter)
        {
            if (adapter == null)
                return;

            Update(adapter, null, null);
        }

        public void Update(AdapterData adapter, List<NetworkAdapter> adapters, List<NetworkInterface> interfaces)
        {
            try
            {
                // Refresh data
                adapters ??= NetworkAdapter.GetInstances().Cast<NetworkAdapter>().ToList();
                interfaces ??= NetworkInterface.GetAllNetworkInterfaces().ToList();

                adapter.Update(adapters, interfaces);

                Name = adapter.NetworkAdapter.Name;
                Mac = adapter.NetworkAdapter.MACAddress;

                Status = adapter.GetStatusText();
                IsActive = adapter.NetworkAdapter.ConfigManagerErrorCode != NetworkAdapter.ConfigManagerErrorCodeValues.This_device_is_disabled_;

                HasAdapter = adapter.NetworkInterface != null;
                if (adapter.NetworkInterface == null)
                    return;

                var networkInterfaceIPProperties = adapter.NetworkInterface.GetIPProperties();
                var networkInterfaceIPv4Properties = networkInterfaceIPProperties.GetIPv4Properties();

                if (adapter.NetworkAdapter.NetConnectionStatus == 2)
                    Speed = (adapter.NetworkAdapter.Speed / (1000 * 1000)).ToString("F1") + " Mbps";
                else
                    Speed = null;

                IsDhcpEnabled = networkInterfaceIPv4Properties.IsDhcpEnabled;
                WinsEnabled = networkInterfaceIPv4Properties.UsesWins.ToActiveText();

                // Ignore loop-back addresses & IPv6
                Ip = string.Join(Environment.NewLine, networkInterfaceIPProperties.UnicastAddresses
                                                                                  .Where(z => z.Address.AddressFamily == AddressFamily.InterNetwork)
                                                                                  .Where(z => !IPAddress.IsLoopback(z.Address))
                                                                                  .Where(z => z.IPv4Mask != null && z.Address != null)
                                                                                  .Select(x => $@"{x.Address}/{x.IPv4Mask}"));

                DnsServers = string.Join(Environment.NewLine, networkInterfaceIPProperties.DnsAddresses
                                                                                          .Where(z => z.AddressFamily == AddressFamily.InterNetwork));

                Gateways = string.Join(Environment.NewLine, networkInterfaceIPProperties.GatewayAddresses.Select(x => x.Address));

                DhcpServers = string.Join(Environment.NewLine, networkInterfaceIPProperties.DhcpServerAddresses
                                                                                           .Where(z => z.AddressFamily == AddressFamily.InterNetwork));

                WinsServers = string.Join(Environment.NewLine, networkInterfaceIPProperties.WinsServersAddresses
                                                                                           .Where(z => z.AddressFamily == AddressFamily.InterNetwork));

                AnyCast = string.Join(Environment.NewLine, networkInterfaceIPProperties.AnycastAddresses
                                                                                       .Where(z => z.Address.AddressFamily == AddressFamily.InterNetwork)
                                                                                       .Select(x => x.Address));

                Multicast = string.Join(Environment.NewLine, networkInterfaceIPProperties.MulticastAddresses
                                                                                         .Where(z => z.Address.AddressFamily == AddressFamily.InterNetwork)
                                                                                         .Select(x=>x.Address));
            }
            catch (Exception ex)
            {
                SimpleMessenger.Default.SendMessage("ErrorText", ex.Message);
            }
        }

        public string Status
        {
            get => status;
            set
            {
                status = value;
                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                NotifyPropertyChanged();
            }
        }

        public string Speed
        {
            get => speed;
            set
            {
                speed = value;
                NotifyPropertyChanged();
            }
        }

        public string WinsEnabled
        {
            get => winsEnabled;
            set
            {
                winsEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public string DhcpEnabled => IsDhcpEnabled.ToActiveText();

        public bool IsDhcpEnabled
        {
            get => isDhcpEnabled;
            set
            {
                isDhcpEnabled = value;
                NotifyPropertyChanged(nameof(DhcpEnabled));
            }
        }

        public string Ip
        {
            get => ip;
            set
            {
                ip = value;
                NotifyPropertyChanged();
            }
        }

        public string DnsServers
        {
            get => dnsServers;
            set
            {
                dnsServers = value;
                NotifyPropertyChanged();
            }
        }

        public string Gateways
        {
            get => gateways;
            set
            {
                gateways = value;
                NotifyPropertyChanged();
            }
        }

        public string DhcpServers
        {
            get => dhcpServers;
            set
            {
                dhcpServers = value;
                NotifyPropertyChanged();
            }
        }

        public string WinsServers
        {
            get => winsServers;
            set
            {
                winsServers = value;
                NotifyPropertyChanged();
            }
        }

        public string AnyCast
        {
            get => anyCast;
            set
            {
                anyCast = value;
                NotifyPropertyChanged();
            }
        }

        public string Multicast
        {
            get => multicast;
            set
            {
                multicast = value;
                NotifyPropertyChanged();
            }
        }

        public string Mac
        {
            get => mac;
            set
            {
                mac = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsActive
        {

            get => isActive;
            set
            {
                isActive = value;
                NotifyPropertyChanged();
            }
        }

        public bool HasAdapter
        {
            get => hasAdapter;
            set
            {
                hasAdapter = value;
                NotifyPropertyChanged();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is AdapterDataModel && Mac == (obj as AdapterDataModel).Mac)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return Mac.GetHashCode();
        }

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public static class AdapterDataModelExtensions
    {
        public static string GetStatusText(this AdapterData adapter)
        {
            switch (adapter.NetworkAdapter.NetConnectionStatus)
            {
                case 0: return Resources.AdapterDataModelLoc.Disconnected;
                case 1: return Resources.AdapterDataModelLoc.Connecting;
                case 2: return Resources.AdapterDataModelLoc.Connected;
                case 3: return Resources.AdapterDataModelLoc.Disconnecting;
                case 4: return Resources.AdapterDataModelLoc.HardwareNotPresent;
                case 5: return Resources.AdapterDataModelLoc.HardwareDisabled;
                case 6: return Resources.AdapterDataModelLoc.HardwareMalfunction;
                case 7: return Resources.AdapterDataModelLoc.MediaDisconnected;
                case 8: return Resources.AdapterDataModelLoc.Authenticating;
                case 9: return Resources.AdapterDataModelLoc.AuthenticationSucceeded;
                case 10: return Resources.AdapterDataModelLoc.AuthenticationFailed;
                case 11: return Resources.AdapterDataModelLoc.InvalidAddress;
                case 12: return Resources.AdapterDataModelLoc.CredentialsRequired;
                default:
                    return Resources.AdapterDataModelLoc.Unknown;
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
