using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TryggSupport.Background;
using TryggSupport.Client;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;

namespace TryggSupport
{
    /// <summary>
    /// The PluginDefinition is the ‘entry’ point to any plugin.  
    /// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.  
    /// Several PluginDefinitions are allowed to be available within one DLL.
    /// Here the references to all other plugin known objects and classes are defined.
    /// The class is an abstract class where all implemented methods and properties need to be declared with override.
    /// The class is constructed when the environment is loading the DLL.
    /// </summary>
    public class TryggSupportDefinition : PluginDefinition
    {
        private static System.Drawing.Image _treeNodeImage;
        private static System.Drawing.Image _topTreeNodeImage;

        internal static Guid TryggSupportPluginId = new Guid("dccbac52-af22-4613-84ac-b08d194696e1");
        internal static Guid TryggSupportKind = new Guid("17270d02-847a-48e5-bff7-67dd2af7fe5b");
        internal static Guid TryggSupportSidePanel = new Guid("798c9207-1f30-4d7c-80ce-b87e9c611ec0");
        internal static Guid TryggSupportViewItemPlugin = new Guid("4feb4f4d-5051-4ec4-b73a-e7db534e972f");
        internal static Guid TryggSupportSettingsPanel = new Guid("d387e693-3507-4233-87cb-ad87a3aafbf7");
        internal static Guid TryggSupportBackgroundPlugin = new Guid("cc8605e5-1496-4299-a377-a763f9a9314e");
        internal static Guid TryggSupportWorkSpacePluginId = new Guid("a91ce7e0-6456-4054-974a-2ba7048d0b28");
        internal static Guid TryggSupportWorkSpaceViewItemPluginId = new Guid("e12fa959-b274-4a57-8dfc-408756315c18");
        internal static Guid TryggSupportTabPluginId = new Guid("8e17ae6f-b65d-4329-bfca-38139e262cc7");
        internal static Guid TryggSupportViewLayoutId = new Guid("4ad63fd3-baf6-467a-a670-390cd4c0b875");
        // IMPORTANT! Due to shortcoming in Visual Studio template the below cannot be automatically replaced with proper unique GUIDs, so you will have to do it yourself
        internal static Guid TryggSupportWorkSpaceToolbarPluginId = new Guid("22222222-2222-2222-2222-222222222222");
        internal static Guid TryggSupportViewItemToolbarPluginId = new Guid("33333333-3333-3333-3333-333333333333");
        internal static Guid TryggSupportToolsOptionDialogPluginId = new Guid("44444444-4444-4444-4444-444444444444");

        #region Private fields

        private UserControl _treeNodeInofUserControl;

        //
        // Note that all the plugin are constructed during application start, and the constructors
        // should only contain code that references their own dll, e.g. resource load.

        private List<BackgroundPlugin> _backgroundPlugins = new List<BackgroundPlugin>();
        private Collection<SettingsPanelPlugin> _settingsPanelPlugins = new Collection<SettingsPanelPlugin>();
        private List<ViewItemPlugin> _viewItemPlugins = new List<ViewItemPlugin>();
        private List<WorkSpacePlugin> _workSpacePlugins = new List<WorkSpacePlugin>();

        #endregion

        #region Initialization

        /// <summary>
        /// Load resources 
        /// </summary>
        static TryggSupportDefinition()
        {
            _treeNodeImage = Properties.Resources.DummyItem;
            _topTreeNodeImage = Properties.Resources.Server;
        }


        /// <summary>
        /// Get the icon for the plugin
        /// </summary>
        internal static Image TreeNodeImage
        {
            get { return _treeNodeImage; }
        }

        #endregion

        /// <summary>
        /// This method is called when the environment is up and running.
        /// Registration of Messages via RegisterReceiver can be done at this point.
        /// </summary>
        public override void Init()
        {
            if (EnvironmentManager.Instance.EnvironmentType == EnvironmentType.SmartClient)
            {
                _workSpacePlugins.Add(new TryggSupportWorkSpacePlugin());
                _viewItemPlugins.Add(new TryggSupportWorkSpaceViewItemPlugin());
                _backgroundPlugins.Add(new TryggSupportBackgroundPlugin());
                //_settingsPanelPlugins.Add(new TryggSupportSettingsPanelPlugin());
            }

        }

        /// <summary>
        /// The main application is about to be in an undetermined state, either logging off or exiting.
        /// You can release resources at this point, it should match what you acquired during Init, so additional call to Init() will work.
        /// </summary>
        public override void Close()
        {
            _viewItemPlugins.Clear();
            _settingsPanelPlugins.Clear();
            _backgroundPlugins.Clear();
            _workSpacePlugins.Clear();
        }

        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get
            {
                return TryggSupportPluginId;
            }
        }

        /// <summary>
        /// This Guid can be defined on several different IPluginDefinitions with the same value,
        /// and will result in a combination of this top level ProductNode for several plugins.
        /// Set to Guid.Empty if no sharing is enabled.
        /// </summary>
        public override Guid SharedNodeId
        {
            get
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Define name of top level Tree node - e.g. A product name
        /// </summary>
        public override string Name
        {
            get { return "TryggSupport"; }
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
                return "0.1.0.0";
            }
        }

        /// <summary>
        /// Icon to be used on top level - e.g. a product or company logo
        /// </summary>
        public override System.Drawing.Image Icon
        {
            get { return _topTreeNodeImage; }
        }



        /// <summary>
        /// A list of Client side definitions for Smart Client
        /// </summary>
        public override List<ViewItemPlugin> ViewItemPlugins
        {
            get { return _viewItemPlugins; }
        }

        /// <summary>
        /// An extension plug-in running in the Smart Client to add more choices on the Settings panel.
        /// Supported from Smart Client 2017 R1. For older versions use OptionsDialogPlugins instead.
        /// </summary>
        public override Collection<SettingsPanelPlugin> SettingsPanelPlugins
        {
            get { return _settingsPanelPlugins; }
        }

        public override List<WorkSpacePlugin> WorkSpacePlugins
        {
            get { return _workSpacePlugins; }
        }


        /// <summary>
        /// Create and returns the background task.
        /// </summary>
        public override List<BackgroundPlugin> BackgroundPlugins
        {
            get { return _backgroundPlugins; }
        }

    }
}
