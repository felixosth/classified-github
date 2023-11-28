using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using ESLogin.Background;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;
using ESLogin.Admin;
using System.Net;

namespace ESLogin
{
    /// <summary>
    /// The PluginDefinition is the ‘entry’ point to any plugin.  
    /// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.  
    /// Several PluginDefinitions are allowed to be available within one DLL.
    /// Here the references to all other plugin known objects and classes are defined.
    /// The class is an abstract class where all implemented methods and properties need to be declared with override.
    /// The class is constructed when the environment is loading the DLL.
    /// </summary>
    public class ESLoginDefinition : PluginDefinition
    {
        private static System.Drawing.Image _treeNodeImage;
        private static System.Drawing.Image _topTreeNodeImage;

        internal static Guid ESLoginPluginId = new Guid("97acfb82-1e4f-4a96-8d3a-a6135258e031");
        internal static Guid ESLoginKind = new Guid("38ce49ee-0c67-4d17-8619-5269d725b7d6");
        internal static Guid ESLoginBackgroundPlugin = new Guid("18fbcb63-b93d-4edf-a289-43bb19760b48");

        private List<BackgroundPlugin> _backgroundPlugins = new List<BackgroundPlugin>();
        private List<ItemNode> _itemNodes = new List<ItemNode>();

        public static string ServerName;

        #region Initialization

        /// <summary>
        /// Load resources 
        /// </summary>
        static ESLoginDefinition()
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

            ServerName = Configuration.Instance.GetItem(Configuration.Instance.ServerFQID)?.Name;

            _itemNodes.Add(new ItemNode(ESLoginKind, Guid.Empty,
                                            "TryggLogin", _treeNodeImage,
                                            "TryggLogin Management", _treeNodeImage,
                                            Category.Text, true,
                                            ItemsAllowed.One,
                                            new ESLoginItemManager(ESLoginKind),
                                            null));


            if(EnvironmentManager.Instance.EnvironmentType == EnvironmentType.Service)
                _backgroundPlugins.Add(new ESLoginBackgroundPlugin());
        }

        public override List<ItemNode> ItemNodes => _itemNodes;

        /// <summary>
        /// The main application is about to be in an undetermined state, either logging off or exiting.
        /// You can release resources at this point, it should match what you acquired during Init, so additional call to Init() will work.
        /// </summary>
        public override void Close()
        {
            adminHelpUserControl = null;
            _backgroundPlugins.Clear();
            _itemNodes.Clear();
        }

        AdminHelpUserControl adminHelpUserControl;
        public override UserControl GenerateUserControl()
        {
            adminHelpUserControl = new AdminHelpUserControl();
            return adminHelpUserControl;
        }


        #region Identification Properties

        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get
            {
                return ESLoginPluginId;
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
                return "1.0";
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

        /// <summary>
        /// Create and returns the background task.
        /// </summary>
        public override List<BackgroundPlugin> BackgroundPlugins
        {
            get { return _backgroundPlugins; }
        }
    }
}
