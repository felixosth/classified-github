using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TryggRetail.Admin;
using TryggRetail.Background;
using TryggRetail.Client;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;
using TryggRetail.PopupWindow.Acknowledge;
using TryggRetail.Playback;
using TryggRetail.PopupWindow.Live;

namespace TryggRetail
{
    /// <summary>
    /// The PluginDefinition is the ‘entry’ point to any plugin.  
    /// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.  
    /// Several PluginDefinitions are allowed to be available within one DLL.
    /// Here the references to all other plugin known objects and classes are defined.
    /// The class is an abstract class where all implemented methods and properties need to be declared with override.
    /// The class is constructed when the environment is loading the DLL.
    /// </summary>
    public class TryggRetailDefinition : PluginDefinition
    {
        private static System.Drawing.Image _treeNodeImage;
        private static System.Drawing.Image _topTreeNodeImage;

        internal static Guid TryggRetailPluginId = new Guid("d0677ba9-3bdc-451a-9088-ea4e7154af70");
        internal static Guid TryggRetailKind = new Guid("6d40025c-adad-4d82-aecc-bd591887252f");
        //internal static Guid TryggRetailSidePanel = new Guid("b5db453e-42be-412b-8df4-86adeab03c7d");
        internal static Guid TryggRetailViewItemPlugin = new Guid("0b18ab96-5b7b-44e2-bd9d-0a5c556ac394"); 
        internal static Guid TryggRetailSettingsPanel = new Guid("3a089adf-74f7-4ad7-aa1e-18f9c4cc032e");
        internal static Guid TryggRetailBackgroundPlugin = new Guid("9fdec33a-daa3-46a4-82ac-5e346c8bf02c");
        internal static Guid TryggRetailEventServerBackgroundPlugin = new Guid("84E9DE4C-4EF4-41C0-99D8-7ECC2AFCD584");
        //internal static Guid TryggRetailWorkSpacePluginId = new Guid("edc403c3-d24e-432c-aac4-867f3e8c57ee");
        internal static Guid TryggRetailWorkSpaceViewItemPluginId = new Guid("dcc74f64-7ead-4bd4-aed0-1124c1286350");  // playback
        internal static Guid TryggRetailAcknowledgeUserControlID = new Guid("485D2E6C-0289-42F0-8209-132224031BAA");

        internal static string LicenseID = "InSupport-TryggRetail-ConcurrentLicense";
        internal static Guid TryggRetailLicenseItemGuid = new Guid("AE74EAB8-04E0-4076-B121-A77CE4BCE37D");

        internal static Guid TryggRetailLiveViewPlugin = new Guid("E3901F80-1240-420A-A40F-4FAA369EAF15");

        internal const string ApproveClientFilter = "InSupport.TryggRetail.LicenseInformationMsg";
        internal const string ClientApprovalFilter = "InSupport.TryggRetail.UserLogon";
        internal const string NewAlarmFilter = "InSupport.TryggRetail.NewAlarm";
        internal const string CloseAllWindowsFilter = "InSupport.TryggRetail.CloseWindows";


        internal Guid parentGuid = new Guid("2A58C8E7-8A50-4B81-A94C-AD854AF17350");

        //internal static Guid TryggRetailTabPluginId = new Guid("52d94799-8ae3-4a49-b3a9-34b0246bbb0f");
        //internal static Guid TryggRetailViewLayoutId = new Guid("660aeb67-0ce2-4891-a3b2-075552453e67");
        // IMPORTANT! Due to shortcoming in Visual Studio template the below cannot be automatically replaced with proper unique GUIDs, so you will have to do it yourself
        //internal static Guid TryggRetailWorkSpaceToolbarPluginId = new Guid("22222222-2222-2222-2222-222222222222");
        //internal static Guid TryggRetailViewItemToolbarPluginId = new Guid("33333333-3333-3333-3333-333333333333");
        //internal static Guid TryggRetailToolsOptionDialogPluginId = new Guid("44444444-4444-4444-4444-444444444444");

        #region Private fields

        private UserControl _treeNodeInofUserControl;

        //
        // Note that all the plugin are constructed during application start, and the constructors
        // should only contain code that references their own dll, e.g. resource load.

        private List<BackgroundPlugin> _backgroundPlugins = new List<BackgroundPlugin>();
        private Collection<SettingsPanelPlugin> _settingsPanelPlugins = new Collection<SettingsPanelPlugin>();
        private List<ViewItemPlugin> _viewItemPlugins = new List<ViewItemPlugin>();
        private List<ItemNode> _itemNodes = new List<ItemNode>();
        private List<SidePanelPlugin> _sidePanelPlugins = new List<SidePanelPlugin>();
        private List<String> _messageIdStrings = new List<string>();
        private List<SecurityAction> _securityActions = new List<SecurityAction>();
        private List<WorkSpacePlugin> _workSpacePlugins = new List<WorkSpacePlugin>();
        private List<TabPlugin> _tabPlugins = new List<TabPlugin>();
        private List<ViewItemToolbarPlugin> _viewItemToolbarPlugins = new List<ViewItemToolbarPlugin>();
        private List<WorkSpaceToolbarPlugin> _workSpaceToolbarPlugins = new List<WorkSpaceToolbarPlugin>();
        private List<ViewLayout> _viewLayouts = new List<ViewLayout>();
        private List<ToolsOptionsDialogPlugin> _toolsOptionsDialogPlugins = new List<ToolsOptionsDialogPlugin>();

