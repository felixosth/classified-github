using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TryggLogin.Admin;
using TryggLogin.Background;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;

namespace TryggLogin
{
    /// <summary>
    /// The PluginDefinition is the ‘entry’ point to any plugin.  
    /// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.  
    /// Several PluginDefinitions are allowed to be available within one DLL.
    /// Here the references to all other plugin known objects and classes are defined.
    /// The class is an abstract class where all implemented methods and properties need to be declared with override.
    /// The class is constructed when the environment is loading the DLL.
    /// </summary>
    public class TryggLoginDefinition : PluginDefinition
    {
        private static System.Drawing.Image _treeNodeImage;
        private static System.Drawing.Image _topTreeNodeImage;

        internal static Guid TryggLoginPluginId = new Guid("{A911606B-4FCB-47FD-BBB8-FB058C8BEAE7}");
        internal static Guid TryggLoginKind = new Guid("{F38A8102-E59E-4A2D-B9DC-2715E2D7489E}");
        internal static Guid TryggLoginBackgroundPlugin = new Guid("{05358463-609B-411A-9EC6-C32EE19C6AE9}");


        #region Private fields

        //private UserControl _treeNodeInofUserControl;

        //
        // Note that all the plugin are constructed during application start, and the constructors
        // should only contain code that references their own dll, e.g. resource load.

        private List<BackgroundPlugin> _backgroundPlugins = new List<BackgroundPlugin>();
        private List<ItemNode> _itemNodes = new List<ItemNode>();

        #endregion

        #region Initialization

        /// <summary>
        /// Load resources 
        /// </summary>
        static TryggLoginDefinition()
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
            if(EnvironmentManager.Instance.EnvironmentType == EnvironmentType.Administration)
                _itemNodes.Add(new ItemNode(TryggLoginKind, Guid.Empty,
                                             "TryggLogin", _treeNodeImage,
                                             "TryggLogin Management", _treeNodeImage,
                                             Category.Text, true,
                                             ItemsAllowed.Many,
                                             new TryggLoginItemManager(TryggLoginKind),
                                             null));

            if (EnvironmentManager.Instance.EnvironmentType == EnvironmentType.Service)
            {
                _backgroundPlugins.Add(new TryggLoginBackgroundPlugin());
            }
        }

        public override string SharedNodeName => InSupport_ManagementNode.Global.SharedNodeName;
        public override Guid SharedNodeId => InSupport_ManagementNode.Global.SharedNodeGuid;


        /// <summary>
        /// The main application is about to be in an undetermined state, either logging off or exiting.
        /// You can release resources at this point, it should match what you acquired during Init, so additional call to Init() will work.
        /// </summary>
        public override void Close()
        {
            _itemNodes.Clear();
            _backgroundPlugins.Clear();
        }


        #region Identification Properties

        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get
            {
                return TryggLoginPluginId;
            }
        }


        /// <summary>
        /// Define name of top level Tree node - e.g. A product name
        /// </summary>
        public override string Name
        {
            get { return "TryggLogin"; }
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
        /// A user control to display when the administrator clicks on the top TreeNode
        /// </summary>
        //public override UserControl GenerateUserControl()
        //{
        //    _treeNodeInofUserControl = new HelpPage();
        //    return _treeNodeInofUserControl;
        //}

        /// <summary>
        /// This property can be set to true, to be able to display your own help UserControl on the entire panel.
        /// When this is false - a standard top and left side is added by the system.
        /// </summary>
        public override bool UserControlFillEntirePanel
        {
            get { return false; }
        }
        #endregion

        public override List<LoginPlugin> LoginPlugins
        {
            get {
                return new List<LoginPlugin>()
                {
                };
            }
        }

        /// <summary>
        /// Create and returns the background task.
        /// </summary>
        public override List<BackgroundPlugin> BackgroundPlugins
        {
            get { return _backgroundPlugins; }
        }


    }
    internal static class Logger
    {
        public static void Log(string msg, bool error = false)
        {
            EnvironmentManager.Instance.Log(error, "TryggLogin", msg);
        }
    }
}
