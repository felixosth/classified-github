using SCTryggLogin.Login;
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

namespace SCTryggLogin
{
    /// <summary>
    /// The PluginDefinition is the ‘entry’ point to any plugin.  
    /// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.  
    /// Several PluginDefinitions are allowed to be available within one DLL.
    /// Here the references to all other plugin known objects and classes are defined.
    /// The class is an abstract class where all implemented methods and properties need to be declared with override.
    /// The class is constructed when the environment is loading the DLL.
    /// </summary>
    public class SCTryggLoginDefinition : PluginDefinition
    {
        internal static Guid SCTryggLoginPluginId = new Guid("eaac58dd-b3ae-41e2-ad06-13c9cbc7cff2");
        internal static Guid SCTryggLoginKind = new Guid("6ec017c8-9602-4d16-8eba-2d17dab028b7");

        List<LoginPlugin> loginPlugins = new List<LoginPlugin>();

        public override void Init()
        {
            //if (EnvironmentManager.Instance.EnvironmentType == EnvironmentType.SmartClient)
            //{
                loginPlugins.Add(new TryggLoginPlugin());
            //}
        }

        public override List<LoginPlugin> LoginPlugins => loginPlugins;

        public override void Close()
        {
        }

        #region Identification Properties

        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get
            {
                return SCTryggLoginPluginId;
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
            get { return "SCTryggLogin"; }
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
            get { return Properties.Resources.DummyItem; }
        }

        #endregion


    }
}
