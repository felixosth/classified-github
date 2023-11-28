using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using UVAPSearchAgent.SkeletonSearchAgent;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;
using VideoOS.Platform.Search;

namespace UVAPSearchAgent
{
    /// <summary>
    /// The PluginDefinition is the ‘entry’ point to any plugin.  
    /// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.  
    /// Several PluginDefinitions are allowed to be available within one DLL.
    /// Here the references to all other plugin known objects and classes are defined.
    /// The class is an abstract class where all implemented methods and properties need to be declared with override.
    /// The class is constructed when the environment is loading the DLL.
    /// </summary>
    public class UVAPSearchAgentPluginDefinition : PluginDefinition
    {
        private static System.Drawing.Image _topTreeNodeImage;

        static UVAPSearchAgentPluginDefinition()
        {
            _topTreeNodeImage = Properties.Resources.Server;
        }

        public override Guid Id
        {
            get
            {
                return new Guid("7eda1a06-e7b1-4eae-a198-1d7d448108fc");
            }
        }


        /// <summary>
        /// Define name of top level Tree node - e.g. A product name
        /// </summary>
        public override string Name
        {
            get { return "UVAP Search Agent"; }
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

        public override IEnumerable<SearchAgentPlugin> SearchAgentPlugins { get; } = new List<SearchAgentPlugin>() // Return the search agent plugins
        {
            new SkeletonSearchAgentPlugin(),
            new HeadSearchAgent.HeadSearchAgentPlugin()
        };

        public override IEnumerable<SearchUserControlsPlugin> SearchUserControlsPlugins { get; } = new[] { new Shared.UvapSearchUserControlsPlugin() }; // return the user control plugin
    }
}
