using Deucalion.IP_Switcher.Helpers.ShowWindow;
using ROOT.CIMV2.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Deucalion.IP_Switcher.Features.IpSwitcher.AdapterData
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

            Update(adapter);
        }

        public void Update(AdapterData adapter)
        {
            try
            {
                // Refresh data
                adapter.networkAdapter = NetworkAdapter.GetInstances().Cast<NetworkAdapter>().FirstOrDefault(z => z.GUID == adapter.networkAdapter.GUID);
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                adapter.networkInterface = interfaces.FirstOrDefault(z => z.Id == adapter.networkAdapter.GUID);

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
                    Speed = (adapter.networkAdapter.Speed / (1000 * 1000)).ToString("F1") + " Mbps";
                else
                    Speed = null;

                IsDhcpEnabled = networkInterfaceIPv4Properties.IsDhcpEnabled;
                WinsEnabled = networkInterfaceIPv4Properties.UsesWins.ToActiveText();

                // Ignore loop-back addresses & IPv6
                var tempIp = string.Empty;
                foreach (var item in networkInterfaceIPProperties.UnicastAddresses
                                                                 .Where(z => z.Address.AddressFamily == AddressFamily.InterNetwork)
                                                                 .Where(z => !IPAddress.IsLoopback(z.Address))
                                                                 .Where(z => z.IPv4Mask != null && z.Address != null))
                    tempIp += string.Format(@"{0}\{1}{2}", item.Address, item.IPv4Mask, Environment.NewLine);
                Ip = tempIp.Trim();

                var tempDnsServers = string.Empty;
                foreach (var item in networkInterfaceIPProperties.DnsAddresses.Where(z => z.AddressFamily == AddressFamily.InterNetwork))
                    tempDnsServers += item + Environment.NewLine;
                DnsServers = tempDnsServers.Trim();

                var tempGateways = string.Empty;
                foreach (var item in networkInterfaceIPProperties.GatewayAddresses)
                    tempGateways += item.Address + Environment.NewLine;
                Gateways = tempGateways.Trim();

                var TempDhcpServers = string.Empty;
                foreach (var item in networkInterfaceIPProperties.DhcpServerAddresses.Where(z => z.AddressFamily == AddressFamily.InterNetwork))
                    TempDhcpServers += item + Environment.NewLine;
                DhcpServers = TempDhcpServers.Trim();

                var tempWinsServers = string.Empty;
                foreach (var item in networkInterfaceIPProperties.WinsServersAddresses.Where(z => z.AddressFamily == AddressFamily.InterNetwork))
                    tempWinsServers += item + Environment.NewLine;
                WinsServers = tempWinsServers.Trim();

                var tempAnyCast = string.Empty;
                foreach (var item in networkInterfaceIPProperties.AnycastAddresses.Where(z => z.Address.AddressFamily == AddressFamily.InterNetwork))
                    tempAnyCast += item.Address + Environment.NewLine;
                AnyCast = tempAnyCast.Trim();

                var tempMulticast = string.Empty;
                foreach (var item in networkInterfaceIPProperties.MulticastAddresses.Where(z => z.Address.AddressFamily == AddressFamily.InterNetwork))
                    tempMulticast += item.Address + Environment.NewLine;
                Multicast = tempMulticast.Trim();
            }
            catch (Exception ex)
            {
                Show.Message("Exception", ex.Message);
            }
        }

        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged();
            }
        }

        public string Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                NotifyPropertyChanged();
            }
        }

        public string WinsEnabled
        {
            get { return winsEnabled; }
            set
            {
                winsEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public string DhcpEnabled
        {
            get { return IsDhcpEnabled.ToActiveText(); }
        }

        public bool IsDhcpEnabled
        {
            get { return isDhcpEnabled; }
            set
            {
                isDhcpEnabled = value;
                NotifyPropertyChanged("DhcpEnabled");
            }
        }

        public string Ip
        {
            get { return ip; }
            set
            {
                ip = value;
                NotifyPropertyChanged();
            }
        }

        public string DnsServers
        {
            get { return dnsServers; }
            set
            {
                dnsServers = value;
                NotifyPropertyChanged();
            }
        }

        public string Gateways
        {
            get { return gateways; }
            set
            {
                gateways = value;
                NotifyPropertyChanged();
            }
        }

        public string DhcpServers
        {
            get { return dhcpServers; }
            set
            {
                dhcpServers = value;
                NotifyPropertyChanged();
            }
        }

        public string WinsServers
        {
            get { return winsServers; }
            set
            {
                winsServers = value;
                NotifyPropertyChanged();
            }
        }

        public string AnyCast
        {
            get { return anyCast; }
            set
            {
                anyCast = value;
                NotifyPropertyChanged();
            }
        }

        public string Multicast
        {
            get { return multicast; }
            set
            {
                multicast = value;
                NotifyPropertyChanged();
            }
        }

        public string Mac
        {
            get { return mac; }
            set
            {
                mac = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsActive
        {

            get { return isActive; }
            set
            {
                isActive = value;
                NotifyPropertyChanged();
            }
        }

        public bool HasAdapter
        {
            get { return hasAdapter; }
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
            return base.GetHashCode();
        }

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public static class AdapterDataModelExtensions
    {
        public static string GetStatusText(this AdapterData adapter)
        {
            switch (adapter.networkAdapter.NetConnectionStatus)
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
