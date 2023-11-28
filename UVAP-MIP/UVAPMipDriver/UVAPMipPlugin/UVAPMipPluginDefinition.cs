using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using UVAPMipPlugin.Background;
using UVAPMipPlugin.Settings;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;

namespace UVAPMipPlugin
{
    /// <summary>
    /// The PluginDefinition is the ‘entry’ point to any plugin.  
    /// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.  
    /// Several PluginDefinitions are allowed to be available within one DLL.
    /// Here the references to all other plugin known objects and classes are defined.
    /// The class is an abstract class where all implemented methods and properties need to be declared with override.
    /// The class is constructed when the environment is loading the DLL.
    /// </summary>
    public class UVAPMipPluginDefinition : PluginDefinition
    {
        internal static Guid UVAPMipPluginPluginId = new Guid("aad7c9f6-1374-4135-b41a-7be0b9e3845a");
        internal static Guid UVAPMipPluginKind = new Guid("e1df40a3-df6c-4eec-80c1-bcf18d07ad70");
        internal static Guid UVAPOverlayBackgroundPlugin = new Guid("{C1619658-EF44-48EF-AF48-5D94164C4212}");

        private List<BackgroundPlugin> _backgroundPlugins = new List<BackgroundPlugin>();
        private Collection<SettingsPanelPlugin> _settingsPlugins = new Collection<SettingsPanelPlugin>();


        static UVAPMipPluginDefinition()
        {
        }


        /// <summary>
        /// Get the icon for the plugin, required but not used
        /// </summary>
        internal static Image TreeNodeImage
        {
            get { return Properties.Resources.Server; }
        }


        /// <summary>
        /// This method is called when the environment is up and running.
        /// Registration of Messages via RegisterReceiver can be done at this point.
        /// </summary>
        public override void Init()
        {
            if (EnvironmentManager.Instance.EnvironmentType == EnvironmentType.SmartClient)
            {
                _backgroundPlugins.Add(new VisualizerBackgroundPlugin());
                _settingsPlugins.Add(new OverlaySettingsPlugin());
            }
        }

        /// <summary>
        /// The main application is about to be in an undetermined state, either logging off or exiting.
        /// You can release resources at this point, it should match what you acquired during Init, so additional call to Init() will work.
        /// </summary>
        public override void Close()
        {
            _backgroundPlugins.Clear();
            _settingsPlugins.Clear();
        }


        #region Identification Properties

        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get
            {
                return UVAPMipPluginPluginId;
            }
        }

        /// <summary>
        /// Define name of top level Tree node - e.g. A product name
        /// </summary>
        public override string Name
        {
            get { return "UVAP MIP Plugin"; }
        }

        /// <summary>
        /// Your company name
        /// </summary>
        public override string Manufacturer
        {
            get
            {
                return "InSupport Nätverksvideo AB";
            }
        }

        /// <summary>
        /// Version of this plugin.
        /// </summary>
        public override string VersionString
        {
            get
            {
                return "1.0.0.0";
            }
        }

        /// <summary>
        /// Icon to be used on top level - e.g. a product or company logo
        /// </summary>
        public override System.Drawing.Image Icon
        {
            get { return Properties.Resources.Server; }
        }

        #endregion

        public override List<BackgroundPlugin> BackgroundPlugins // returns the background plugins
        {
            get { return _backgroundPlugins; }
        }

        public override Collection<SettingsPanelPlugin> SettingsPanelPlugins => _settingsPlugins; // return the settings plugins, viewed in the Smart Client settings panel

    }
}
