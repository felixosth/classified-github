using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using VideoOS.Platform;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;

namespace TryggSupport.Client
{
    public class TryggSupportWorkSpacePlugin : WorkSpacePlugin
    {

        /// <summary>
        /// The Id.
        /// </summary>
        public override Guid Id
        {
            get { return TryggSupportDefinition.TryggSupportWorkSpacePluginId; }
        }

        /// <summary>
        /// The name displayed on top
        /// </summary>
        public override string Name
        {
            get { return "TryggSupport"; }
        }

        /// <summary>
        /// We support setup mode
        /// </summary>
        public override bool IsSetupStateSupported
        {
            get { return true; }
        }

        /// <summary>
        /// Initializa the plugin
        /// </summary>
        public override void Init()
        {
            LoadProperties(true);

            //build view layout - modify to your needs. Here we use a matrix of 1000x1000 to define the layout 
            List<Rectangle> rectangles = new List<Rectangle>();
            //rectangles.Add(new Rectangle(000, 000, 200, 200));      // Index 0 = Used by a camera below
            //rectangles.Add(new Rectangle(200, 000, 800, 200));      // Index 1 = the sample ViewItem
            rectangles.Add(new Rectangle(000, 000, 1000, 1000));     // Index 2
            ViewAndLayoutItem.Layout = rectangles.ToArray();
            ViewAndLayoutItem.Name = Name;

            //add viewitems to view layout

            //Dictionary<String, String> properties = new Dictionary<string, string>();
            //properties.Add("CameraId", cameraItem != null ? cameraItem.FQID.ObjectId.ToString() : Guid.Empty.ToString());

            //ViewAndLayoutItem.InsertBuiltinViewItem(0, ViewAndLayoutItem.CameraBuiltinId, properties);

            ViewAndLayoutItem.InsertViewItemPlugin(0, new TryggSupportWorkSpaceViewItemPlugin(), new Dictionary<string, string>());

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
