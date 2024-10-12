using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Threading;
using System.Text;

namespace NativeWifi
{
    /// <summary>
    /// Represents a client to the Zeroconf (Native Wifi) service.
    /// </summary>
    /// <remarks>
    /// This class is the entrypoint to Native Wifi management. To manage WiFi settings, create an instance
    /// of this class.
    /// </remarks>
    public class WlanClient : IDisposable
    {
        /// <summary>
        /// Represents a Wifi network interface.
        /// </summary>
        public class WlanInterface
        {
            private readonly WlanClient client;
            private readonly WLan.WlanInterfaceInfo info;

            #region Events
            /// <summary>
            /// Represents a method that will handle <see cref="WlanNotification"/> events.
            /// </summary>
            /// <param name="notifyData">The notification data.</param>
            public delegate void WlanNotificationEventHandler(WLan.WlanNotificationData notifyData);

            /// <summary>
            /// Represents a method that will handle <see cref="WlanConnectionNotification"/> events.
            /// </summary>
            /// <param name="notifyData">The notification data.</param>
            /// <param name="connNotifyData">The notification data.</param>
            public delegate void WlanConnectionNotificationEventHandler(WLan.WlanNotificationData notifyData, WLan.WlanConnectionNotificationData connNotifyData);

            /// <summary>
            /// Represents a method that will handle <see cref="WlanReasonNotification"/> events.
            /// </summary>
            /// <param name="notifyData">The notification data.</param>
            /// <param name="reasonCode">The reason code.</param>
            public delegate void WlanReasonNotificationEventHandler(WLan.WlanNotificationData notifyData, WLan.WlanReasonCode reasonCode);

            /// <summary>
            /// Occurs when an event of any kind occurs on a WLAN interface.
            /// </summary>
            public event WlanNotificationEventHandler WlanNotification;

            /// <summary>
            /// Occurs when a WLAN interface changes connection state.
            /// </summary>
            public event WlanConnectionNotificationEventHandler WlanConnectionNotification;

            /// <summary>
            /// Occurs when a WLAN operation fails due to some reason.
            /// </summary>
            public event WlanReasonNotificationEventHandler WlanReasonNotification;

            #endregion

            #region Event queue
            private bool queueEvents;
            private readonly AutoResetEvent eventQueueFilled = new(false);
            private readonly Queue<object> eventQueue = new();

            private struct WlanConnectionNotificationEventData
            {
                public WLan.WlanNotificationData notifyData;
                public WLan.WlanConnectionNotificationData connNotifyData;
            }
            private struct WlanReasonNotificationData
            {
                public WLan.WlanNotificationData notifyData;
                public WLan.WlanReasonCode reasonCode;
            }
            #endregion

            internal WlanInterface(WlanClient client, WLan.WlanInterfaceInfo info)
            {
                this.client = client;
                this.info = info;
            }

            /// <summary>
            /// Sets a parameter of the interface whose data type is <see cref="int"/>.
            /// </summary>
            /// <param name="opCode">The opcode of the parameter.</param>
            /// <param name="value">The value to set.</param>
            private void SetInterfaceInt(WLan.WlanIntfOpcode opCode, int value)
            {
                var valuePtr = Marshal.AllocHGlobal(sizeof(int));
                Marshal.WriteInt32(valuePtr, value);
                try
                {
                    WLan.ThrowIfError(
                        WLan.WlanSetInterface(client.clientHandle, info.interfaceGuid, opCode, sizeof(int), valuePtr, IntPtr.Zero));
                }
                finally
                {
                    Marshal.FreeHGlobal(valuePtr);
                }
            }

