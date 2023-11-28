using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using VideoOS.Platform;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;

namespace TryggSync.Workspace
{
    public class TryggSyncWorkSpacePlugin : WorkSpacePlugin
    {

        /// <summary>
        /// The Id.
        /// </summary>
        public override Guid Id
        {
            get { return TryggSyncDefinition.TryggSyncWorkSpacePluginId; }
        }

        /// <summary>
        /// The name displayed on top
        /// </summary>
        public override string Name
        {
            get { return "TryggSync"; }
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
            //LoadProperties(true);

            //build view layout - modify to your needs. Here we use a matrix of 1000x1000 to define the layout 
            List<Rectangle> rectangles = new List<Rectangle>();
            rectangles.Add(new Rectangle(0, 0, 1000, 1000));      // Index 0 = Used by a camera below
            ViewAndLayoutItem.Layout = rectangles.ToArray();
            ViewAndLayoutItem.Name = Name;

            ViewAndLayoutItem.InsertViewItemPlugin(0, new TryggSyncWorkSpaceViewItemPlugin(), new Dictionary<string, string>());
        }

        /// <summary>
        /// Close workspace and clean up
        /// </summary>
        public override void Close()
        {
        }

        /// <summary>
        /// User modified something in setup mode
        /// </summary>
        /// <param name="index"></param>
        public override void ViewItemConfigurationModified(int index)
        {
            base.ViewItemConfigurationModified(index);

            //if (ViewAndLayoutItem.ViewItemId(index) == ViewAndLayoutItem.CameraBuiltinId)
            //{
            //    SetProperty("Camera" + index, ViewAndLayoutItem.ViewItemConfigurationString(index));
            //    SaveProperties(true);
            //}
        }

    }
}
