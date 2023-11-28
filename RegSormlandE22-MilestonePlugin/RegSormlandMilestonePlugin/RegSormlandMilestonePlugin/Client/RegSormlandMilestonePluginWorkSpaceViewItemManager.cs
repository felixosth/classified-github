using RegSormlandMilestonePlugin.Shared;
using System.Collections.Generic;
using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace RegSormlandMilestonePlugin.Client
{
    public class RegSormlandMilestonePluginWorkSpaceViewItemManager : ViewItemManager
    {
        public RegSormlandMilestonePluginWorkSpaceViewItemManager() : base("RegSormlandMilestonePluginWorkSpaceViewItemManager")
        {
        }

        public override void PropertiesLoaded()
        {

        }

        public override ViewItemWpfUserControl GenerateViewItemWpfUserControl()
        {
            return new RegSormlandMilestonePluginWorkSpaceViewItemWpfUserControl(this);
        }
    }
}
