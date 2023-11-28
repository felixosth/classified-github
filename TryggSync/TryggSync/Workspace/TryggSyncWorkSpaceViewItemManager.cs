using VideoOS.Platform.Client;

namespace TryggSync.Workspace
{
    public class TryggSyncWorkSpaceViewItemManager : ViewItemManager
    {
        public TryggSyncWorkSpaceViewItemManager() : base("TryggSyncWorkSpaceViewItemManager")
        {
        }

        public override ViewItemWpfUserControl GenerateViewItemWpfUserControl()
        {
            return new TryggSyncWorkSpaceViewItemWpfUserControl();
        }


    }
}
