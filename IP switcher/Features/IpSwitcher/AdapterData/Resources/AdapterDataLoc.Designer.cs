﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TTech.IP_Switcher.Features.IpSwitcher.AdapterData.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class AdapterDataLoc {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AdapterDataLoc() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TTech.IP_Switcher.Features.IpSwitcher.AdapterData.Resources.AdapterDataLoc", typeof(AdapterDataLoc).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to activate adapter.
        /// </summary>
        public static string ActivationFailed {
            get {
                return ResourceManager.GetString("ActivationFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to apply settings.
        /// </summary>
        public static string ApplyFailed {
            get {
                return ResourceManager.GetString("ApplyFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to deactivate adapter.
        /// </summary>
        public static string DeactivationFailed {
            get {
                return ResourceManager.GetString("DeactivationFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to ser DHCP.
        /// </summary>
        public static string EnableDHCPFailed {
            get {
                return ResourceManager.GetString("EnableDHCPFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to set IP adresses.
        /// </summary>
        public static string EnableStaticFailed {
            get {
                return ResourceManager.GetString("EnableStaticFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Errormessage: {0} - {1}.
        /// </summary>
        public static string ErrorMessage {
            get {
                return ResourceManager.GetString("ErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New preset.
        /// </summary>
        public static string NewLocationDescription {
            get {
                return ResourceManager.GetString("NewLocationDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to renew DHCP lease.
        /// </summary>
        public static string RenewDHCPLeaseFailed {
            get {
                return ResourceManager.GetString("RenewDHCPLeaseFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to set DNS servers.
        /// </summary>
        public static string SetDnsServersFailed {
            get {
                return ResourceManager.GetString("SetDnsServersFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to set gateways.
        /// </summary>
        public static string SetGatewaysFailed {
            get {
                return ResourceManager.GetString("SetGatewaysFailed", resourceCulture);
            }
        }
    }
}
