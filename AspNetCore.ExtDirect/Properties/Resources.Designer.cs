﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AspNetCore.ExtDirect.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AspNetCore.ExtDirect.Properties.Resources", typeof(Resources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to locate Polling events source provider &quot;{0}&quot;. Consider to check the Ext Direct services configuration..
        /// </summary>
        internal static string ERR_CANNOT_FIND_POLLING_HANDLER {
            get {
                return ResourceManager.GetString("ERR_CANNOT_FIND_POLLING_HANDLER", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ExtDirect action with name &quot;{0}&quot; has already been registered. Action names should be unique..
        /// </summary>
        internal static string ERR_DUPLICATE_ACTION {
            get {
                return ResourceManager.GetString("ERR_DUPLICATE_ACTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Name of Polling API provider being registered must be unique (&quot;{0}&quot;).
        /// </summary>
        internal static string ERR_DUPLICATE_POLLING_API_NAME {
            get {
                return ResourceManager.GetString("ERR_DUPLICATE_POLLING_API_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Name of Remoting API provider being registered must be unique (&quot;{0}&quot;).
        /// </summary>
        internal static string ERR_DUPLICATE_REMOTING_API_NAME {
            get {
                return ResourceManager.GetString("ERR_DUPLICATE_REMOTING_API_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid argument. Polling provider name is empty. Consider to check the Ext Direct services configuration..
        /// </summary>
        internal static string ERR_EMPTY_POLLING_PROVIDER_NAME {
            get {
                return ResourceManager.GetString("ERR_EMPTY_POLLING_PROVIDER_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid type of input Ext Direct action method arguments (action: {0}, method: {1}, type of method arguments: {2}). Arguments should be of type javascript Array, javascript Object, or scalar value (string, number, bool, or Date).
        /// </summary>
        internal static string ERR_INVALID_ARG_TYPE {
            get {
                return ResourceManager.GetString("ERR_INVALID_ARG_TYPE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid input JSON data. Expecting instance of Object or Array according Ext Direct Specification..
        /// </summary>
        internal static string ERR_INVALID_EXT_REQUEST {
            get {
                return ResourceManager.GetString("ERR_INVALID_EXT_REQUEST", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid JSON input data..
        /// </summary>
        internal static string ERR_INVALID_JSON {
            get {
                return ResourceManager.GetString("ERR_INVALID_JSON", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Provider name &quot;{0}&quot; must be a valid JavaScript identifier..
        /// </summary>
        internal static string ERR_INVALID_PROVIDER_NAME {
            get {
                return ResourceManager.GetString("ERR_INVALID_PROVIDER_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to AspNetCore.ExtDirect configuration error. Some configuration methods (IServiceProvider.AddExtDirect and other) should be called to configure the library..
        /// </summary>
        internal static string ERR_NOT_CONFIGURED {
            get {
                return ResourceManager.GetString("ERR_NOT_CONFIGURED", resourceCulture);
            }
        }
    }
}