            /// <summary>
            /// Gets a parameter of the interface whose data type is <see cref="int"/>.
            /// </summary>
            /// <param name="opCode">The opcode of the parameter.</param>
            /// <returns>The integer value.</returns>
            private int GetInterfaceInt(WLan.WlanIntfOpcode opCode)
            {
                IntPtr valuePtr;
                int valueSize;
                WLan.WlanOpcodeValueType opcodeValueType;
                WLan.ThrowIfError(
                    WLan.WlanQueryInterface(client.clientHandle, info.interfaceGuid, opCode, IntPtr.Zero, out valueSize, out valuePtr, out opcodeValueType));
                try
                {
                    return Marshal.ReadInt32(valuePtr);
                }
                finally
                {
                    WLan.WlanFreeMemory(valuePtr);
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="WlanInterface"/> is automatically configured.
            /// </summary>
            /// <value><c>true</c> if "autoconf" is enabled; otherwise, <c>false</c>.</value>
            public bool Autoconf
            {
                get => GetInterfaceInt(WLan.WlanIntfOpcode.AutoconfEnabled) != 0;
                set => SetInterfaceInt(WLan.WlanIntfOpcode.AutoconfEnabled, value ? 1 : 0);
            }

            /// <summary>
            /// Gets or sets the BSS type for the indicated interface.
            /// </summary>
            /// <value>The type of the BSS.</value>
            public WLan.Dot11BssType BssType
            {
                get => (WLan.Dot11BssType)GetInterfaceInt(WLan.WlanIntfOpcode.BssType);
                set => SetInterfaceInt(WLan.WlanIntfOpcode.BssType, (int)value);
            }

            /// <summary>
            /// Gets the state of the interface.
            /// </summary>
            /// <value>The state of the interface.</value>
            public WLan.WlanInterfaceState InterfaceState => (WLan.WlanInterfaceState)GetInterfaceInt(WLan.WlanIntfOpcode.InterfaceState);

            /// <summary>
            /// Gets the channel.
            /// </summary>
            /// <value>The channel.</value>
            /// <remarks>Not supported on Windows XP SP2.</remarks>
            public int Channel => GetInterfaceInt(WLan.WlanIntfOpcode.ChannelNumber);

            /// <summary>
            /// Gets the RSSI.
            /// </summary>
            /// <value>The RSSI.</value>
            /// <remarks>Not supported on Windows XP SP2.</remarks>
            public int RSSI => GetInterfaceInt(WLan.WlanIntfOpcode.RSSI);

            /// <summary>
            /// Gets the radio state.
            /// </summary>
            /// <value>The radio state.</value>
            /// <remarks>Not supported on Windows XP.</remarks>
            //public Wlan.WlanRadioState RadioState
            //{
            //    get
            //    {
            //        int valueSize;
            //        IntPtr valuePtr;
            //        Wlan.WlanOpcodeValueType opcodeValueType;
            //        Wlan.ThrowIfError(
            //            Wlan.WlanQueryInterface(client.clientHandle, info.interfaceGuid, Wlan.WlanIntfOpcode.RadioState, IntPtr.Zero, out valueSize, out valuePtr, out opcodeValueType));
            //        try
            //        {
            //            return (Wlan.WlanRadioState)Marshal.PtrToStructure(valuePtr, typeof(Wlan.WlanRadioState));
            //        }
            //        finally
            //        {
            //            Wlan.WlanFreeMemory(valuePtr);
            //        }
            //    }
            //}

            /// <summary>
            /// Gets the current operation mode.
            /// </summary>
            /// <value>The current operation mode.</value>
            /// <remarks>Not supported on Windows XP SP2.</remarks>
            public WLan.Dot11OperationMode CurrentOperationMode => (WLan.Dot11OperationMode)GetInterfaceInt(WLan.WlanIntfOpcode.CurrentOperationMode);

            /// <summary>
            /// Gets the attributes of the current connection.
            /// </summary>
            /// <value>The current connection attributes.</value>
            /// <exception cref="Win32Exception">An exception with code 0x0000139F (The group or resource is not in the correct state to perform the requested operation.) will be thrown if the interface is not connected to a network.</exception>
            public WLan.WlanConnectionAttributes CurrentConnection
            {
                get
                {
                    int valueSize;
                    IntPtr valuePtr;
                    WLan.WlanOpcodeValueType opcodeValueType;
                    WLan.ThrowIfError(
                        WLan.WlanQueryInterface(client.clientHandle, info.interfaceGuid, WLan.WlanIntfOpcode.CurrentConnection, IntPtr.Zero, out valueSize, out valuePtr, out opcodeValueType));
                    try
                    {
                        return (WLan.WlanConnectionAttributes)Marshal.PtrToStructure(valuePtr, typeof(WLan.WlanConnectionAttributes));
                    }
                    finally
                    {
                        WLan.WlanFreeMemory(valuePtr);
                    }
                }
            }

            /// <summary>
            /// Requests a scan for available networks.
            /// </summary>
            /// <remarks>
            /// The method returns immediately. Progress is reported through the <see cref="WlanNotification"/> event.
            /// </remarks>
            public void Scan()
            {
                WLan.ThrowIfError(
                    WLan.WlanScan(client.clientHandle, info.interfaceGuid, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero));
            }

            /// <summary>
            /// Converts a pointer to a available networks list (header + entries) to an array of available network entries.
            /// </summary>
            /// <param name="availNetListPtr">A pointer to an available networks list's header.</param>
            /// <returns>An array of available network entries.</returns>
            private static WLan.WlanAvailableNetwork[] ConvertAvailableNetworkListPtr(IntPtr availNetListPtr)
            {
                var availNetListHeader = (WLan.WlanAvailableNetworkListHeader)Marshal.PtrToStructure(availNetListPtr, typeof(WLan.WlanAvailableNetworkListHeader));
                long availNetListIt = availNetListPtr.ToInt64() + Marshal.SizeOf(typeof(WLan.WlanAvailableNetworkListHeader));
                WLan.WlanAvailableNetwork[] availNets = new WLan.WlanAvailableNetwork[availNetListHeader.numberOfItems];
                for (int i = 0; i < availNetListHeader.numberOfItems; ++i)
                {
                    availNets[i] = (WLan.WlanAvailableNetwork)Marshal.PtrToStructure(new IntPtr(availNetListIt), typeof(WLan.WlanAvailableNetwork));
                    availNetListIt += Marshal.SizeOf(typeof(WLan.WlanAvailableNetwork));
                }
                return availNets;
            }

            /// <summary>
            /// Retrieves the list of available networks.
            /// </summary>
            /// <param name="flags">Controls the type of networks returned.</param>
            /// <returns>A list of the available networks.</returns>
            public WLan.WlanAvailableNetwork[] GetAvailableNetworkList(WLan.WlanGetAvailableNetworkFlags flags)
            {
                IntPtr availNetListPtr;
                WLan.ThrowIfError(
                    WLan.WlanGetAvailableNetworkList(client.clientHandle, info.interfaceGuid, flags, IntPtr.Zero, out availNetListPtr));
                try
                {
                    return ConvertAvailableNetworkListPtr(availNetListPtr);
                }
                finally
                {
                    WLan.WlanFreeMemory(availNetListPtr);
                }
            }

            /// <summary>
            /// Converts a pointer to a BSS list (header + entries) to an array of BSS entries.
            /// </summary>
            /// <param name="bssListPtr">A pointer to a BSS list's header.</param>
            /// <returns>An array of BSS entries.</returns>
            private static WLan.WlanBssEntry[] ConvertBssListPtr(IntPtr bssListPtr)
            {
                var bssListHeader = (WLan.WlanBssListHeader)Marshal.PtrToStructure(bssListPtr, typeof(WLan.WlanBssListHeader));
                long bssListIt = bssListPtr.ToInt64() + Marshal.SizeOf(typeof(WLan.WlanBssListHeader));
                WLan.WlanBssEntry[] bssEntries = new WLan.WlanBssEntry[bssListHeader.numberOfItems];
                for (int i = 0; i < bssListHeader.numberOfItems; ++i)
                {
                    bssEntries[i] = (WLan.WlanBssEntry)Marshal.PtrToStructure(new IntPtr(bssListIt), typeof(WLan.WlanBssEntry));
                    bssListIt += Marshal.SizeOf(typeof(WLan.WlanBssEntry));
                }
                return bssEntries;
            }

            /// <summary>
            /// Retrieves the basic service sets (BSS) list of all available networks.
            /// </summary>
            public WLan.WlanBssEntry[] GetNetworkBssList()
            {
                IntPtr bssListPtr;
                WLan.ThrowIfError(
                    WLan.WlanGetNetworkBssList(client.clientHandle, info.interfaceGuid, IntPtr.Zero, WLan.Dot11BssType.Any, false, IntPtr.Zero, out bssListPtr));
                try
                {
                    return ConvertBssListPtr(bssListPtr);
                }
                finally
                {
                    WLan.WlanFreeMemory(bssListPtr);
                }
            }

            /// <summary>
            /// Retrieves the basic service sets (BSS) list of the specified network.
            /// </summary>
            /// <param name="ssid">Specifies the SSID of the network from which the BSS list is requested.</param>
            /// <param name="bssType">Indicates the BSS type of the network.</param>
            /// <param name="securityEnabled">Indicates whether security is enabled on the network.</param>
            public WLan.WlanBssEntry[] GetNetworkBssList(WLan.Dot11Ssid ssid, WLan.Dot11BssType bssType, bool securityEnabled)
            {
                var ssidPtr = Marshal.AllocHGlobal(Marshal.SizeOf(ssid));
                Marshal.StructureToPtr(ssid, ssidPtr, false);
                try
                {
                    IntPtr bssListPtr;
                    WLan.ThrowIfError(
                        WLan.WlanGetNetworkBssList(client.clientHandle, info.interfaceGuid, ssidPtr, bssType, securityEnabled, IntPtr.Zero, out bssListPtr));
                    try
                    {
                        return ConvertBssListPtr(bssListPtr);
                    }
                    finally
                    {
                        WLan.WlanFreeMemory(bssListPtr);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(ssidPtr);
                }
            }

            /// <summary>
            /// Connects to a network defined by a connection parameters structure.
            /// </summary>
            /// <param name="connectionParams">The connection paramters.</param>
            protected void Connect(WLan.WlanConnectionParameters connectionParams)
            {
                WLan.ThrowIfError(
                    WLan.WlanConnect(client.clientHandle, info.interfaceGuid, ref connectionParams, IntPtr.Zero));
            }

            /// <summary>
            /// Requests a connection (association) to the specified wireless network.
            /// </summary>
            /// <remarks>
            /// The method returns immediately. Progress is reported through the <see cref="WlanNotification"/> event.
            /// </remarks>
            public void Connect(WLan.WlanConnectionMode connectionMode, WLan.Dot11BssType bssType, string profile)
            {
                var connectionParams = new WLan.WlanConnectionParameters();
                connectionParams.wlanConnectionMode = connectionMode;
                connectionParams.profile = profile;
                connectionParams.dot11BssType = bssType;
                connectionParams.flags = 0;
                Connect(connectionParams);
            }

            /// <summary>
            /// Connects (associates) to the specified wireless network, returning either on a success to connect
            /// or a failure.
            /// </summary>
            /// <param name="connectionMode"></param>
            /// <param name="bssType"></param>
            /// <param name="profile"></param>
            /// <param name="connectTimeout"></param>
            /// <returns></returns>
            public bool ConnectSynchronously(WLan.WlanConnectionMode connectionMode, WLan.Dot11BssType bssType, string profile, int connectTimeout)
            {
                queueEvents = true;
                try
                {
                    Connect(connectionMode, bssType, profile);
                    while (queueEvents && eventQueueFilled.WaitOne(connectTimeout, true))
                    {
                        lock (eventQueue)
                        {
                            while (eventQueue.Count != 0)
                            {
                                var e = eventQueue.Dequeue();
                                if (e is WlanConnectionNotificationEventData)
                                {
                                    var wlanConnectionData = (WlanConnectionNotificationEventData)e;
                                    // Check if the conditions are good to indicate either success or failure.
                                    if (wlanConnectionData.notifyData.notificationSource == WLan.WlanNotificationSource.ACM)
                                    {
                                        switch ((WLan.WlanNotificationCodeAcm)wlanConnectionData.notifyData.notificationCode)
                                        {
                                            case WLan.WlanNotificationCodeAcm.ConnectionComplete:
                                                if (wlanConnectionData.connNotifyData.profileName == profile)
                                                    return true;
                                                break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    queueEvents = false;
                    eventQueue.Clear();
                }
                return false; // timeout expired and no "connection complete"
            }

            /// <summary>
            /// Connects to the specified wireless network.
            /// </summary>
            /// <remarks>
            /// The method returns immediately. Progress is reported through the <see cref="WlanNotification"/> event.
            /// </remarks>
            public void Connect(WLan.WlanConnectionMode connectionMode, WLan.Dot11BssType bssType, WLan.Dot11Ssid ssid, WLan.WlanConnectionFlags flags)
            {
                var connectionParams = new WLan.WlanConnectionParameters();
                connectionParams.wlanConnectionMode = connectionMode;
                connectionParams.dot11SsidPtr = Marshal.AllocHGlobal(Marshal.SizeOf(ssid));
                Marshal.StructureToPtr(ssid, connectionParams.dot11SsidPtr, false);
                connectionParams.dot11BssType = bssType;
                connectionParams.flags = flags;
                Connect(connectionParams);
                Marshal.DestroyStructure(connectionParams.dot11SsidPtr, ssid.GetType());
                Marshal.FreeHGlobal(connectionParams.dot11SsidPtr);
            }

            /// <summary>
            /// Deletes a profile.
            /// </summary>
            /// <param name="profileName">
            /// The name of the profile to be deleted. Profile names are case-sensitive.
            /// On Windows XP SP2, the supplied name must match the profile name derived automatically from the SSID of the network. For an infrastructure network profile, the SSID must be supplied for the profile name. For an ad hoc network profile, the supplied name must be the SSID of the ad hoc network followed by <c>-adhoc</c>.
            /// </param>
            public void DeleteProfile(string profileName)
            {
                WLan.ThrowIfError(
                    WLan.WlanDeleteProfile(client.clientHandle, info.interfaceGuid, profileName, IntPtr.Zero));
            }

            /// <summary>
            /// Sets the profile.
            /// </summary>
            /// <param name="flags">The flags to set on the profile.</param>
            /// <param name="profileXml">The XML representation of the profile. On Windows XP SP 2, special care should be taken to adhere to its limitations.</param>
            /// <param name="overwrite">If a profile by the given name already exists, then specifies whether to overwrite it (if <c>true</c>) or return an error (if <c>false</c>).</param>
            /// <returns>The resulting code indicating a success or the reason why the profile wasn't valid.</returns>
            public WLan.WlanReasonCode SetProfile(WLan.WlanProfileFlags flags, string profileXml, bool overwrite)
            {
                WLan.WlanReasonCode reasonCode;
                WLan.ThrowIfError(
                        WLan.WlanSetProfile(client.clientHandle, info.interfaceGuid, flags, profileXml, null, overwrite, IntPtr.Zero, out reasonCode));
                return reasonCode;
            }

            /// <summary>
            /// Gets the profile's XML specification.
            /// </summary>
            /// <param name="profileName">The name of the profile.</param>
            /// <returns>The XML document.</returns>
            public string GetProfileXml(string profileName)
            {
                if (profileName == null)
                    return string.Empty;
                IntPtr profileXmlPtr;
                WLan.WlanProfileFlags flags = WLan.WlanProfileFlags.GET_PLAINTEXT_KEY;
                WLan.WlanAccess access;
                WLan.ThrowIfError(
                    WLan.WlanGetProfile(client.clientHandle, info.interfaceGuid, profileName, IntPtr.Zero, out profileXmlPtr, out flags,
                                   out access));
                try
                {
                    return Marshal.PtrToStringUni(profileXmlPtr);
                }
                finally
                {
                    WLan.WlanFreeMemory(profileXmlPtr);
                }
            }

            /// <summary>
            /// Gets the information of all profiles on this interface.
            /// </summary>
            /// <returns>The profiles information.</returns>
            public WLan.WlanProfileInfo[] GetProfiles()
            {
                IntPtr profileListPtr;
                WLan.ThrowIfError(
                    WLan.WlanGetProfileList(client.clientHandle, info.interfaceGuid, IntPtr.Zero, out profileListPtr));
                try
                {
                    var header = (WLan.WlanProfileInfoListHeader)Marshal.PtrToStructure(profileListPtr, typeof(WLan.WlanProfileInfoListHeader));
                    WLan.WlanProfileInfo[] profileInfos = new WLan.WlanProfileInfo[header.numberOfItems];
                    long profileListIterator = profileListPtr.ToInt64() + Marshal.SizeOf(header);
                    for (int i = 0; i < header.numberOfItems; ++i)
                    {
                        var profileInfo = (WLan.WlanProfileInfo)Marshal.PtrToStructure(new IntPtr(profileListIterator), typeof(WLan.WlanProfileInfo));
                        profileInfos[i] = profileInfo;
                        profileListIterator += Marshal.SizeOf(profileInfo);
                    }
                    return profileInfos;
                }
                finally
                {
                    WLan.WlanFreeMemory(profileListPtr);
                }
            }

            internal void OnWlanConnection(WLan.WlanNotificationData notifyData, WLan.WlanConnectionNotificationData connNotifyData)
            {
                if (WlanConnectionNotification != null)
                    WlanConnectionNotification(notifyData, connNotifyData);

                if (queueEvents)
                {
                    var queuedEvent = new WlanConnectionNotificationEventData();
                    queuedEvent.notifyData = notifyData;
                    queuedEvent.connNotifyData = connNotifyData;
                    EnqueueEvent(queuedEvent);
                }
            }

            internal void OnWlanReason(WLan.WlanNotificationData notifyData, WLan.WlanReasonCode reasonCode)
            {
                if (WlanReasonNotification != null)
                    WlanReasonNotification(notifyData, reasonCode);
                if (queueEvents)
                {
                    var queuedEvent = new WlanReasonNotificationData();
                    queuedEvent.notifyData = notifyData;
                    queuedEvent.reasonCode = reasonCode;
                    EnqueueEvent(queuedEvent);
                }
            }

            internal void OnWlanNotification(WLan.WlanNotificationData notifyData)
            {
                if (WlanNotification != null)
                    WlanNotification(notifyData);
            }

            /// <summary>
            /// Enqueues a notification event to be processed serially.
            /// </summary>
            private void EnqueueEvent(object queuedEvent)
            {
                lock (eventQueue)
                    eventQueue.Enqueue(queuedEvent);
                eventQueueFilled.Set();
            }

            /// <summary>
            /// Gets the network interface of this wireless interface.
            /// </summary>
            /// <remarks>
            /// The network interface allows querying of generic network properties such as the interface's IP address.
            /// </remarks>
            public NetworkInterface NetworkInterface
            {
                get
                {
                    // Do not cache the NetworkInterface; We need it fresh
                    // each time cause otherwise it caches the IP information.
                    foreach (NetworkInterface netIface in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        var netIfaceGuid = new Guid(netIface.Id);
                        if (netIfaceGuid.Equals(info.interfaceGuid))
                        {
                            return netIface;
                        }
                    }
                    return null;
                }
            }

            /// <summary>
            /// The GUID of the interface (same content as the <see cref="System.Net.NetworkInformation.NetworkInterface.Id"/> value).
            /// </summary>
            public Guid InterfaceGuid => info.interfaceGuid;

            /// <summary>
            /// The description of the interface.
            /// This is a user-immutable string containing the vendor and model name of the adapter.
            /// </summary>
            public string InterfaceDescription => info.interfaceDescription;

            /// <summary>
            /// The friendly name given to the interface by the user (e.g. "Local Area Network Connection").
            /// </summary>
            public string InterfaceName => NetworkInterface.Name;
        }

        private IntPtr clientHandle;
        private readonly WLan.WlanNotificationCallbackDelegate wlanNotificationCallback;
        private readonly Dictionary<Guid, WlanInterface> ifaces = [];

        /// <summary>
        /// Creates a new instance of a Native Wifi service client.
        /// </summary>
        public WlanClient()
        {
            WLan.ThrowIfError(
                WLan.WlanOpenHandle(WLan.WLAN_CLIENT_VERSION_XP_SP2, IntPtr.Zero, out var negotiatedVersion, out clientHandle));
            try
            {
                WLan.WlanNotificationSource prevSrc;
                wlanNotificationCallback = OnWlanNotification;
                WLan.ThrowIfError(
                    WLan.WlanRegisterNotification(clientHandle, WLan.WlanNotificationSource.All, false, wlanNotificationCallback, IntPtr.Zero, IntPtr.Zero, out prevSrc));
            }
            catch
            {
                Close();
                throw;
            }
        }

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
            Close();
        }

        ~WlanClient()
        {
            Close();
        }

        /// <summary>
        /// Closes the handle.
        /// </summary>
        private void Close()
        {
            if (clientHandle != IntPtr.Zero)
            {
                WLan.WlanCloseHandle(clientHandle, IntPtr.Zero);
                clientHandle = IntPtr.Zero;
            }
        }

        private static WLan.WlanConnectionNotificationData? ParseWlanConnectionNotification(ref WLan.WlanNotificationData notifyData)
        {
            var expectedSize = Marshal.SizeOf(typeof(WLan.WlanConnectionNotificationData));
            if (notifyData.dataSize < expectedSize)
                return null;

            var connNotifyData =
                (WLan.WlanConnectionNotificationData)
                Marshal.PtrToStructure(notifyData.dataPtr, typeof(WLan.WlanConnectionNotificationData));
            if (connNotifyData.wlanReasonCode == WLan.WlanReasonCode.Success)
            {
                var profileXmlPtr = new IntPtr(
                    notifyData.dataPtr.ToInt64() +
                    Marshal.OffsetOf(typeof(WLan.WlanConnectionNotificationData), "profileXml").ToInt64());
                connNotifyData.profileXml = Marshal.PtrToStringUni(profileXmlPtr);
            }

            return connNotifyData;
        }

        private void OnWlanNotification(ref WLan.WlanNotificationData notifyData, IntPtr context)
        {
            WlanInterface wlanIface;
            ifaces.TryGetValue(notifyData.interfaceGuid, out wlanIface);

            switch (notifyData.notificationSource)
            {
                case WLan.WlanNotificationSource.ACM:
                    switch ((WLan.WlanNotificationCodeAcm)notifyData.notificationCode)
                    {
                        case WLan.WlanNotificationCodeAcm.ConnectionStart:
                        case WLan.WlanNotificationCodeAcm.ConnectionComplete:
                        case WLan.WlanNotificationCodeAcm.ConnectionAttemptFail:
                        case WLan.WlanNotificationCodeAcm.Disconnecting:
                        case WLan.WlanNotificationCodeAcm.Disconnected:
                            var connNotifyData = ParseWlanConnectionNotification(ref notifyData);
                            if (connNotifyData.HasValue && wlanIface != null)
                                wlanIface.OnWlanConnection(notifyData, connNotifyData.Value);
                            break;
                        case WLan.WlanNotificationCodeAcm.ScanFail:
                            {
                                var expectedSize = Marshal.SizeOf(typeof(int));
                                if (notifyData.dataSize >= expectedSize)
                                {
                                    var reasonCode = (WLan.WlanReasonCode)Marshal.ReadInt32(notifyData.dataPtr);
                                    if (wlanIface != null)
                                        wlanIface.OnWlanReason(notifyData, reasonCode);
                                }
                            }
                            break;
                    }
                    break;
                case WLan.WlanNotificationSource.MSM:
                    switch ((WLan.WlanNotificationCodeMsm)notifyData.notificationCode)
                    {
                        case WLan.WlanNotificationCodeMsm.Associating:
                        case WLan.WlanNotificationCodeMsm.Associated:
                        case WLan.WlanNotificationCodeMsm.Authenticating:
                        case WLan.WlanNotificationCodeMsm.Connected:
                        case WLan.WlanNotificationCodeMsm.RoamingStart:
                        case WLan.WlanNotificationCodeMsm.RoamingEnd:
                        case WLan.WlanNotificationCodeMsm.Disassociating:
                        case WLan.WlanNotificationCodeMsm.Disconnected:
                        case WLan.WlanNotificationCodeMsm.PeerJoin:
                        case WLan.WlanNotificationCodeMsm.PeerLeave:
                        case WLan.WlanNotificationCodeMsm.AdapterRemoval:
                            var connNotifyData = ParseWlanConnectionNotification(ref notifyData);
                            if (connNotifyData.HasValue && wlanIface != null)
                                wlanIface.OnWlanConnection(notifyData, connNotifyData.Value);
                            break;
                    }
                    break;
            }

            if (wlanIface != null)
                wlanIface.OnWlanNotification(notifyData);
        }

        /// <summary>
        /// Gets the WLAN interfaces.
        /// </summary>
        /// <value>The WLAN interfaces.</value>
        public WlanInterface[] Interfaces
        {
            get
            {
                IntPtr ifaceList;
                WLan.ThrowIfError(
                    WLan.WlanEnumInterfaces(clientHandle, IntPtr.Zero, out ifaceList));
                try
                {
                    var header =
                        (WLan.WlanInterfaceInfoListHeader)Marshal.PtrToStructure(ifaceList, typeof(WLan.WlanInterfaceInfoListHeader));
                    long listIterator = ifaceList.ToInt64() + Marshal.SizeOf(header);
                    WlanInterface[] interfaces = new WlanInterface[header.numberOfItems];
                    var currentIfaceGuids = new List<Guid>();
                    for (int i = 0; i < header.numberOfItems; ++i)
                    {
                        var info =
                            (WLan.WlanInterfaceInfo)Marshal.PtrToStructure(new IntPtr(listIterator), typeof(WLan.WlanInterfaceInfo));
                        listIterator += Marshal.SizeOf(info);
                        currentIfaceGuids.Add(info.interfaceGuid);

                        WlanInterface wlanIface;
                        if (!ifaces.TryGetValue(info.interfaceGuid, out wlanIface))
                        {
                            wlanIface = new WlanInterface(this, info);
                            ifaces[info.interfaceGuid] = wlanIface;
                        }

                        interfaces[i] = wlanIface;
                    }

                    // Remove stale interfaces
                    var deadIfacesGuids = new Queue<Guid>();
                    foreach (var ifaceGuid in ifaces.Keys)
                    {
                        if (!currentIfaceGuids.Contains(ifaceGuid))
                            deadIfacesGuids.Enqueue(ifaceGuid);
                    }
                    while (deadIfacesGuids.Count != 0)
                    {
                        var deadIfaceGuid = deadIfacesGuids.Dequeue();
                        ifaces.Remove(deadIfaceGuid);
                    }

                    return interfaces;
                }
                finally
                {
                    WLan.WlanFreeMemory(ifaceList);
                }
            }
        }

        /// <summary>
        /// Gets a string that describes a specified reason code.
        /// </summary>
        /// <param name="reasonCode">The reason code.</param>
        /// <returns>The string.</returns>
        public static string GetStringForReasonCode(WLan.WlanReasonCode reasonCode)
        {
            var sb = new StringBuilder(1024); // the 1024 size here is arbitrary; the WlanReasonCodeToString docs fail to specify a recommended size
            WLan.ThrowIfError(
                WLan.WlanReasonCodeToString(reasonCode, sb.Capacity, sb, IntPtr.Zero));
            return sb.ToString();
        }
    }
}