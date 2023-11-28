using VideoOS.Platform.Client;

namespace TryggSupport.Client
{
    public class TryggSupportWorkSpaceViewItemManager : ViewItemManager
    {
        public TryggSupportWorkSpaceViewItemManager() : base("TryggSupportWorkSpaceViewItemManager")
        {
        }

        public override ViewItemWpfUserControl GenerateViewItemWpfUserControl()
        {
            return new TryggSupportWorkSpaceViewItemWpfUserControl();
        }
    }
}
