using REIDSearchAgent.SearchToolbar.Add;
using REIDSearchAgent.SearchToolbar.Remove;
using REIDSearchAgent.Settings;
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
using VideoOS.Platform.Search;

namespace REIDSearchAgent
{
    public class ReidPluginSearchAgentDefinition : PluginDefinition
    {
        private static System.Drawing.Image _topTreeNodeImage;

        internal static NodeREDConnection NodeRED;
        private Collection<SettingsPanelPlugin> _settingsPlugins = new Collection<SettingsPanelPlugin>();

        static ReidPluginSearchAgentDefinition()
        {
            _topTreeNodeImage = Properties.Resources.Server;
        }

        public override Guid Id
        {
            get
            {
                return new Guid("{6B62E450-1BFA-4B9C-A82D-4152B45DB279}");
            }
        }

        public override void Init()
        {
            base.Init();

            _settingsPlugins.Add(new ReidSettingsPlugin());
            //NodeRED = new NodeREDConnection(new Uri("http://172.16.100.5:1880"));
        }

        public override void Close()
        {
            NodeRED?.Close();
            _settingsPlugins.Clear();
        }

        public override string Name
        {
            get { return "REIDSearchAgent"; }
        }

        public override string Manufacturer
        {
            get
            {
                return "InSupport Nätverksvideo AB";
            }
        }


        public override System.Drawing.Image Icon
        {
            get { return _topTreeNodeImage; }
        }

        public override IEnumerable<SearchAgentPlugin> SearchAgentPlugins { get; } = new[] { new SearchAgent.ReidSearchAgentPlugin() };

        public override IEnumerable<SearchToolbarPlugin> SearchToolbarPlugins =>
            new List<SearchToolbarPlugin>() {
                new ReidAddToolbarPlugin(),
                new ReidEditToolbarPlugin(),
                new ReidRemoveToolbarPlugin() 
            };

        public override Collection<SettingsPanelPlugin> SettingsPanelPlugins => _settingsPlugins; // return the settings plugins, viewed in the Smart Client settings panel
    }
}
