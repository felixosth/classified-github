using RegSormlandMilestonePlugin.Admin;
using RegSormlandMilestonePlugin.Background;
using RegSormlandMilestonePlugin.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;

namespace RegSormlandMilestonePlugin
{
    /// <summary>
    /// The PluginDefinition is the ‘entry’ point to any plugin.  
    /// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.  
    /// Several PluginDefinitions are allowed to be available within one DLL.
    /// Here the references to all other plugin known objects and classes are defined.
    /// The class is an abstract class where all implemented methods and properties need to be declared with override.
    /// The class is constructed when the environment is loading the DLL.
    /// </summary>
    public class RegSormlandMilestonePluginDefinition : PluginDefinition
    {
        private static System.Drawing.Image _treeNodeImage;
        private static System.Drawing.Image _topTreeNodeImage;

        internal static Guid ConfigItemGuid = new Guid("{836CFDEA-BEED-43E8-B686-1C522CDD5A97}");

        internal static Guid RegSormlandMilestonePluginPluginId = new Guid("a42a882f-017b-4c97-8af6-80c4d998524e");
        internal static Guid RegSormlandMilestonePluginKind = new Guid("3a5f47c7-2f39-486f-af0d-f141b97c59a4");
        internal static Guid RegSormlandMilestonePluginSidePanel = new Guid("861d0e1b-432c-41ab-8586-2e58f0d03747");
        internal static Guid RegSormlandMilestonePluginViewItemPlugin = new Guid("9bd41685-c14e-4f04-b335-9b6b017758be");
        internal static Guid RegSormlandMilestonePluginSettingsPanel = new Guid("f3369553-6b6e-4eeb-8d94-4250e9ccfa20");
        internal static Guid RegSormlandMilestonePluginBackgroundPlugin = new Guid("921fddb7-a872-4ed7-b1b6-d662b8502cec");
        internal static Guid RegSormlandMilestonePluginWorkSpacePluginId = new Guid("cf5599ba-467b-4447-aaa0-057e0bfd24cf");
        internal static Guid RegSormlandMilestonePluginWorkSpaceViewItemPluginId = new Guid("f7c55bbf-5b16-4bc8-9fd7-bf030e4ed3d4");
        internal static Guid RegSormlandMilestonePluginTabPluginId = new Guid("b3b838e7-f53c-4285-bd99-1f8d6509259c");
        internal static Guid RegSormlandMilestonePluginViewLayoutId = new Guid("2438bcac-e9b6-4997-a59a-4083716b885c");
        // IMPORTANT! Due to shortcoming in Visual Studio template the below cannot be automatically replaced with proper unique GUIDs, so you will have to do it yourself
        internal static Guid RegSormlandMilestonePluginWorkSpaceToolbarPluginId = new Guid("22222222-2222-2222-2222-222222222222");
        internal static Guid RegSormlandMilestonePluginViewItemToolbarPluginId = new Guid("33333333-3333-3333-3333-333333333333");
        internal static Guid RegSormlandMilestonePluginToolsOptionDialogPluginId = new Guid("44444444-4444-4444-4444-444444444444");

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
        static RegSormlandMilestonePluginDefinition()
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
            // Populate all relevant lists with your plugins etc.
            _itemNodes.Add(new ItemNode(RegSormlandMilestonePluginKind, Guid.Empty,
                                         "RegSormlandMilestonePlugin", _treeNodeImage,
                                         "RegSormlandMilestonePlugins", _treeNodeImage,
                                         Category.Text, true,
                                         ItemsAllowed.One,
                                         new RegSormlandMilestonePluginItemManager(RegSormlandMilestonePluginKind),
                                         null
                                         ));

            if (EnvironmentManager.Instance.EnvironmentType == EnvironmentType.SmartClient)
            {
                _workSpacePlugins.Add(new RegSormlandMilestonePluginWorkSpacePlugin());
                _viewItemPlugins.Add(new RegSormlandMilestonePluginWorkSpaceViewItemPlugin());
                _backgroundPlugins.Add(new RegSormlandMilestonePluginBackgroundPlugin());
            }

