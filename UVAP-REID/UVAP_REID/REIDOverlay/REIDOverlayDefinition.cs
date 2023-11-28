using REIDOverlay.Background;
using REIDShared;
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

namespace REIDOverlay
{
    /// <summary>
    /// The PluginDefinition is the ‘entry’ point to any plugin.  
    /// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.  
    /// Several PluginDefinitions are allowed to be available within one DLL.
    /// Here the references to all other plugin known objects and classes are defined.
    /// The class is an abstract class where all implemented methods and properties need to be declared with override.
    /// The class is constructed when the environment is loading the DLL.
    /// </summary>
    public class REIDOverlayDefinition : PluginDefinition
    {
        private static System.Drawing.Image _treeNodeImage;
        private static System.Drawing.Image _topTreeNodeImage;

        internal static Guid REIDOverlayPluginId = new Guid("f6245e70-7034-4f13-ad0e-60dc7d5621a3");
        internal static Guid REIDOverlayKind = new Guid("9a9cb7f8-d659-45d2-bf56-5a08d37db982");

        internal static NodeREDConnection NodeRED;

        private List<BackgroundPlugin> _backgroundPlugins = new List<BackgroundPlugin>();


        static REIDOverlayDefinition()
        {
            _treeNodeImage = Properties.Resources.DummyItem;
            _topTreeNodeImage = Properties.Resources.Server;
        }


        internal static Image TreeNodeImage
        {
            get { return _treeNodeImage; }
        }

        /// <summary>
        /// This method is called when the environment is up and running.
        /// Registration of Messages via RegisterReceiver can be done at this point.
        /// </summary>
        public override void Init()
        {
            if (EnvironmentManager.Instance.EnvironmentType == EnvironmentType.SmartClient)
            {
                var url = REIDShared.Settings.GetNodeREDUrl();
                if(url != null)
                {
                    NodeRED = new NodeREDConnection(new Uri(url));

                    if (NodeRED.Connect())
                        NodeRED.StartPoll();
                    else
                        NodeRED = null;
                }

                _backgroundPlugins.Add(new VisualizerBackgroundPlugin());
            }
        }

        /// <summary>
        /// The main application is about to be in an undetermined state, either logging off or exiting.
        /// You can release resources at this point, it should match what you acquired during Init, so additional call to Init() will work.
        /// </summary>
        public override void Close()
        {
            NodeRED?.Close();
            _backgroundPlugins.Clear();
        }


        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get
            {
                return REIDOverlayPluginId;
            }
        }

        /// <summary>
        /// Define name of top level Tree node - e.g. A product name
        /// </summary>
        public override string Name
        {
            get { return "REIDOverlay"; }
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
        /// Icon to be used on top level - e.g. a product or company logo
        /// </summary>
        public override System.Drawing.Image Icon
        {
            get { return _topTreeNodeImage; }
        }


        public override List<BackgroundPlugin> BackgroundPlugins => _backgroundPlugins;
    }
}