        #endregion

        #region Initialization

        /// <summary>
        /// Load resources 
        /// </summary>
        static TryggRetailDefinition()
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

        EventServerBackgroundPlugin eventServerBackgroundPlugin;

        /// <summary>
        /// This method is called when the environment is up and running.
        /// Registration of Messages via RegisterReceiver can be done at this point.
        /// </summary>
        public override void Init()
        {
            this.AdminPlacementHint = AdminPlacementHint.Root;
            //AdminPlacementHint = AdminPlacementHint.
            // Populate all relevant lists with your plugins etc.
      

            var node = new ItemNode(TryggRetailKind, Guid.Empty,
                                         "TryggRetail", _treeNodeImage,
                                         "TryggRetail Items", _treeNodeImage,
                                         Category.Text, true,
                                         ItemsAllowed.Many,
                                         new TryggRetailItemManager(TryggRetailKind),
                                         null
                                         );
            //node.PlacementHint = PlacementHint.Root;
            _itemNodes.Add(node);
            if (EnvironmentManager.Instance.EnvironmentType == EnvironmentType.SmartClient)
            {

                var back = new TryggRetailBackgroundPlugin();
                _viewItemPlugins.Add(new PlaybackAlarmManager(back));
                _viewItemPlugins.Add(new LiveViewManager(back));
                _viewItemPlugins.Add(new AcknowledgeWpfUserControlManager(null, back));
                //_viewItemPlugins.Add(new TryggRetailWorkSpaceViewItemPlugin());
                //_viewItemToolbarPlugins.Add(new TryggRetailViewItemToolbarPlugin());
                //_workSpaceToolbarPlugins.Add(new TryggRetailWorkSpaceToolbarPlugin());
                //_viewLayouts.Add(new TryggRetailViewLayout());
                _settingsPanelPlugins.Add(new TryggRetailSettingsPanelPlugin(back));
                _backgroundPlugins.Add(back);
            }
            else if(EnvironmentManager.Instance.EnvironmentType == EnvironmentType.Service)
            {
                eventServerBackgroundPlugin = new EventServerBackgroundPlugin();
                _backgroundPlugins.Add(eventServerBackgroundPlugin);

            }
            //if (EnvironmentManager.Instance.EnvironmentType == EnvironmentType.Administration)
            //{
            //    //_tabPlugins.Add(new TryggRetailTabPlugin());
            //    //_toolsOptionsDialogPlugins.Add(new TryggRetailToolsOptionDialogPlugin());
            //}

        }

        /// <summary>
        /// The main application is about to be in an undetermined state, either logging off or exiting.
        /// You can release resources at this point, it should match what you acquired during Init, so additional call to Init() will work.
        /// </summary>
        public override void Close()
        {
            _itemNodes.Clear();
            //_sidePanelPlugins.Clear();
            _viewItemPlugins.Clear();
            _settingsPanelPlugins.Clear();
            _backgroundPlugins.Clear();
            _workSpacePlugins.Clear();
            //_tabPlugins.Clear();
            //_viewItemToolbarPlugins.Clear();
            //_workSpaceToolbarPlugins.Clear();
            //_viewLayouts.Clear();
            //_toolsOptionsDialogPlugins.Clear();
        }

        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get
            {
                return TryggRetailPluginId;
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
            get { return "TryggRetail"; }
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
            get { return _topTreeNodeImage; }
        }



        #region Administration properties

        /// <summary>
        /// A list of server side configuration items in the administrator
        /// </summary>
        public override List<ItemNode> ItemNodes
        {
            get { return _itemNodes; }
        }


        /// <summary>
        /// A user control to display when the administrator clicks on the top TreeNode
        /// </summary>
        public override UserControl GenerateUserControl()
        {
            _treeNodeInofUserControl = new HelpPage(LicenseID, TryggRetailPluginId);
            return _treeNodeInofUserControl;
        }

        /// <summary>
        /// This property can be set to true, to be able to display your own help UserControl on the entire panel.
        /// When this is false - a standard top and left side is added by the system.
        /// </summary>
        public override bool UserControlFillEntirePanel
        {
            get { return false; }
        }
        #endregion

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
        public override Collection<SettingsPanelPlugin> SettingsPanelPlugins
        {
            get { return _settingsPanelPlugins; }
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
