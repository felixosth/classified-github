using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UVAPMipPlugin.Background.Visualizers;
using VideoOS.Platform;
using VideoOS.Platform.Background;
using VideoOS.Platform.Client;

namespace UVAPMipPlugin.Background
{
    // This class runs in the background and is initialized by the plugin definition.
    // Derives from API class BackgrounPlugin
    public class VisualizerBackgroundPlugin : BackgroundPlugin
    {
        internal static bool EnableSkeleton { get; set; }
        internal static bool EnableHeaddetection { get; set; }

        public override Guid Id => new Guid("{27AF873C-6ECB-4C3B-9554-AB89576431F1}");

        public override string Name => nameof(VisualizerBackgroundPlugin);

        public override List<EnvironmentType> TargetEnvironments => new List<EnvironmentType>() { EnvironmentType.SmartClient }; // target the smart client

        private List<BaseVisualizer> Visualizers { get; set; } = new List<BaseVisualizer>();

        public override void Init()
        {
            // Fetching configuration from the OverlayOptionsPlugin
            // The user can disable the visualizers from the Smart Client settings panel (cog wheel on the upper right)
            System.Xml.XmlNode result = VideoOS.Platform.Configuration.Instance.GetOptionsConfiguration(Settings.OverlaySettingsPlugin.ID, true);
            EnableSkeleton = true;
            EnableHeaddetection = true;

            if(result != null)
            {
                foreach(XmlNode node in result.ChildNodes)
                {
                    switch(node.Attributes["name"].Value)
                    {
                        case Settings.OverlaySettingsPlugin.SkeletonKey:
                            EnableSkeleton = bool.Parse(node.Attributes["value"].Value);
                            break;
                        case Settings.OverlaySettingsPlugin.HeadKey:
                            EnableHeaddetection = bool.Parse(node.Attributes["value"].Value);
                            break;
                    }
                }
            }

            // Subscribe to new ImageControllers (The frames holding cameras)
            ClientControl.Instance.NewImageViewerControlEvent += Instance_NewImageViewerControlEvent;
        }

        // Fired when the Smart Client opens a new camera
        private void Instance_NewImageViewerControlEvent(ImageViewerAddOn imageViewerAddOn)
        {
            if (imageViewerAddOn.ImageViewerType != ImageViewerType.CameraViewItem)
                return;

            var cameraItem = Configuration.Instance.GetItem(imageViewerAddOn.CameraFQID);

            if (cameraItem == null)
                return;

            var relatedMetadata = cameraItem.GetRelated().Where(i => i.FQID.Kind == Kind.Metadata); // Find the related metadata items from the configuration api

            BaseVisualizer visualizer = null;

            foreach(var metadataDevice in relatedMetadata)
            {
                var lowName = metadataDevice.Name.ToLower();

                if(lowName.Contains("skeleton") && EnableSkeleton)
                {
                    visualizer = new SkeletonVisualizer(imageViewerAddOn, metadataDevice, cameraItem);
                }
                else if(lowName.Contains("head") && EnableHeaddetection)
                {
                    visualizer = new HeadVisualizer(imageViewerAddOn, metadataDevice, cameraItem);
                }
            }

            if (visualizer != null)
            {
                lock (Visualizers)
                {
                    Visualizers.Add(visualizer);
                }

                imageViewerAddOn.ShowMetadataOverlay = false;

                imageViewerAddOn.CloseEvent += ImageViewerAddOn_CloseEvent;
            }
        }

        // The camera is no longer visible in the Smart Client, close the visualizers
        private void ImageViewerAddOn_CloseEvent(object sender, EventArgs e)
        {
            lock (Visualizers)
            {
                foreach (var visualizer in Visualizers)
                {
                    if (visualizer.ImageViewer == sender as ImageViewerAddOn)
                    {
                        visualizer.Close();
                    }
                }

                Visualizers.RemoveAll(v => v.Closed);
            }
        }

        public override void Close()
        {
            foreach (var v in Visualizers)
            {
                v.Close();
            }
        }
    }
}
