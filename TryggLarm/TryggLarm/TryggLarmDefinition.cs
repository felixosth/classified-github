using InSupport_LicenseSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TryggLarm.Admin;
using TryggLarm.Background;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;

namespace TryggLarm
{
    /// <summary>
    /// The PluginDefinition is the ‘entry’ point to any plugin.  
    /// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.  
    /// Several PluginDefinitions are allowed to be available within one DLL.
    /// Here the references to all other plugin known objects and classes are defined.
    /// The class is an abstract class where all implemented methods and properties need to be declared with override.
    /// The class is constructed when the environment is loading the DLL.
    /// </summary>
    public class TryggLarmDefinition : PluginDefinition
    {
        private static System.Drawing.Image _treeNodeImage;
        private static System.Drawing.Image _topTreeNodeImage;

        internal static Guid TryggLarmPluginId = new Guid("c631a0ee-4f7c-4cac-ac3a-71082623d3ce");
        public static Guid TryggLarmKind = new Guid("93a976ac-6b6e-4f7a-a64a-d8538b6e2c83");


        internal const string LicenseCommunicationFilter = "TryggLarm.License.Com";

        internal static Guid TryggLarmBackgroundPlugin = new Guid("baae1dd7-e4c7-4d37-b1d3-c84de7a628f8");

        #region Private fields

        Guid parentGuid = new Guid("E8F1033E-A54E-45E7-B4C5-84FF83FD81DE");

        private UserControl _treeNodeInofUserControl;

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
        static TryggLarmDefinition()
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
            _itemNodes.Add(new ItemNode(TryggLarmKind, Guid.Empty,
                                         "TryggLarm", Properties.Resources.insupport,
                                         "TryggLarm", _treeNodeImage,
                                         Category.Text, true,
                                         ItemsAllowed.Many,
                                         new TryggLarmItemManager(TryggLarmKind),
                                         null
                                         ));

            _backgroundPlugins.Add(new TryggLarmBackgroundPlugin());
        }

        /// <summary>
        /// The main application is about to be in an undetermined state, either logging off or exiting.
        /// You can release resources at this point, it should match what you acquired during Init, so additional call to Init() will work.
        /// </summary>
        public override void Close()
        {
            _itemNodes.Clear();
            _backgroundPlugins.Clear();
        }


        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get
            {
                return TryggLarmPluginId;
            }
        }

        /// <summary>
        /// This Guid can be defined on several different IPluginDefinitions with the same value,
        /// and will result in a combination of this top level ProductNode for several plugins.
        /// Set to Guid.Empty if no sharing is enabled.
        /// </summary>
        //public override Guid SharedNodeId
        //{
        //    get
        //    {
        //        return InSupport_ManagementNode.Global.SharedNodeGuid;
        //    }
        //}

        //public override string SharedNodeName => InSupport_ManagementNode.Global.SharedNodeName;

        /// <summary>
        /// Define name of top level Tree node - e.g. A product name
        /// </summary>
        public override string Name
        {
            get { return "TryggLarm"; }
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
            get { return Properties.Resources.insupport; }
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


        /// <summary>
        /// Create and returns the background task.
        /// </summary>
        public override List<BackgroundPlugin> BackgroundPlugins
        {
            get { return _backgroundPlugins; }
        }

    }
}
