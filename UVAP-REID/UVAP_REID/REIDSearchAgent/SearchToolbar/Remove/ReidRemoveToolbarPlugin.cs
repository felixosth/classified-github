using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Search;

namespace REIDSearchAgent.SearchToolbar.Remove
{
    public class ReidRemoveToolbarPlugin : SearchToolbarPlugin
    {
        public override Guid Id { get; protected set; } = new Guid("{56D3F9BE-B634-480A-B96B-823D28D5E2EF}");
        public override string Name { get; protected set; } = "ReidRemoveFromDb";

        public ReidRemoveToolbarPlugin()
        {
            this.SupportedSearchAgentPluginIds = new[] { REIDSearchAgent.SearchAgent.ReidSearchAgentPlugin._ID };
            this.ToolbarPluginOverflowMode = VideoOS.Platform.Client.ToolbarPluginOverflowMode.AsNeeded;
        }

        public override SearchToolbarPluginInstance GenerateSearchToolbarPluginInstance()
        {
            return new ReidRemoveToolbarInstance();
        }
    }
}
