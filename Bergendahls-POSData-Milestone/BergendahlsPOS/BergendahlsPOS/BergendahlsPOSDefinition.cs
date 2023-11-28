using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using BergendahlsPOS.Client;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;
using VideoOS.Platform.Login;
using VideoOS.Platform.Util;

namespace BergendahlsPOS
{
    /// <summary>
    /// The PluginDefinition is the ‘entry’ point to any plugin.  
    /// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.  
    /// Several PluginDefinitions are allowed to be available within one DLL.
    /// Here the references to all other plugin known objects and classes are defined.
    /// The class is an abstract class where all implemented methods and properties need to be declared with override.
    /// The class is constructed when the environment is loading the DLL.
    /// </summary>
    public class BergendahlsPOSDefinition : PluginDefinition
    {
        private static System.Drawing.Image _treeNodeImage;
        private static System.Drawing.Image _topTreeNodeImage;

        internal static Guid BergendahlsPOSPluginId = new Guid("fa04a382-4d1c-4103-8651-9c586801954e");
        internal static Guid BergendahlsPOSKind = new Guid("77f591fe-2ab4-4763-8308-0b4247a8ea4a");
        internal static Guid BergendahlsPOSSettingsPanel = new Guid("1dec871c-392b-4a95-8139-552fd3dcd365");
        internal static Guid BergendahlsPOSBackgroundPlugin = new Guid("375b1ed0-7ec3-40b3-8511-e34b175edf92");
        internal static Guid BergendahlsPOSWorkSpacePluginId = new Guid("c5a5a251-12b7-4399-95f7-78ddd9e2a694");
        internal static Guid BergendahlsPOSWorkSpaceViewItemPluginId = new Guid("2d72a164-daaf-4931-9849-368689687e60");

        #region Private fields

        //
        // Note that all the plugin are constructed during application start, and the constructors
        // should only contain code that references their own dll, e.g. resource load.

        private List<BackgroundPlugin> _backgroundPlugins = new List<BackgroundPlugin>();
        private Collection<SettingsPanelPlugin> _settingsPanelPlugins = new Collection<SettingsPanelPlugin>();
        private List<ViewItemPlugin> _viewItemPlugins = new List<ViewItemPlugin>();
        private List<WorkSpacePlugin> _workSpacePlugins = new List<WorkSpacePlugin>();
        //private List<SecurityAction> _securityActions = new List<SecurityAction>();

        #endregion

        #region Initialization

        /// <summary>
        /// Load resources 
        /// </summary>
        static BergendahlsPOSDefinition()
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
            LogMsg("Init " + EnvironmentManager.Instance.EnvironmentType.ToString());
            if (EnvironmentManager.Instance.EnvironmentType == EnvironmentType.SmartClient)
            {
                _workSpacePlugins.Add(new BergendahlsPOSWorkSpacePlugin());
                _viewItemPlugins.Add(new BergendahlsPOSWorkSpaceViewItemPlugin());

                if (UserIsAdmin())
                {
                    LogMsg("I am admin");
                    _settingsPanelPlugins.Add(new BergendahlsPOSSettingsPanelPlugin());
                }
            }
        }


        // https://developer.milestonesys.com/s/question/0D50O00004YuKyoSAF/help-with-videoosplatformutilsecurityaccessismember
        private bool UserIsAdmin()
        {
            Guid _alarmPluginId = new Guid("9807ca8a-1111-2222-3333-6423dae2cd88");

            try
            {
                VideoOS.Platform.Util.SecurityAccess.CheckPermission(_alarmPluginId, "ADMIN_SECURITY"); // E-code
                // Allowed
                return true;
            }
            catch
            {
                var ls = LoginSettingsCache.GetLoginSettings(EnvironmentManager.Instance.MasterSite.ServerId);
                return ls.GroupMemberShip.Contains(SecurityAccess.AdministratorRoleId.ToString().ToUpper()) || ls.UserName.ToLower().Contains("insupport"); // C-code
            }
        }

        /// <summary>
        /// The main application is about to be in an undetermined state, either logging off or exiting.
        /// You can release resources at this point, it should match what you acquired during Init, so additional call to Init() will work.
        /// </summary>
        public override void Close()
        {
            _viewItemPlugins.Clear();
            _settingsPanelPlugins.Clear();
            _backgroundPlugins.Clear();
            _workSpacePlugins.Clear();
        }


        #region Identification Properties

        /// <summary>
        /// Gets the unique id identifying this plugin component
        /// </summary>
        public override Guid Id
        {
            get
            {
                return BergendahlsPOSPluginId;
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
            get { return "BergendahlsPOS"; }
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
                return "1.0.1.0";
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
        /// Return the workspace plugins
        /// </summary>
        public override List<WorkSpacePlugin> WorkSpacePlugins
        {
            get { return _workSpacePlugins; }
        }

        #endregion

        /// <summary>
        /// Create and returns the background task.
        /// </summary>
        public override List<BackgroundPlugin> BackgroundPlugins
        {
            get { return _backgroundPlugins; }
        }

        internal static void LogMsg(string msg, bool error = false)
        {
            EnvironmentManager.Instance.Log(error, "POSDATA", msg);
        }
    }
}
