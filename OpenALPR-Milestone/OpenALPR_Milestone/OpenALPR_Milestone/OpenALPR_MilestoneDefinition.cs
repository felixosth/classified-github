using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using OpenALPR_Milestone.Admin;
using OpenALPR_Milestone.Background;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;

namespace OpenALPR_Milestone
{
    /// <summary>
    /// The PluginDefinition is the ‘entry’ point to any plugin.  
    /// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.  
    /// Several PluginDefinitions are allowed to be available within one DLL.
    /// Here the references to all other plugin known objects and classes are defined.
    /// The class is an abstract class where all implemented methods and properties need to be declared with override.
    /// The class is constructed when the environment is loading the DLL.
    /// </summary>
    public class OpenALPR_MilestoneDefinition : PluginDefinition
    {
        private static System.Drawing.Image _treeNodeImage;
        private static System.Drawing.Image _topTreeNodeImage;

        internal static Guid OpenALPR_MilestonePluginId = new Guid("1a0a0f1e-4dd7-449d-9968-ad29f138410b");
        internal static Guid OpenALPR_MilestoneKind = new Guid("76dc402d-d040-4fb2-8efd-457c4eeb322b");
        internal static Guid OpenALPR_MilestoneBackgroundPlugin = new Guid("cc25e282-c3e4-4900-ae11-0070880f67e4");

        internal static Guid OpenALPR_MilestoneConfigItemId = new Guid("{58907CB3-980E-4D55-840C-8CACBF6405C7}");
        internal const string LicenseCommunicationFilter = "OpenALPR.InSupport.License.Com";


        #region Private fields

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
        static OpenALPR_MilestoneDefinition()
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
            _itemNodes.Add(new ItemNode(OpenALPR_MilestoneKind, Guid.Empty,
                                         "OpenALPR_Milestone", _treeNodeImage,
                                         "Configuration", _treeNodeImage,
                                         Category.Text, true,
                                         ItemsAllowed.One,
                                         new OpenALPR_MilestoneItemManager(OpenALPR_MilestoneKind),
                                         null
                                         ));

            if (EnvironmentManager.Instance.EnvironmentType == EnvironmentType.Service)
                _backgroundPlugins.Add(new OpenALPR_MilestoneBackgroundPlugin());
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


        #region Identification Properties

        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get
            {
                return OpenALPR_MilestonePluginId;
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
            get { return "OpenALPR_Milestone"; }
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

    internal static class Logger
    {
        public static void Log(string msg, bool error = false)
        {
            EnvironmentManager.Instance.Log(error, "OpenALPR", msg);
        }
    }
}
