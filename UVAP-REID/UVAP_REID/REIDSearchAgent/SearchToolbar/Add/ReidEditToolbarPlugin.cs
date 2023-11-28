using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Search;

namespace REIDSearchAgent.SearchToolbar.Add
{
    public class ReidEditToolbarPlugin : SearchToolbarPlugin
    {
        public override Guid Id { get; protected set; } = new Guid("{04F3E29C-FAF8-463F-B702-A28646089705}");
        public override string Name { get; protected set; } = "ReidEditDb";

        public ReidEditToolbarPlugin()
        {
            this.SupportedSearchAgentPluginIds = new[] { REIDSearchAgent.SearchAgent.ReidSearchAgentPlugin._ID };
            this.ToolbarPluginOverflowMode = VideoOS.Platform.Client.ToolbarPluginOverflowMode.AsNeeded;
        }

        public override SearchToolbarPluginInstance GenerateSearchToolbarPluginInstance()
        {
            return new ReidEditToolbarInstance();
        }
    }
}
