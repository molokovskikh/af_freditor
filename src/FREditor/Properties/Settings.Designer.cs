﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3074
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FREditor.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Gainsboro")]
        public global::System.Drawing.Color BaseCostColor {
            get {
                return ((global::System.Drawing.Color)(this["BaseCostColor"]));
            }
            set {
                this["BaseCostColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("LightGreen")]
        public global::System.Drawing.Color NewCostColor {
            get {
                return ((global::System.Drawing.Color)(this["NewCostColor"]));
            }
            set {
                this["NewCostColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("LightBlue")]
        public global::System.Drawing.Color ChangedCostColor {
            get {
                return ((global::System.Drawing.Color)(this["ChangedCostColor"]));
            }
            set {
                this["ChangedCostColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("IndianRed")]
        public global::System.Drawing.Color DeletedCostColor {
            get {
                return ((global::System.Drawing.Color)(this["DeletedCostColor"]));
            }
            set {
                this["DeletedCostColor"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("tcp://fms.adc.analit.net:889/RemotePriceProcessor")]
        public string PriceProcessorURL {
            get {
                return ((string)(this["PriceProcessorURL"]));
            }
        }
    }
}
