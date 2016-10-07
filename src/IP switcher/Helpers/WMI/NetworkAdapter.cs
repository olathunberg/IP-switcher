namespace ROOT.CIMV2.Win32
{
    using System;
    using System.ComponentModel;
    using System.Management;
    using System.Collections;
    using System.Globalization;
    using System.Threading.Tasks;


    // Functions ShouldSerialize<PropertyName> are functions used by VS property browser to check if a particular property has to be serialized. These functions are added for all ValueType properties ( properties of type Int32, BOOL etc.. which cannot be set to null). These functions use Is<PropertyName>Null function. These functions are also used in the TypeConverter implementation for the properties to check for NULL value of property so that an empty value can be shown in Property browser in case of Drag and Drop in Visual studio.
    // Functions Is<PropertyName>Null() are used to check if a property is NULL.
    // Functions Reset<PropertyName> are added for Nullable Read/Write properties. These functions are used by VS designer in property browser to set a property to NULL.
    // Every property added to the class for WMI property has attributes set to define its behavior in Visual Studio designer and also to define a TypeConverter to be used.
    // Datetime conversion functions ToDateTime and ToDmtfDateTime are added to the class to convert DMTF datetime to System.DateTime and vice-versa.
    // An Early Bound class generated for the WMI class.Win32_NetworkAdapter
    public class NetworkAdapter : System.ComponentModel.Component
    {
        // Private property to hold the WMI namespace in which the class resides.
        private static string CreatedWmiNamespace = "root\\CimV2";

        // Private property to hold the name of WMI class which created this class.
        private static string CreatedClassName = "Win32_NetworkAdapter";

        // Private member variable to hold the ManagementScope which is used by the various methods.
        private static ManagementScope statMgmtScope = null;

        private ManagementSystemProperties PrivateSystemProperties;

        // Underlying lateBound WMI object.
        private ManagementObject PrivateLateBoundObject;

        // Member variable to store the 'automatic commit' behavior for the class.
        private bool AutoCommitProp;

        // Private variable to hold the embedded property representing the instance.
        private readonly ManagementBaseObject embeddedObj;

        // The current WMI object used
        private ManagementBaseObject curObj;

        // Flag to indicate if the instance is an embedded object.
        private bool isEmbedded;

        // Below are different overloads of constructors to initialize an instance of the class with a WMI object.
        public NetworkAdapter()
        {
            this.InitializeObject(null, null, null);
        }

        public NetworkAdapter(string keyDeviceID)
        {
            this.InitializeObject(null, new ManagementPath(NetworkAdapter.ConstructPath(keyDeviceID)), null);
        }

        public NetworkAdapter(ManagementScope mgmtScope, string keyDeviceID)
        {
            this.InitializeObject(((ManagementScope)(mgmtScope)), new ManagementPath(NetworkAdapter.ConstructPath(keyDeviceID)), null);
        }

        public NetworkAdapter(ManagementPath path, ObjectGetOptions getOptions)
        {
            this.InitializeObject(null, path, getOptions);
        }

        public NetworkAdapter(ManagementScope mgmtScope, ManagementPath path)
        {
            this.InitializeObject(mgmtScope, path, null);
        }

        public NetworkAdapter(ManagementPath path)
        {
            this.InitializeObject(null, path, null);
        }

        public NetworkAdapter(ManagementScope mgmtScope, ManagementPath path, ObjectGetOptions getOptions)
        {
            this.InitializeObject(mgmtScope, path, getOptions);
        }

        public NetworkAdapter(ManagementObject theObject)
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
                throw new ArgumentException("Class name does not match.");
            }
        }

        public NetworkAdapter(ManagementBaseObject theObject)
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
                throw new ArgumentException("Class name does not match.");
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ManagementClassName
        {
            get
            {
                string strRet = CreatedClassName;
                if ((curObj != null))
                {
                    if ((curObj.ClassPath != null))
                    {
                        strRet = ((string)(curObj["__CLASS"]));
                        if ((string.IsNullOrEmpty(strRet)))
                        {
                            strRet = CreatedClassName;
                        }
                    }
                }
                return strRet;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsConfigManagerErrorCodeNull
        {
            get
            {
                if ((curObj["ConfigManagerErrorCode"] == null))
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
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ConfigManagerErrorCodeValues ConfigManagerErrorCode
        {
            get
            {
                if ((curObj["ConfigManagerErrorCode"] == null))
                {
                    return ((ConfigManagerErrorCodeValues)(Convert.ToInt32(32)));
                }
                return ((ConfigManagerErrorCodeValues)(Convert.ToInt32(curObj["ConfigManagerErrorCode"])));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The Description property provides a textual description of the object. ")]
        public string Description
        {
            get
            {
                return ((string)(curObj["Description"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The GUID property specifies the Globally-unique identifier for the connection.")]
        public string GUID
        {
            get
            {
                return ((string)(curObj["GUID"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The MACAddress property indicates the media access control address for this network adapter. A MAC address is a unique 48-bit number assigned to the network adapter by the manufacturer. It uniquely identifies this network adapter and is used for mapping TCP/IP network communications.")]
        public string MACAddress
        {
            get
            {
                return ((string)(curObj["MACAddress"]));
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
                    return Convert.ToUInt32(0);
                }
                return ((uint)(curObj["InterfaceIndex"]));
            }
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMaxSpeedNull
        {
            get
            {
                if ((curObj["MaxSpeed"] == null))
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
        [Description("The maximum speed, in bits per second, for the network adapter.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong MaxSpeed
        {
            get
            {
                if ((curObj["MaxSpeed"] == null))
                {
                    return Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["MaxSpeed"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The Name property defines the label by which the object is known. When subclassed" +
            ", the Name property can be overridden to be a Key property.")]
        public string Name
        {
            get
            {
                return ((string)(curObj["Name"]));
            }
        }
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The NetConnectionID property specifies the name of the network connection as it a" +
            "ppears in the \'Network Connections\' folder.")]
        public string NetConnectionID
        {
            get
            {
                return ((string)(curObj["NetConnectionID"]));
            }
            set
            {
                curObj["NetConnectionID"] = value;
                if (((isEmbedded == false)
                            && (AutoCommitProp == true)))
                {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsNetConnectionStatusNull
        {
            get
            {
                if ((curObj["NetConnectionStatus"] == null))
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
        [Description(@"NetConnectionStatus is a string indicating the state of the network adapter's connection to the network. The value of the property is to be interpreted as follows:
0 - Disconnected
1 - Connecting
2 - Connected
3 - Disconnecting
4 - Hardware not present
5 - Hardware disabled
6 - Hardware malfunction
7 - Media disconnected
8 - Authenticating
9 - Authentication succeeded
10 - Authentication failed
11 - Invalid Address
12 - Credentials Required
.. - Other - For integer values other than those listed above, refer to Win32 error code documentation.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ushort NetConnectionStatus
        {
            get
            {
                if ((curObj["NetConnectionStatus"] == null))
                {
                    return Convert.ToUInt16(0);
                }
                return ((ushort)(curObj["NetConnectionStatus"]));
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsNetEnabledNull
        {
            get
            {
                if ((curObj["NetEnabled"] == null))
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
        [Description("The NetEnabled property specifies whether the network connection is enabled or no" +
            "t.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool NetEnabled
        {
            get
            {
                if ((curObj["NetEnabled"] == null))
                {
                    return Convert.ToBoolean(0);
                }
                return ((bool)(curObj["NetEnabled"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("An array of strings indicating the network addresses for an adapter.")]
        public string[] NetworkAddresses
        {
            get
            {
                return ((string[])(curObj["NetworkAddresses"]));
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPhysicalAdapterNull
        {
            get
            {
                if ((curObj["PhysicalAdapter"] == null))
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
        [Description("The PhysicalAdapter property specifies whether the adapter is physical adapter or" +
            " logical adapter.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public bool PhysicalAdapter
        {
            get
            {
                if ((curObj["PhysicalAdapter"] == null))
                {
                    return Convert.ToBoolean(0);
                }
                return ((bool)(curObj["PhysicalAdapter"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The ProductName property indicates the product name of the network adapter.\nExamp" +
            "le: Fast EtherLink XL")]
        public string ProductName
        {
            get
            {
                return ((string)(curObj["ProductName"]));
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSpeedNull
        {
            get
            {
                if ((curObj["Speed"] == null))
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
        [Description("An estimate of the current bandwidth in bits per second. For endpoints which vary" +
            " in bandwidth or for those where no accurate estimation can be made, this proper" +
            "ty should contain the nominal bandwidth.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong Speed
        {
            get
            {
                if ((curObj["Speed"] == null))
                {
                    return Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["Speed"]));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The Status property is a string indicating the current status of the object. Various operational and non-operational statuses can be defined. Operational statuses are ""OK"", ""Degraded"" and ""Pred Fail"". ""Pred Fail"" indicates that an element may be functioning properly but predicting a failure in the near future. An example is a SMART-enabled hard drive. Non-operational statuses can also be specified. These are ""Error"", ""Starting"", ""Stopping"" and ""Service"". The latter, ""Service"", could apply during mirror-resilvering of a disk, reload of a user permissions list, or other administrative work. Not all such work is on-line, yet the managed element is neither ""OK"" nor in one of the other states.")]
        public string Status
        {
            get
            {
                return ((string)(curObj["Status"]));
            }
        }

        private bool CheckIfProperClass(ManagementScope mgmtScope, ManagementPath path, ObjectGetOptions OptionsParam)
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
            else
            {
                Array parentClasses = ((Array)(theObj["__DERIVATION"]));
                if ((parentClasses != null))
                {
                    int count = 0;
                    for (count = 0; (count < parentClasses.Length); count = (count + 1))
                    {
                        if ((string.Compare(((string)(parentClasses.GetValue(count))), this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // Converts a given datetime in DMTF format to System.DateTime object.
        static DateTime ToDateTime(string dmtfDate)
        {
            DateTime initializer = DateTime.MinValue;
            int year = initializer.Year;
            int month = initializer.Month;
            int day = initializer.Day;
            int hour = initializer.Hour;
            int minute = initializer.Minute;
            int second = initializer.Second;
            long ticks = 0;
            string dmtf = dmtfDate;
            string tempString = string.Empty;

            if ((dmtf == null))
            {
                throw new ArgumentOutOfRangeException();
            }
            if ((dmtf.Length == 0))
            {
                throw new ArgumentOutOfRangeException();
            }
            if ((dmtf.Length != 25))
            {
                throw new ArgumentOutOfRangeException();
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
                    ticks = (long.Parse(tempString) * ((long)((TimeSpan.TicksPerMillisecond / 1000))));
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
                    throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                throw new ArgumentOutOfRangeException(null, e.Message);
            }

            var datetime = new DateTime(year, month, day, hour, minute, second, 0);
            datetime = datetime.AddTicks(ticks);
            System.TimeSpan tickOffset = System.TimeZone.CurrentTimeZone.GetUtcOffset(datetime);
            int UTCOffset = 0;
            int OffsetToBeAdjusted = 0;
            long OffsetMins = ((long)((tickOffset.Ticks / TimeSpan.TicksPerMinute)));
            tempString = dmtf.Substring(22, 3);
            if ((tempString != "******"))
            {
                tempString = dmtf.Substring(21, 4);
                try
                {
                    UTCOffset = int.Parse(tempString);
                }
                catch (Exception e)
                {
                    throw new ArgumentOutOfRangeException(null, e.Message);
                }
                OffsetToBeAdjusted = ((int)((OffsetMins - UTCOffset)));
                datetime = datetime.AddMinutes(((double)(OffsetToBeAdjusted)));
            }
            return datetime;
        }

        // Converts a given System.DateTime object to DMTF datetime format.
        static string ToDmtfDateTime(DateTime date)
        {
            string utcString = string.Empty;
            System.TimeSpan tickOffset = System.TimeZone.CurrentTimeZone.GetUtcOffset(date);
            long OffsetMins = ((long)((tickOffset.Ticks / TimeSpan.TicksPerMinute)));
            if ((System.Math.Abs(OffsetMins) > 999))
            {
                date = date.ToUniversalTime();
                utcString = "+000";
            }
            else
            {
                if ((tickOffset.Ticks >= 0))
                {
                    utcString = string.Concat("+", ((long)((tickOffset.Ticks / TimeSpan.TicksPerMinute))).ToString().PadLeft(3, '0'));
                }
                else
                {
                    string strTemp = ((long)(OffsetMins)).ToString();
                    utcString = string.Concat("-", strTemp.Substring(1, (strTemp.Length - 1)).PadLeft(3, '0'));
                }
            }
            string dmtfDateTime = ((int)(date.Year)).ToString().PadLeft(4, '0');
            dmtfDateTime = string.Concat(dmtfDateTime, ((int)(date.Month)).ToString().PadLeft(2, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, ((int)(date.Day)).ToString().PadLeft(2, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, ((int)(date.Hour)).ToString().PadLeft(2, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, ((int)(date.Minute)).ToString().PadLeft(2, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, ((int)(date.Second)).ToString().PadLeft(2, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, ".");
            var dtTemp = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, 0);
            long microsec = ((long)((((date.Ticks - dtTemp.Ticks)
                        * 1000)
                        / TimeSpan.TicksPerMillisecond)));
            string strMicrosec = ((long)(microsec)).ToString();
            if ((strMicrosec.Length > 6))
            {
                strMicrosec = strMicrosec.Substring(0, 6);
            }
            dmtfDateTime = string.Concat(dmtfDateTime, strMicrosec.PadLeft(6, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, utcString);
            return dmtfDateTime;
        }

        private void ResetNetConnectionID()
        {
            curObj["NetConnectionID"] = null;
            if (((isEmbedded == false)
                        && (AutoCommitProp == true)))
            {
                PrivateLateBoundObject.Put();
            }
        }

        [Browsable(true)]
        public void CommitObject()
        {
            if ((isEmbedded == false))
            {
                PrivateLateBoundObject.Put();
            }
        }

        private void Initialize()
        {
            AutoCommitProp = true;
            isEmbedded = false;
        }

        private static string ConstructPath(string keyDeviceID)
        {
            string strPath = "root\\CimV2:Win32_NetworkAdapter";
            strPath = string.Concat(strPath, string.Concat(".DeviceID=", string.Concat("\"", string.Concat(keyDeviceID, "\""))));
            return strPath;
        }

        private void InitializeObject(ManagementScope mgmtScope, ManagementPath path, ObjectGetOptions getOptions)
        {
            Initialize();
            if ((path != null))
            {
                if ((CheckIfProperClass(mgmtScope, path, getOptions) != true))
                {
                    throw new ArgumentException("Class name does not match.");
                }
            }
            PrivateLateBoundObject = new ManagementObject(mgmtScope, path, getOptions);
            PrivateSystemProperties = new ManagementSystemProperties(PrivateLateBoundObject);
            curObj = PrivateLateBoundObject;
        }

        // Different overloads of GetInstances() help in enumerating instances of the WMI class.
        public static NetworkAdapterCollection GetInstances()
        {
            return GetInstances(null, null, null);
        }

        public static NetworkAdapterCollection GetInstances(ManagementScope mgmtScope, string condition, System.String[] selectedProperties)
        {
            if ((mgmtScope == null))
            {
                if ((statMgmtScope == null))
                {
                    mgmtScope = new ManagementScope();
                    mgmtScope.Path.NamespacePath = "root\\CimV2";
                }
                else
                {
                    mgmtScope = statMgmtScope;
                }
            }
            var ObjectSearcher = new ManagementObjectSearcher(mgmtScope, new SelectQuery("Win32_NetworkAdapter", condition, selectedProperties));
            var enumOptions = new EnumerationOptions();
            enumOptions.EnsureLocatable = true;
            ObjectSearcher.Options = enumOptions;
            return new NetworkAdapterCollection(ObjectSearcher.Get());
        }

        [Browsable(true)]
        public static NetworkAdapter CreateInstance()
        {
            ManagementScope mgmtScope = null;
            if ((statMgmtScope == null))
            {
                mgmtScope = new ManagementScope();
                mgmtScope.Path.NamespacePath = CreatedWmiNamespace;
            }
            else
            {
                mgmtScope = statMgmtScope;
            }
            var mgmtPath = new ManagementPath(CreatedClassName);
            var tmpMgmtClass = new ManagementClass(mgmtScope, mgmtPath, null);
            return new NetworkAdapter(tmpMgmtClass.CreateInstance());
        }

        [Browsable(true)]
        public void Delete()
        {
            PrivateLateBoundObject.Delete();
        }

        public async Task<uint> DisableAsync()
        {
            if ((isEmbedded == false))
            {
                ManagementBaseObject inParams = null;
                var observer = new ManagementOperationObserver();
                var tcs = new TaskCompletionSource<uint>();
                observer.ObjectReady += (sender, e) =>
                    {
                        tcs.SetResult(Convert.ToUInt32(e.NewObject.Properties["ReturnValue"].Value));
                    };

                PrivateLateBoundObject.InvokeMethod(observer, "Disable", inParams, null);

                return await tcs.Task;
            }
            else
            {
                return 0;
            }
        }

        public async Task<uint> EnableAsync()
        {
            if ((isEmbedded == false))
            {
                ManagementBaseObject inParams = null;
                var observer = new ManagementOperationObserver();
                var tcs = new TaskCompletionSource<uint>();
                observer.ObjectReady += (sender, e) =>
                    {
                        tcs.SetResult(Convert.ToUInt32(e.NewObject.Properties["ReturnValue"].Value));
                    };

                PrivateLateBoundObject.InvokeMethod(observer, "Enable", inParams, null);

                return await tcs.Task;
            }
            else
            {
                return Convert.ToUInt32(0);
            }
        }

        public uint Reset()
        {
            if ((isEmbedded == false))
            {
                ManagementBaseObject inParams = null;
                ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("Reset", inParams, null);
                return Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else
            {
                return Convert.ToUInt32(0);
            }
        }

        public enum ConfigManagerErrorCodeValues
        {

            This_device_is_working_properly_ = 0,

            This_device_is_not_configured_correctly_ = 1,

            Windows_cannot_load_the_driver_for_this_device_ = 2,

            The_driver_for_this_device_might_be_corrupted_or_your_system_may_be_running_low_on_memory_or_other_resources_ = 3,

            This_device_is_not_working_properly_One_of_its_drivers_or_your_registry_might_be_corrupted_ = 4,

            The_driver_for_this_device_needs_a_resource_that_Windows_cannot_manage_ = 5,

            The_boot_configuration_for_this_device_conflicts_with_other_devices_ = 6,

            Cannot_filter_ = 7,

            The_driver_loader_for_the_device_is_missing_ = 8,

            This_device_is_not_working_properly_because_the_controlling_firmware_is_reporting_the_resources_for_the_device_incorrectly_ = 9,

            This_device_cannot_start_ = 10,

            This_device_failed_ = 11,

            This_device_cannot_find_enough_free_resources_that_it_can_use_ = 12,

            Windows_cannot_verify_this_device_s_resources_ = 13,

            This_device_cannot_work_properly_until_you_restart_your_computer_ = 14,

            This_device_is_not_working_properly_because_there_is_probably_a_re_enumeration_problem_ = 15,

            Windows_cannot_identify_all_the_resources_this_device_uses_ = 16,

            This_device_is_asking_for_an_unknown_resource_type_ = 17,

            Reinstall_the_drivers_for_this_device_ = 18,

            Failure_using_the_VxD_loader_ = 19,

            Your_registry_might_be_corrupted_ = 20,

            System_failure_Try_changing_the_driver_for_this_device_If_that_does_not_work_see_your_hardware_documentation_Windows_is_removing_this_device_ = 21,

            This_device_is_disabled_ = 22,

            System_failure_Try_changing_the_driver_for_this_device_If_that_doesn_t_work_see_your_hardware_documentation_ = 23,

            This_device_is_not_present_is_not_working_properly_or_does_not_have_all_its_drivers_installed_ = 24,

            Windows_is_still_setting_up_this_device_ = 25,

            Windows_is_still_setting_up_this_device_0 = 26,

            This_device_does_not_have_valid_log_configuration_ = 27,

            The_drivers_for_this_device_are_not_installed_ = 28,

            This_device_is_disabled_because_the_firmware_of_the_device_did_not_give_it_the_required_resources_ = 29,

            This_device_is_using_an_Interrupt_Request_IRQ_resource_that_another_device_is_using_ = 30,

            This_device_is_not_working_properly_because_Windows_cannot_load_the_drivers_required_for_this_device_ = 31,

            NULL_ENUM_VALUE = 32,
        }

        // Enumerator implementation for enumerating instances of the class.
        public class NetworkAdapterCollection : object, ICollection
        {
            private readonly ManagementObjectCollection privColObj;

            public NetworkAdapterCollection(ManagementObjectCollection objCollection)
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

            public virtual void CopyTo(Array array, int index)
            {
                privColObj.CopyTo(array, index);
                int nCtr;
                for (nCtr = 0; (nCtr < array.Length); nCtr = (nCtr + 1))
                {
                    array.SetValue(new NetworkAdapter(((ManagementObject)(array.GetValue(nCtr)))), nCtr);
                }
            }

            public virtual IEnumerator GetEnumerator()
            {
                return new NetworkAdapterEnumerator(privColObj.GetEnumerator());
            }

            public class NetworkAdapterEnumerator : object, IEnumerator
            {

                private readonly ManagementObjectCollection.ManagementObjectEnumerator privObjEnum;

                public NetworkAdapterEnumerator(ManagementObjectCollection.ManagementObjectEnumerator objEnum)
                {
                    privObjEnum = objEnum;
                }

                public virtual object Current
                {
                    get
                    {
                        return new NetworkAdapter(((ManagementObject)(privObjEnum.Current)));
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

    public class WMIValueTypeConverter : TypeConverter
    {
        private readonly TypeConverter baseConverter;

        private readonly System.Type baseType;

        public WMIValueTypeConverter(System.Type inBaseType)
        {
            baseConverter = TypeDescriptor.GetConverter(inBaseType);
            baseType = inBaseType;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
        {
            return baseConverter.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return baseConverter.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return baseConverter.ConvertFrom(context, culture, value);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            return baseConverter.CreateInstance(context, propertyValues);
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return baseConverter.GetCreateInstanceSupported(context);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return baseConverter.GetProperties(context, value, attributes);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return baseConverter.GetStandardValuesExclusive(context);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return baseConverter.GetStandardValuesSupported(context);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ((baseType.BaseType == typeof(System.Enum)))
            {
                if ((value.GetType() == destinationType))
                {
                    return value;
                }
                if ((((value == null)
                            && (context != null))
                            && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false)))
                {
                    return "NULL_ENUM_VALUE";
                }
                return baseConverter.ConvertTo(context, culture, value, destinationType);
            }
            if (((baseType == typeof(bool))
                        && (baseType.BaseType == typeof(System.ValueType))))
            {
                if ((((value == null)
                            && (context != null))
                            && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false)))
                {
                    return "";
                }
                return baseConverter.ConvertTo(context, culture, value, destinationType);
            }
            if (((context != null)
                        && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false)))
            {
                return "";
            }
            return baseConverter.ConvertTo(context, culture, value, destinationType);
        }
    }

    // Embedded class to represent WMI system Properties.
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ManagementSystemProperties
    {
        private readonly ManagementBaseObject PrivateLateBoundObject;

        public ManagementSystemProperties(ManagementBaseObject ManagedObject)
        {
            PrivateLateBoundObject = ManagedObject;
        }

        [Browsable(true)]
        public int GENUS
        {
            get
            {
                return ((int)(PrivateLateBoundObject["__GENUS"]));
            }
        }

        [Browsable(true)]
        public string CLASS
        {
            get
            {
                return ((string)(PrivateLateBoundObject["__CLASS"]));
            }
        }

        [Browsable(true)]
        public string SUPERCLASS
        {
            get
            {
                return ((string)(PrivateLateBoundObject["__SUPERCLASS"]));
            }
        }

        [Browsable(true)]
        public string DYNASTY
        {
            get
            {
                return ((string)(PrivateLateBoundObject["__DYNASTY"]));
            }
        }

        [Browsable(true)]
        public string RELPATH
        {
            get
            {
                return ((string)(PrivateLateBoundObject["__RELPATH"]));
            }
        }

        [Browsable(true)]
        public int PROPERTY_COUNT
        {
            get
            {
                return ((int)(PrivateLateBoundObject["__PROPERTY_COUNT"]));
            }
        }

        [Browsable(true)]
        public string[] DERIVATION
        {
            get
            {
                return ((string[])(PrivateLateBoundObject["__DERIVATION"]));
            }
        }

        [Browsable(true)]
        public string SERVER
        {
            get
            {
                return ((string)(PrivateLateBoundObject["__SERVER"]));
            }
        }

        [Browsable(true)]
        public string NAMESPACE
        {
            get
            {
                return ((string)(PrivateLateBoundObject["__NAMESPACE"]));
            }
        }

        [Browsable(true)]
        public string PATH
        {
            get
            {
                return ((string)(PrivateLateBoundObject["__PATH"]));
            }
        }
    }
}