            //_backgroundPlugins.Add(new RegSormlandMilestonePluginBackgroundPlugin());
        }

        /// <summary>
        /// The main application is about to be in an undetermined state, either logging off or exiting.
        /// You can release resources at this point, it should match what you acquired during Init, so additional call to Init() will work.
        /// </summary>
        public override void Close()
        {
            _itemNodes.Clear();
            _sidePanelPlugins.Clear();
            _viewItemPlugins.Clear();
            _settingsPanelPlugins.Clear();
            _backgroundPlugins.Clear();
            _workSpacePlugins.Clear();
            _tabPlugins.Clear();
            _viewItemToolbarPlugins.Clear();
            _workSpaceToolbarPlugins.Clear();
            _toolsOptionsDialogPlugins.Clear();
        }

        /// <summary>
        /// Return any new messages that this plugin can use in SendMessage or PostMessage,
        /// or has a Receiver set up to listen for.
        /// The suggested format is: "YourCompany.Area.MessageId"
        /// </summary>
        public override List<string> PluginDefinedMessageIds
        {
            get
            {
                return _messageIdStrings;
            }
        }

        /// <summary>
        /// If authorization is to be used, add the SecurityActions the entire plugin 
        /// would like to be available.  E.g. Application level authorization.
        /// </summary>
        public override List<SecurityAction> SecurityActions
        {
            get
            {
                return _securityActions;
            }
            set
            {
            }
        }

        #region Identification Properties

        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get
            {
                return RegSormlandMilestonePluginPluginId;
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
            get { return "RegSormlandMilestonePlugin"; }
        }

        /// <summary>
        /// Your company name
        /// </summary>
        public override string Manufacturer
        {
            get
            {
                return "Your Company name";
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

        #endregion


        #region Administration properties

        /// <summary>
        /// A list of server side configuration items in the administrator
        /// </summary>
        public override List<ItemNode> ItemNodes
        {
            get { return _itemNodes; }
        }

        /// <summary>
        /// An extension plug-in running in the Administrator to add a tab for built-in devices and hardware.
        /// </summary>
        public override ICollection<TabPlugin> TabPlugins
        {
            get { return _tabPlugins; }
        }

        /// <summary>
        /// An extension plug-in running in the Administrator to add more tabs to the Tools-Options dialog.
        /// </summary>
        public override List<ToolsOptionsDialogPlugin> ToolsOptionsDialogPlugins
        {
            get { return _toolsOptionsDialogPlugins; }
        }

        /// <summary>
        /// A user control to display when the administrator clicks on the top TreeNode
        /// </summary>
        public override UserControl GenerateUserControl()
        {
            _treeNodeInofUserControl = new HelpPage();
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

        /// <summary> 
        /// An extension plugin to add to the side panel of the Smart Client.
        /// </summary>
        public override List<SidePanelPlugin> SidePanelPlugins
        {
            get { return _sidePanelPlugins; }
        }

        /// <summary>
        /// Return the workspace plugins
        /// </summary>
        public override List<WorkSpacePlugin> WorkSpacePlugins
        {
            get { return _workSpacePlugins; }
        }

        /// <summary> 
        /// An extension plug-in to add to the view item toolbar in the Smart Client.
        /// </summary>
        public override List<ViewItemToolbarPlugin> ViewItemToolbarPlugins
        {
            get { return _viewItemToolbarPlugins; }
        }

        /// <summary> 
        /// An extension plug-in to add to the work space toolbar in the Smart Client.
        /// </summary>
        public override List<WorkSpaceToolbarPlugin> WorkSpaceToolbarPlugins
        {
            get { return _workSpaceToolbarPlugins; }
        }

        /// <summary>
        /// An extension plug-in running in the Smart Client to provide extra view layouts.
        /// </summary>
        public override List<ViewLayout> ViewLayouts
        {
            get { return _viewLayouts; }
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
