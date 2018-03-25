namespace ROOT.CIMV2.Win32
{
    using System.Collections;
    using System.ComponentModel;
    using System.Management;

    public class NetworkAdapterConfiguration : System.ComponentModel.Component
    {

        // Private property to hold the WMI namespace in which the class resides.
        private static string CreatedWmiNamespace = "root\\CimV2";

        // Private property to hold the name of WMI class which created this class.
        private static string CreatedClassName = "Win32_NetworkAdapterConfiguration";

        private ManagementSystemProperties PrivateSystemProperties;

        // Underlying lateBound WMI object.
        private ManagementObject PrivateLateBoundObject;

        private readonly ManagementBaseObject embeddedObj;

        // The current WMI object used
        private ManagementBaseObject curObj;

        // Flag to indicate if the instance is an embedded object.
        private bool isEmbedded;

        // Below are different overloads of constructors to initialize an instance of the class with a WMI object.
        public NetworkAdapterConfiguration()
        {
            this.InitializeObject(null, null, null);
        }

        public NetworkAdapterConfiguration(uint keyIndex)
        {
            this.InitializeObject(null, new ManagementPath(NetworkAdapterConfiguration.ConstructPath(keyIndex)), null);
        }

        public NetworkAdapterConfiguration(ManagementScope mgmtScope, uint keyIndex)
        {
            this.InitializeObject(((ManagementScope)(mgmtScope)), new ManagementPath(NetworkAdapterConfiguration.ConstructPath(keyIndex)), null);
        }

        public NetworkAdapterConfiguration(ManagementPath path, System.Management.ObjectGetOptions getOptions)
        {
            this.InitializeObject(null, path, getOptions);
        }

        public NetworkAdapterConfiguration(ManagementScope mgmtScope, ManagementPath path)
        {
            this.InitializeObject(mgmtScope, path, null);
        }

        public NetworkAdapterConfiguration(ManagementPath path)
        {
            this.InitializeObject(null, path, null);
        }

        public NetworkAdapterConfiguration(ManagementScope mgmtScope, ManagementPath path, System.Management.ObjectGetOptions getOptions)
        {
            this.InitializeObject(mgmtScope, path, getOptions);
        }

        public NetworkAdapterConfiguration(ManagementObject theObject)
        {
            Initialize();
            if ((CheckIfProperClass(theObject) == true))
            {
                PrivateLateBoundObject = theObject;
                PrivateSystemProperties = new ManagementSystemProperties(PrivateLateBoundObject);
                curObj = PrivateLateBoundObject;
            }
            else
            {
                throw new System.ArgumentException("Class name does not match.");
            }
        }

        public NetworkAdapterConfiguration(ManagementBaseObject theObject)
        {
            Initialize();
            if ((CheckIfProperClass(theObject) == true))
            {
                embeddedObj = theObject;
                PrivateSystemProperties = new ManagementSystemProperties(theObject);
                curObj = embeddedObj;
                isEmbedded = true;
            }
            else
            {
                throw new System.ArgumentException("Class name does not match.");
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ManagementClassName
        {
            get
            {
                string strRet = CreatedClassName;
                if ((curObj != null) && (curObj.ClassPath != null))
                {
                    strRet = ((string)(curObj["__CLASS"]));
                    if (string.IsNullOrEmpty(strRet))
                    {
                        strRet = CreatedClassName;
                    }
                }
                return strRet;
            }
        }

        // Property pointing to an embedded object to get System properties of the WMI object.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ManagementSystemProperties SystemProperties
        {
            get
            {
                return PrivateSystemProperties;
            }
        }

        // Property returning the underlying lateBound object.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ManagementBaseObject LateBoundObject
        {
            get
            {
                return curObj;
            }
        }

        // ManagementScope of the object.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ManagementScope Scope
        {
            get
            {
                if ((isEmbedded == false))
                {
                    return PrivateLateBoundObject.Scope;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if ((isEmbedded == false))
                {
                    PrivateLateBoundObject.Scope = value;
                }
            }
        }

        // Property to show the commit behavior for the WMI object. If true, WMI object will be automatically saved after each
        // property modification.(ie. Put() is called after modification of a property).
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AutoCommit { get; set; }


        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The DefaultIPGateway property contains a list of IP addresses of default gateways" +
            " used by the computer system.\nExample: 194.161.12.1 194.162.46.1")]
        public string[] DefaultIPGateway
        {
            get
            {
                return ((string[])(curObj["DefaultIPGateway"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("A textual description of the CIM_Setting object.")]
        public string Description
        {
            get
            {
                return ((string)(curObj["Description"]));
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDHCPEnabledNull
        {
            get
            {
                if ((curObj["DHCPEnabled"] == null))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The DHCPEnabled property indicates whether the dynamic host configuration protoco" +
            "l  (DHCP) server automatically assigns an IP address to the computer system when" +
            " establishing a network connection.\nValues: TRUE or FALSE. If TRUE, DHCP is enab" +
            "led.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool DHCPEnabled
        {
            get
            {
                if ((curObj["DHCPEnabled"] == null))
                {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["DHCPEnabled"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The DHCPLeaseExpires property indicates the expiration date and time for a leased" +
            " IP address that was assigned to the computer by the dynamic host configuration " +
            "protocol (DHCP) server.\nExample: 20521201000230.000000000")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public System.DateTime DHCPLeaseExpires
        {
            get
            {
                if ((curObj["DHCPLeaseExpires"] != null))
                {
                    return ToDateTime(((string)(curObj["DHCPLeaseExpires"])));
                }
                else
                {
                    return System.DateTime.MinValue;
                }
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The DHCPLeaseObtained property indicates the date and time the lease was obtained" +
            " for the IP address assigned to the computer by the dynamic host configuration p" +
            "rotocol (DHCP) server. \nExample: 19521201000230.000000000")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public System.DateTime DHCPLeaseObtained
        {
            get
            {
                if ((curObj["DHCPLeaseObtained"] != null))
                {
                    return ToDateTime(((string)(curObj["DHCPLeaseObtained"])));
                }
                else
                {
                    return System.DateTime.MinValue;
                }
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The DHCPServer property indicates the IP address of the dynamic host configuratio" +
            "n protocol (DHCP) server.\nExample: 154.55.34")]
        public string DHCPServer
        {
            get
            {
                return ((string)(curObj["DHCPServer"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The DNSDomain property indicates an organization name followed by a period and an extension that indicates the type of " +
                     @"organization, such as microsoft.com. The name can be any combination of the letters A through Z, the num" +
                     @"erals 0 through 9, and the hyphen (-), plus the period (.) character used as a separator. Example: microsoft.com")]
        public string DNSDomain
        {
            get
            {
                return ((string)(curObj["DNSDomain"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The DNSDomainSuffixSearchOrder property specifies the DNS domain suffixes to be appended to the end of host names during name resolution. When " +
                     @"attempting to resolve a fully qualified domain name (FQDN) from a host only name, the system will first append the local domain name. If this is not " +
                     @"successful, the system will use the domain suffix list to create additional FQDNs in the order listed and query DNS servers for each. " +
                     @"Example: samples.microsoft.com example.microsoft.com")]
        public string[] DNSDomainSuffixSearchOrder
        {
            get
            {
                return ((string[])(curObj["DNSDomainSuffixSearchOrder"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The DNSHostName property indicates the host name used to identify the local computer for authentication by some utilities. Other " +
            @"TCP/IP-based utilities can use this value to acquire the name of the local computer. Host names are stored on DNS servers in a table that maps " +
            @"names to IP addresses for use by DNS. The name can be any combination of the letters A through Z, the numerals 0 through 9, and the hyphen (-)," +
            @"plus the period (.) character used as a separator. By default, this value is the Microsoft networking computer name, but the network administrator " +
            @"can assign another host name without affecting the computer name. Example: corpdns")]
        public string DNSHostName
        {
            get
            {
                return ((string)(curObj["DNSHostName"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The DNSServerSearchOrder property indicates an ordered list of server IP addresse" +
            "s to be used in querying for DNS Servers.")]
        public string[] DNSServerSearchOrder
        {
            get
            {
                return ((string[])(curObj["DNSServerSearchOrder"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The GatewayCostMetric reflects an integer cost metric (ranging from 1 to 9999) to" +
            " be used in calculating the fastest, most reliable, and/or least expensive route" +
            "s. This argument has a one to one correspondence with the DefaultIPGateway. Wind" +
            "ows 2000 only.")]
        public ushort[] GatewayCostMetric
        {
            get
            {
                return ((ushort[])(curObj["GatewayCostMetric"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The IPAddress property contains a list of all of the IP addresses associated with" +
            " the current network adapter.\nExample: 155.34.22.0")]
        public string[] IPAddress
        {
            get
            {
                return ((string[])(curObj["IPAddress"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The InterfaceIndex property contains the index value that uniquely identifies the" +
            " local interface.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint InterfaceIndex
        {
            get
            {
                if ((curObj["InterfaceIndex"] == null))
                {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["InterfaceIndex"]));
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsIPEnabledNull
        {
            get
            {
                if ((curObj["IPEnabled"] == null))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The IPEnabled property indicates whether TCP/IP is bound and enabled on this netw" +
            "ork adapt.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool IPEnabled
        {
            get
            {
                if ((curObj["IPEnabled"] == null))
                {
                    return System.Convert.ToBoolean(0);
                }
                return ((bool)(curObj["IPEnabled"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The IPSubnet property contains a list of all the subnet masks associated with the" +
            " current network adapter.\nExample: 255.255.0")]
        public string[] IPSubnet
        {
            get
            {
                return ((string[])(curObj["IPSubnet"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The MACAddress property indicates the Media Access Control (MAC) address of the n" +
            "etwork adapter. A MAC address is assigned by the manufacturer to uniquely identi" +
            "fy the network adapter.\nExample: 00:80:C7:8F:6C:96")]
        public string MACAddress
        {
            get
            {
                return ((string)(curObj["MACAddress"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The WINSPrimaryServer property indicates the IP address for the primary WINS serv" +
            "er. ")]
        public string WINSPrimaryServer
        {
            get
            {
                return ((string)(curObj["WINSPrimaryServer"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The WINSSecondaryServer property indicates the IP address for the secondary WINS " +
            "server. ")]
        public string WINSSecondaryServer
        {
            get
            {
                return ((string)(curObj["WINSSecondaryServer"]));
            }
        }

        private bool CheckIfProperClass(ManagementScope mgmtScope, ManagementPath path, System.Management.ObjectGetOptions OptionsParam)
        {
            if (((path != null)
                        && (string.Compare(path.ClassName, this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)))
            {
                return true;
            }
            else
            {
                return CheckIfProperClass(new ManagementObject(mgmtScope, path, OptionsParam));
            }
        }

        private bool CheckIfProperClass(ManagementBaseObject theObj)
        {
            if (((theObj != null)
                        && (string.Compare(((string)(theObj["__CLASS"])), this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)))
            {
                return true;
            }

            return false;
        }

        // Converts a given datetime in DMTF format to System.DateTime object.
        static System.DateTime ToDateTime(string dmtfDate)
        {
            System.DateTime initializer = System.DateTime.MinValue;
            int year = initializer.Year;
            int month = initializer.Month;
            int day = initializer.Day;
            int hour = initializer.Hour;
            int minute = initializer.Minute;
            int second = initializer.Second;
            long ticks = 0;
            string dmtf = dmtfDate;
            System.DateTime datetime;
            string tempString = string.Empty;
            if ((dmtf == null))
            {
                throw new System.ArgumentOutOfRangeException(nameof(dmtfDate));
            }
            if ((dmtf.Length == 0))
            {
                throw new System.ArgumentOutOfRangeException(nameof(dmtfDate));
            }
            if ((dmtf.Length != 25))
            {
                throw new System.ArgumentOutOfRangeException(nameof(dmtfDate));
            }
            try
            {
                tempString = dmtf.Substring(0, 4);
                if (("****" != tempString))
                {
                    year = int.Parse(tempString);
                }
                tempString = dmtf.Substring(4, 2);
                if (("**" != tempString))
                {
                    month = int.Parse(tempString);
                }
                tempString = dmtf.Substring(6, 2);
                if (("**" != tempString))
                {
                    day = int.Parse(tempString);
                }
                tempString = dmtf.Substring(8, 2);
                if (("**" != tempString))
                {
                    hour = int.Parse(tempString);
                }
                tempString = dmtf.Substring(10, 2);
                if (("**" != tempString))
                {
                    minute = int.Parse(tempString);
                }
                tempString = dmtf.Substring(12, 2);
                if (("**" != tempString))
                {
                    second = int.Parse(tempString);
                }
                tempString = dmtf.Substring(15, 6);
                if (("******" != tempString))
                {
                    ticks = (long.Parse(tempString) * ((long)((System.TimeSpan.TicksPerMillisecond / 1000))));
                }
                if (((((((((year < 0)
                            || (month < 0))
                            || (day < 0))
                            || (hour < 0))
                            || (minute < 0))
                            || (minute < 0))
                            || (second < 0))
                            || (ticks < 0)))
                {
                    throw new System.ArgumentOutOfRangeException(nameof(dmtfDate));
                }
            }
            catch (System.Exception e)
            {
                throw new System.ArgumentOutOfRangeException(null, e.Message);
            }
            datetime = new System.DateTime(year, month, day, hour, minute, second, 0);
            datetime = datetime.AddTicks(ticks);
            System.TimeSpan tickOffset = System.TimeZone.CurrentTimeZone.GetUtcOffset(datetime);
            int UTCOffset = 0;
            int OffsetToBeAdjusted = 0;
            long OffsetMins = ((long)((tickOffset.Ticks / System.TimeSpan.TicksPerMinute)));
            tempString = dmtf.Substring(22, 3);
            if ((tempString != "******"))
            {
                tempString = dmtf.Substring(21, 4);
                try
                {
                    UTCOffset = int.Parse(tempString);
                }
                catch (System.Exception e)
                {
                    throw new System.ArgumentOutOfRangeException(null, e.Message);
                }
                OffsetToBeAdjusted = ((int)((OffsetMins - UTCOffset)));
                datetime = datetime.AddMinutes(((double)(OffsetToBeAdjusted)));
            }
            return datetime;
        }

        [Browsable(true)]
        public void CommitObject()
        {
            if ((isEmbedded == false))
            {
                PrivateLateBoundObject.Put();
            }
        }

        [Browsable(true)]
        public void CommitObject(System.Management.PutOptions putOptions)
        {
            if ((isEmbedded == false))
            {
                PrivateLateBoundObject.Put(putOptions);
            }
        }

        private void Initialize()
        {
            AutoCommit = true;
            isEmbedded = false;
        }

        private static string ConstructPath(uint keyIndex)
        {
            string strPath = "root\\CimV2:Win32_NetworkAdapterConfiguration";
            strPath = string.Concat(strPath, string.Concat(".Index=", ((System.UInt32)(keyIndex)).ToString()));
            return strPath;
        }

        private void InitializeObject(ManagementScope mgmtScope, ManagementPath path, System.Management.ObjectGetOptions getOptions)
        {
            Initialize();
            if (path != null && CheckIfProperClass(mgmtScope, path, getOptions) != true)
            {
                throw new System.ArgumentException("Class name does not match.");
            }
            PrivateLateBoundObject = new ManagementObject(mgmtScope, path, getOptions);
            PrivateSystemProperties = new ManagementSystemProperties(PrivateLateBoundObject);
            curObj = PrivateLateBoundObject;
        }

        public static NetworkAdapterConfigurationCollection GetInstances()
        {
            return GetInstances(null, null, null);
        }

        public static NetworkAdapterConfigurationCollection GetInstances(ManagementScope mgmtScope, EnumerationOptions enumOptions)
        {
            if ((mgmtScope == null))
            {
                mgmtScope = new ManagementScope();
                mgmtScope.Path.NamespacePath = "root\\CimV2";
            }
            var pathObj = new ManagementPath
            {
                ClassName = "Win32_NetworkAdapterConfiguration",
                NamespacePath = "root\\CimV2"
            };
            var clsObject = new ManagementClass(mgmtScope, pathObj, null);
            if ((enumOptions == null))
            {
                enumOptions = new EnumerationOptions
                {
                    EnsureLocatable = true
                };
            }
            return new NetworkAdapterConfigurationCollection(clsObject.GetInstances(enumOptions));
        }

        public static NetworkAdapterConfigurationCollection GetInstances(ManagementScope mgmtScope, string condition, System.String[] selectedProperties)
        {
            if ((mgmtScope == null))
            {
                mgmtScope = new ManagementScope();
                mgmtScope.Path.NamespacePath = "root\\CimV2";
            }
            var ObjectSearcher = new ManagementObjectSearcher(mgmtScope, new SelectQuery("Win32_NetworkAdapterConfiguration", condition, selectedProperties));
            var enumOptions = new EnumerationOptions
            {
                EnsureLocatable = true
            };
            ObjectSearcher.Options = enumOptions;
            return new NetworkAdapterConfigurationCollection(ObjectSearcher.Get());
        }

        [Browsable(true)]
        public static NetworkAdapterConfiguration CreateInstance()
        {
            ManagementScope mgmtScope = null;
            mgmtScope = new ManagementScope();
            mgmtScope.Path.NamespacePath = CreatedWmiNamespace;

            var mgmtPath = new ManagementPath(CreatedClassName);
            var tmpMgmtClass = new ManagementClass(mgmtScope, mgmtPath, null);
            return new NetworkAdapterConfiguration(tmpMgmtClass.CreateInstance());
        }

        [Browsable(true)]
        public void Delete()
        {
            PrivateLateBoundObject.Delete();
        }

        public uint DisableIPSec()
        {
            if ((isEmbedded == false))
            {
                ManagementBaseObject inParams = null;
                ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("DisableIPSec", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint EnableDHCP()
        {
            if ((isEmbedded == false))
            {
                ManagementBaseObject inParams = null;
                ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("EnableDHCP", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint EnableDNS()
        {
            if ((isEmbedded == false))
            {
                ManagementBaseObject inParams = null;
                var mgmtPath = new ManagementPath(CreatedClassName);
                var classObj = new ManagementClass(null, mgmtPath, null);
                inParams = classObj.GetMethodParameters("EnableDNS");
                inParams["DNSDomain"] = ((System.String)(DNSDomain));
                inParams["DNSDomainSuffixSearchOrder"] = ((string[])(DNSDomainSuffixSearchOrder));
                inParams["DNSHostName"] = ((System.String)(DNSHostName));
                inParams["DNSServerSearchOrder"] = ((string[])(DNSServerSearchOrder));
                ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("EnableDNS", inParams, null);

                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint EnableStatic(string[] IPAddress, string[] SubnetMask)
        {
            if ((isEmbedded == false))
            {
                ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("EnableStatic");
                inParams["IPAddress"] = ((string[])(IPAddress));
                inParams["SubnetMask"] = ((string[])(SubnetMask));
                ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("EnableStatic", inParams, null);

                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint ReleaseDHCPLease()
        {
            if ((isEmbedded == false))
            {
                ManagementBaseObject inParams = null;
                ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("ReleaseDHCPLease", inParams, null);

                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint RenewDHCPLease()
        {
            if ((isEmbedded == false))
            {
                ManagementBaseObject inParams = null;
                ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("RenewDHCPLease", inParams, null);

                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint SetDNSServerSearchOrder(string[] DNSServerSearchOrder)
        {
            if ((isEmbedded == false))
            {
                ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("SetDNSServerSearchOrder");
                inParams["DNSServerSearchOrder"] = ((string[])(DNSServerSearchOrder));
                ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("SetDNSServerSearchOrder", inParams, null);

                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        public uint SetGateways(string[] DefaultIPGateway)
        {
            if ((isEmbedded == false))
            {
                ManagementBaseObject inParams = null;
                inParams = PrivateLateBoundObject.GetMethodParameters("SetGateways");
                inParams["DefaultIPGateway"] = DefaultIPGateway;
                ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("SetGateways", inParams, null);

                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return System.Convert.ToUInt32(0);
            }
        }

        // Enumerator implementation for enumerating instances of the class.
        public class NetworkAdapterConfigurationCollection : object, ICollection
        {
            private readonly ManagementObjectCollection privColObj;

            public NetworkAdapterConfigurationCollection(ManagementObjectCollection objCollection)
            {
                privColObj = objCollection;
            }

            public virtual int Count
            {
                get
                {
                    return privColObj.Count;
                }
            }

            public virtual bool IsSynchronized
            {
                get
                {
                    return privColObj.IsSynchronized;
                }
            }

            public virtual object SyncRoot
            {
                get
                {
                    return this;
                }
            }

            public virtual void CopyTo(System.Array array, int index)
            {
                privColObj.CopyTo(array, index);
                int nCtr;
                for (nCtr = 0; (nCtr < array.Length); nCtr = (nCtr + 1))
                {
                    array.SetValue(new NetworkAdapterConfiguration(((ManagementObject)(array.GetValue(nCtr)))), nCtr);
                }
            }

            public virtual IEnumerator GetEnumerator()
            {
                return new NetworkAdapterConfigurationEnumerator(privColObj.GetEnumerator());
            }

            public class NetworkAdapterConfigurationEnumerator : object, IEnumerator
            {
                private readonly ManagementObjectCollection.ManagementObjectEnumerator privObjEnum;

                public NetworkAdapterConfigurationEnumerator(ManagementObjectCollection.ManagementObjectEnumerator objEnum)
                {
                    privObjEnum = objEnum;
                }

                public virtual object Current
                {
                    get
                    {
                        return new NetworkAdapterConfiguration(((ManagementObject)(privObjEnum.Current)));
                    }
                }

                public virtual bool MoveNext()
                {
                    return privObjEnum.MoveNext();
                }

                public virtual void Reset()
                {
                    privObjEnum.Reset();
                }
            }
        }
    }
}
