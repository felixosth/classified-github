using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using VideoOS.Platform;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;

namespace BergendahlsPOS.Client
{
    public class BergendahlsPOSWorkSpacePlugin : WorkSpacePlugin
    {
        /// <summary>
        /// The Id.
        /// </summary>
        public override Guid Id
        {
            get { return BergendahlsPOSDefinition.BergendahlsPOSWorkSpacePluginId; }
        }


        /// <summary>
        /// The name displayed on top
        /// </summary>
        public override string Name
        {
            get
            {
#if RELEASE
                return "POS data");
#else
                return "POS data (TEST)";
#endif
            }
        }

        /// <summary>
        /// We support setup mode
        /// </summary>
        public override bool IsSetupStateSupported
        {
            get { return false; }
        }

        /// <summary>
        /// Initializa the plugin
        /// </summary>
        public override void Init()
        {
            LoadProperties(true);
            List<Rectangle> rectangles = new List<Rectangle>();
            rectangles.Add(new Rectangle(000, 000, 1000, 1000));
            ViewAndLayoutItem.Layout = rectangles.ToArray();
            ViewAndLayoutItem.Name = Name;

            ViewAndLayoutItem.InsertViewItemPlugin(0, new BergendahlsPOSWorkSpaceViewItemPlugin(), new Dictionary<string, string>());
        }

        /// <summary>
        /// Close workspace and clean up
        /// </summary>
        public override void Close()
        {
        }

    }
}
