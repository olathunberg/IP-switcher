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

        private readonly ManagementObject PrivateLateBoundObject;

        // Member variable to store the 'automatic commit' behavior for the class.
        private bool AutoCommitProp;

        // The current WMI object used
        private readonly ManagementBaseObject curObj;

        // Flag to indicate if the instance is an embedded object.
        private bool isEmbedded;

        public NetworkAdapter(ManagementObject theObject)
        {
            Initialize();
            if (CheckIfProperClass(theObject))
            {
                PrivateLateBoundObject = theObject;
                curObj = PrivateLateBoundObject;
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
                if (curObj?.ClassPath != null)
                {
                    strRet = (string)curObj["__CLASS"];
                    if (string.IsNullOrEmpty(strRet))
                    {
                        strRet = CreatedClassName;
                    }
                }

                return strRet;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsConfigManagerErrorCodeNull => curObj[nameof(ConfigManagerErrorCode)] == null;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [TypeConverter(typeof(WmiValueTypeConverter))]
        public ConfigManagerErrorCodeValues ConfigManagerErrorCode
        {
            get
            {
                if (curObj[nameof(ConfigManagerErrorCode)] == null)
                {
                    return (ConfigManagerErrorCodeValues)Convert.ToInt32(32);
                }
                return (ConfigManagerErrorCodeValues)Convert.ToInt32(curObj[nameof(ConfigManagerErrorCode)]);
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The Description property provides a textual description of the object. ")]
        public string Description => (string)curObj[nameof(Description)];

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The GUID property specifies the Globally-unique identifier for the connection.")]
        public string GUID => (string)curObj[nameof(GUID)];

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The MACAddress property indicates the media access control address for this network adapter. A MAC address is a unique 48-bit number assigned to the network adapter by the manufacturer. It uniquely identifies this network adapter and is used for mapping TCP/IP network communications.")]
        public string MACAddress => (string)curObj[nameof(MACAddress)];
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The InterfaceIndex property contains the index value that uniquely identifies the" +
            " local interface.")]
        [TypeConverter(typeof(WmiValueTypeConverter))]
        public uint InterfaceIndex
        {
            get
            {
                if (curObj[nameof(InterfaceIndex)] == null)
                {
                    return Convert.ToUInt32(0);
                }
                return (uint)curObj[nameof(InterfaceIndex)];
            }
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMaxSpeedNull => curObj[nameof(MaxSpeed)] == null;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The maximum speed, in bits per second, for the network adapter.")]
        [TypeConverter(typeof(WmiValueTypeConverter))]
        public ulong MaxSpeed
        {
            get
            {
                if (curObj[nameof(MaxSpeed)] == null)
                {
                    return Convert.ToUInt64(0);
                }
                return (ulong)curObj[nameof(MaxSpeed)];
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The Name property defines the label by which the object is known. When subclassed" +
            ", the Name property can be overridden to be a Key property.")]
        public string Name => (string)curObj[nameof(Name)];
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The NetConnectionID property specifies the name of the network connection as it a" +
            "ppears in the \'Network Connections\' folder.")]
        public string NetConnectionID
        {
            get => curObj[nameof(NetConnectionID)] as string;
            set
            {
                curObj[nameof(NetConnectionID)] = value;
                if (!isEmbedded && AutoCommitProp)
                {
                    PrivateLateBoundObject.Put();
                }
            }
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsNetConnectionStatusNull => curObj[nameof(NetConnectionStatus)] == null;

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
        [TypeConverter(typeof(WmiValueTypeConverter))]
        public ushort NetConnectionStatus
        {
            get
            {
                if (curObj[nameof(NetConnectionStatus)] == null)
                {
                    return Convert.ToUInt16(0);
                }
                return (ushort)curObj[nameof(NetConnectionStatus)];
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsNetEnabledNull
        {
            get
            {
                return curObj[nameof(NetEnabled)] == null;
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The NetEnabled property specifies whether the network connection is enabled or no" +
            "t.")]
        [TypeConverter(typeof(WmiValueTypeConverter))]
        public bool NetEnabled
        {
            get
            {
                if (curObj[nameof(NetEnabled)] == null)
                {
                    return Convert.ToBoolean(0);
                }
                return (bool)curObj[nameof(NetEnabled)];
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("An array of strings indicating the network addresses for an adapter.")]
        public string[] NetworkAddresses => (string[])curObj[nameof(NetworkAddresses)];

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPhysicalAdapterNull => curObj[nameof(PhysicalAdapter)] == null;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The PhysicalAdapter property specifies whether the adapter is physical adapter or" +
            " logical adapter.")]
        [TypeConverter(typeof(WmiValueTypeConverter))]
        public bool PhysicalAdapter
        {
            get
            {
                if (curObj[nameof(PhysicalAdapter)] == null)
                {
                    return Convert.ToBoolean(0);
                }
                return (bool)curObj[nameof(PhysicalAdapter)];
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The ProductName property indicates the product name of the network adapter.\nExamp" +
            "le: Fast EtherLink XL")]
        public string ProductName => (string)curObj[nameof(ProductName)];

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSpeedNull => curObj[nameof(Speed)] == null;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("An estimate of the current bandwidth in bits per second. For endpoints which vary" +
            " in bandwidth or for those where no accurate estimation can be made, this proper" +
            "ty should contain the nominal bandwidth.")]
        [TypeConverter(typeof(WmiValueTypeConverter))]
        public ulong Speed
        {
            get
            {
                if (curObj[nameof(Speed)] == null)
                {
                    return Convert.ToUInt64(0);
                }
                return (ulong)curObj[nameof(Speed)];
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The Status property is a string indicating the current status of the object. Various operational and non-operational statuses can be defined. Operational statuses are ""OK"", ""Degraded"" and ""Pred Fail"". ""Pred Fail"" indicates that an element may be functioning properly but predicting a failure in the near future. An example is a SMART-enabled hard drive. Non-operational statuses can also be specified. These are ""Error"", ""Starting"", ""Stopping"" and ""Service"". The latter, ""Service"", could apply during mirror-resilvering of a disk, reload of a user permissions list, or other administrative work. Not all such work is on-line, yet the managed element is neither ""OK"" nor in one of the other states.")]
        public string Status => (string)curObj[nameof(Status)];

        private bool CheckIfProperClass(ManagementBaseObject theObj)
        {
            if ((theObj != null)
                        && (string.Compare((string)theObj["__CLASS"], this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0))
            {
                return true;
            }

            return false;
        }

        // Converts a given datetime in DMTF format to System.DateTime object.

        [Browsable(true)]
        public void CommitObject()
        {
            if (!isEmbedded)
            {
                PrivateLateBoundObject.Put();
            }
        }

        private void Initialize()
        {
            AutoCommitProp = true;
            isEmbedded = false;
        }

        // Different overloads of GetInstances() help in enumerating instances of the WMI class.
        public static NetworkAdapterCollection GetInstances() => GetInstances(null, null, null);

        public static NetworkAdapterCollection GetInstances(ManagementScope mgmtScope, string condition, string[] selectedProperties)
        {
            var ObjectSearcher = new ManagementObjectSearcher(mgmtScope, new SelectQuery("Win32_NetworkAdapter", condition, selectedProperties));
            var enumOptions = new EnumerationOptions();
            enumOptions.EnsureLocatable = true;
            ObjectSearcher.Options = enumOptions;
            return new NetworkAdapterCollection(ObjectSearcher.Get());
        }

        [Browsable(true)]
        public static NetworkAdapter CreateInstance()
        {
            ManagementScope mgmtScope = new();
            mgmtScope.Path.NamespacePath = CreatedWmiNamespace;

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
            if (!isEmbedded)
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
            if (!isEmbedded)
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
            if (!isEmbedded)
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
        public class NetworkAdapterCollection : ICollection
        {
            private readonly ManagementObjectCollection privColObj;

            public NetworkAdapterCollection(ManagementObjectCollection objCollection)
            {
                privColObj = objCollection;
            }

            public virtual int Count => privColObj.Count;

            public virtual bool IsSynchronized => privColObj.IsSynchronized;

            public virtual object SyncRoot => this;

            public virtual void CopyTo(Array array, int index)
            {
                privColObj.CopyTo(array, index);
                int nCtr;
                for (nCtr = 0; nCtr < array.Length; nCtr = nCtr + 1)
                {
                    array.SetValue(new NetworkAdapter((ManagementObject)array.GetValue(nCtr)), nCtr);
                }
            }

            public virtual IEnumerator GetEnumerator()
            {
                return new NetworkAdapterEnumerator(privColObj.GetEnumerator());
            }

            public class NetworkAdapterEnumerator : IEnumerator
            {

                private readonly ManagementObjectCollection.ManagementObjectEnumerator privObjEnum;

                public NetworkAdapterEnumerator(ManagementObjectCollection.ManagementObjectEnumerator objEnum)
                {
                    privObjEnum = objEnum;
                }

                public virtual object Current => new NetworkAdapter((ManagementObject)privObjEnum.Current);

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

    public class WmiValueTypeConverter : TypeConverter
    {
        private readonly TypeConverter baseConverter;

        private readonly System.Type baseType;

        public WmiValueTypeConverter(System.Type inBaseType)
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
            if (baseType.BaseType == typeof(System.Enum))
            {
                if (value.GetType() == destinationType)
                {
                    return value;
                }

                return context != null && !context.PropertyDescriptor.ShouldSerializeValue(context.Instance)
                    ? "NULL_ENUM_VALUE"
                    : baseConverter.ConvertTo(context, culture, value, destinationType);
            }
            if (baseType == typeof(bool) && baseType.BaseType == typeof(System.ValueType))
            {
                if ((value == null)
                            && (context != null)
                            && (!context.PropertyDescriptor.ShouldSerializeValue(context.Instance)))
                {
                    return "";
                }
                return baseConverter.ConvertTo(context, culture, value, destinationType);
            }
            if ((context != null)
                        && (!context.PropertyDescriptor.ShouldSerializeValue(context.Instance)))
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
        public int GENUS => (int)PrivateLateBoundObject["__GENUS"];

        [Browsable(true)]
        public string CLASS => (string)PrivateLateBoundObject["__CLASS"];

        [Browsable(true)]
        public string SUPERCLASS => (string)PrivateLateBoundObject["__SUPERCLASS"];

        [Browsable(true)]
        public string DYNASTY => (string)PrivateLateBoundObject["__DYNASTY"];

        [Browsable(true)]
        public string RELPATH => (string)PrivateLateBoundObject["__RELPATH"];

        [Browsable(true)]
        public int PROPERTY_COUNT => (int)PrivateLateBoundObject["__PROPERTY_COUNT"];

        [Browsable(true)]
        public string[] DERIVATION => (string[])PrivateLateBoundObject["__DERIVATION"];

        [Browsable(true)]
        public string SERVER => (string)PrivateLateBoundObject["__SERVER"];

        [Browsable(true)]
        public string NAMESPACE => (string)PrivateLateBoundObject["__NAMESPACE"];

        [Browsable(true)]
        public string PATH => (string)PrivateLateBoundObject["__PATH"];
    }
}
