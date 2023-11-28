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
using XProtectWebStreamPlugin.Client;

namespace XProtectWebStreamPlugin
{
    /// <summary>
    /// The PluginDefinition is the ‘entry’ point to any plugin.  
    /// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.  
    /// Several PluginDefinitions are allowed to be available within one DLL.
    /// Here the references to all other plugin known objects and classes are defined.
    /// The class is an abstract class where all implemented methods and properties need to be declared with override.
    /// The class is constructed when the environment is loading the DLL.
    /// </summary>
    public class XProtectWebStreamPluginDefinition : PluginDefinition
    {

        internal static Guid XProtectWebStreamPluginPluginId = new Guid("3d0c4567-6df7-49ca-a64c-f7781105a523");
        internal static Guid XProtectWebStreamPluginKind = new Guid("d8ebe58c-9ed3-4197-8c01-6e344f3be6e4");

        internal static Guid XProtectWebStreamPluginViewItemToolbarPluginId = new Guid("{6347FFCC-2663-4541-8E39-744F01C0684B}");

        #region Private fields


        //
        // Note that all the plugin are constructed during application start, and the constructors
        // should only contain code that references their own dll, e.g. resource load.

        private List<ViewItemToolbarPlugin> _viewItemToolbarPlugins = new List<ViewItemToolbarPlugin>();

        #endregion

        #region Initialization



        #endregion

        /// <summary>
        /// This method is called when the environment is up and running.
        /// Registration of Messages via RegisterReceiver can be done at this point.
        /// </summary>
        public override void Init()
        {
            if (EnvironmentManager.Instance.EnvironmentType == EnvironmentType.SmartClient)
            {
                _viewItemToolbarPlugins.Add(new XProtectWebStreamPluginViewItemToolbarPlugin());
            }
        }

        /// <summary>
        /// The main application is about to be in an undetermined state, either logging off or exiting.
        /// You can release resources at this point, it should match what you acquired during Init, so additional call to Init() will work.
        /// </summary>
        public override void Close()
        {
            _viewItemToolbarPlugins.Clear();
        }


        #region Identification Properties

        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get
            {
                return XProtectWebStreamPluginPluginId;
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
            get { return "XProtectWebStreamPlugin"; }
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


        /// <summary> 
        /// An extension plug-in to add to the view item toolbar in the Smart Client.
        /// </summary>
        public override List<ViewItemToolbarPlugin> ViewItemToolbarPlugins
        {
            get { return _viewItemToolbarPlugins; }
        }

        public override UserControl GenerateUserControl()
        {
            return null;
        }

        public override List<ItemNode> ItemNodes
        {
            get { return new List<ItemNode>(); }
        }


    }
}
