using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TryggSync.Server;
using TryggSync.Client;
using TryggSync.Workspace;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;

namespace TryggSync
{
    /// <summary>
    /// The PluginDefinition is the ‘entry’ point to any plugin.  
    /// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.  
    /// Several PluginDefinitions are allowed to be available within one DLL.
    /// Here the references to all other plugin known objects and classes are defined.
    /// The class is an abstract class where all implemented methods and properties need to be declared with override.
    /// The class is constructed when the environment is loading the DLL.
    /// </summary>
    public class TryggSyncDefinition : PluginDefinition
    {
        private static System.Drawing.Image _treeNodeImage;
        private static System.Drawing.Image _topTreeNodeImage;

        internal static Guid TryggSyncPluginId = new Guid("c0db9ee7-e577-4f3e-95d7-2456c9c59954");
        internal static Guid TryggSyncKind = new Guid("cb10008e-ebab-402d-827e-fe46cd7cbc79");
        internal static Guid TryggSyncSidePanel = new Guid("cd8d4f70-69dd-49d6-9d53-cebb32e362c8");
        internal static Guid TryggSyncViewItemPlugin = new Guid("81916cce-4765-4371-9347-6b54f85b9890");
        internal static Guid TryggSyncSettingsPanel = new Guid("8fed7a5f-a81e-41a0-a0f3-2b727b1ba916");
        internal static Guid TryggSyncBackgroundPlugin = new Guid("04b5caac-e9d7-42a4-afae-feca1c8d41ce");
        internal static Guid TryggSyncWorkSpacePluginId = new Guid("b26398ee-f797-4800-b292-206232986d74");
        internal static Guid TryggSyncWorkSpaceViewItemPluginId = new Guid("7e6e7a96-06b5-43ab-a336-2190f9a9620d");

        internal const string TryggSyncMsgCommandRequest = "TryggSync.Msg.Command.Request";
        internal const string TryggSyncMsgCommandResponse = "TryggSync.Msg.Command.Response";

        #region Private fields

        //
        // Note that all the plugin are constructed during application start, and the constructors
        // should only contain code that references their own dll, e.g. resource load.

        private List<BackgroundPlugin> _backgroundPlugins = new List<BackgroundPlugin>();
        private Collection<SettingsPanelPlugin> _settingsPanelPlugins = new Collection<SettingsPanelPlugin>();
        private List<ViewItemPlugin> _viewItemPlugins = new List<ViewItemPlugin>();
        //private List<SidePanelPlugin> _sidePanelPlugins = new List<SidePanelPlugin>();
        private List<WorkSpacePlugin> _workSpacePlugins = new List<WorkSpacePlugin>();
        private List<ViewItemToolbarPlugin> _viewItemToolbarPlugins = new List<ViewItemToolbarPlugin>();
        private List<WorkSpaceToolbarPlugin> _workSpaceToolbarPlugins = new List<WorkSpaceToolbarPlugin>();

        #endregion

        #region Initialization

        /// <summary>
        /// Load resources 
        /// </summary>
        static TryggSyncDefinition()
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
                _workSpacePlugins.Add(new TryggSyncWorkSpacePlugin());
                //_sidePanelPlugins.Add(new TryggSyncSidePanelPlugin());
                //_viewItemPlugins.Add(new TryggSyncViewItemPlugin());
                _viewItemPlugins.Add(new TryggSyncWorkSpaceViewItemPlugin());
                _settingsPanelPlugins.Add(new TryggSyncSettingsPanelPlugin());
            }

            if(EnvironmentManager.Instance.EnvironmentType == EnvironmentType.Service)
                _backgroundPlugins.Add(new TryggSyncBackgroundPlugin());

        }

        /// <summary>
        /// The main application is about to be in an undetermined state, either logging off or exiting.
        /// You can release resources at this point, it should match what you acquired during Init, so additional call to Init() will work.
        /// </summary>
        public override void Close()
        {
            //_sidePanelPlugins.Clear();
            _viewItemPlugins.Clear();
            _settingsPanelPlugins.Clear();
            _backgroundPlugins.Clear();
            _workSpacePlugins.Clear();
            _workSpaceToolbarPlugins.Clear();
        }

        public override Guid Id
        {
            get
            {
                return TryggSyncPluginId;
            }
        }

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
            get { return "TryggSync"; }
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
        public override string VersionString => "0.1.0.0";

        public override System.Drawing.Image Icon => _topTreeNodeImage;

        #region Client related methods and properties

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
        //public override Collection<SettingsPanelPlugin> SettingsPanelPlugins
        //{
        //    get { return _settingsPanelPlugins; }
        //}

        //public override List<SidePanelPlugin> SidePanelPlugins
        //{
        //    get { return _sidePanelPlugins; }
        //}

        /// <summary>
        /// Return the workspace plugins
        /// </summary>
        public override List<WorkSpacePlugin> WorkSpacePlugins
        {
            get { return _workSpacePlugins; }
        }


        #endregion


        /// <summary>
        /// Create and returns the background task.
        /// </summary>
        public override List<BackgroundPlugin> BackgroundPlugins
        {
            get { return _backgroundPlugins; }
        }

    }
}
