using VideoOS.Platform.Client;

namespace BergendahlsPOS.Client
{
    public class BergendahlsPOSWorkSpaceViewItemManager : ViewItemManager
    {

        public BergendahlsPOSWorkSpaceViewItemManager() : base("BergendahlsPOSWorkSpaceViewItemManager")
        {
        }

        public override ViewItemWpfUserControl GenerateViewItemWpfUserControl()
        {
            return new BergendahlsPOSWorkSpaceViewItemWpfUserControl();
        }
    }
}
