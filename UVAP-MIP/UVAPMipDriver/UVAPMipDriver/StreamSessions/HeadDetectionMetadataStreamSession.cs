using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvapShared;
using UvapShared.Driver;
using UvapShared.Objects;
using VideoOS.Platform.DriverFramework.Data;
using VideoOS.Platform.DriverFramework.Data.Settings;
using VideoOS.Platform.DriverFramework.Managers;
using VideoOS.Platform.DriverFramework.Utilities;
using VideoOS.Platform.Metadata;

namespace UVAPMipDriver
{
    /// <summary>
    /// StreamSession class used to fetch HeadDetection data.
    /// </summary>
    internal class HeadDetectionMetadataStreamSession : UVAPBaseStreamSession
    {
        private Consumer KafkaConsumer { get; set; }

        private int FrameWidth, FrameHeight;

        private bool UseNativeMode { get; set; }
        private bool DrawText { get; set; }

        private uint LineThickness { get; set; }

        private DisplayColor BoundingBoxColor { get; set; }
        private DisplayColor TextColor { get; set; }

        IDictionary<string, string> settings;

        public HeadDetectionMetadataStreamSession(IDictionary<string, string> settings, Consumer consumer, ISettingsManager settingsManager, UVAPMipDriverConnectionManager connectionManager, Guid sessionId, string deviceId, Guid streamId, int channel) :
            base(settingsManager, connectionManager, sessionId, deviceId, streamId)
        {
            this.settings = settings;
            int.TryParse(settings[Constants.UvapFrameWidthSetting], out FrameWidth);
            int.TryParse(settings[Constants.UvapFrameHeightSetting], out FrameHeight);
            LineThickness = uint.Parse(settings[Constants.UvapLineThicknessNameSetting]);
            UseNativeMode = bool.Parse(settings[Constants.UvapHeaddetectionNativemodeNameSetting]);
            DrawText = bool.Parse(settings[Constants.UvapShowTextInNativeNameSetting]);
            UpdateColor();
            UpdateTextColor();

            Channel = channel;
            this.KafkaConsumer = consumer;
        }

        // The frame data is fetched from the Kafka server in this method. Collect all frames produced by Kafka untill the EndOfFrame is true.
        protected override bool GetLiveFrameInternal(TimeSpan timeout, out BaseDataHeader header, out byte[] data)
        {
            header = null;

            List<HeadDetection> headDetections = new List<HeadDetection>();
            DateTime dt;
            while(true)
            {
                var consumption = KafkaConsumer.Consume();
                var hd = HeadDetection.FromString(consumption.Value);

                if (!hd.EndOfFrame)
                {
                    headDetections.Add(hd);
                }
                else
                {
                    dt = consumption.Timestamp.UtcDateTime;
                    break;
                }
            }

            if (UseNativeMode) // If we're using the native mode, the headdetection data format will be in onvif. This format can be parsed by the Smart Client without a plugin.
                data = HeadDetectionsToBoundingBoxes(headDetections, dt, FrameWidth, FrameHeight); // This is not supported by the search agent
            else
                data = Encoding.UTF8.GetBytes(SlimHeadDetection.ToSlim(JsonConvert.SerializeObject(headDetections))); // json format

            if (data.Length == 0)
                return false;

            header = new MetadataHeader()
            {
                Length = (ulong)data.Length,
                SequenceNumber = _sequence++,
                Timestamp = dt
            };
            return true;
        }

        /// <summary>
        /// This format can be read by the Smart Client natively and doesn't need any plugin to visualize the data.
        /// This doesn't work for the search plugin.
        /// </summary>
        byte[] HeadDetectionsToBoundingBoxes(List<HeadDetection> headDetections, DateTime dt, float imgWidth, float imgHeight)
        {
            List<OnvifObject> onvifObjects = new List<OnvifObject>();
            Transformation transformation = new Transformation()
            {
                Translate = new Vector()
                {
                    X = -1,
                    Y = -1
                },
                Scale = new Vector()
                {
                    X = 2f / imgWidth,
                    Y = 2f / imgHeight
                }
            };

            for (int i = 0; i < headDetections.Count; i++)
            {
                var boundingBox = headDetections[i].BoundingBox;

                var onvifAppearance = new Appearance()
                {
                    Class = new OnvifClass()
                    {
                        ClassCandidates =
                            {
                                new ClassCandidate()
                                {
                                    Type = "Head",
                                    Likelihood = headDetections[i].Confidence
                                }
                            }
                    },
                    Shape = new Shape()
                    {
                        BoundingBox = new Rectangle()
                        {
                            Left = boundingBox.X,
                            Right = boundingBox.X + boundingBox.Width,
                            Bottom = imgHeight - boundingBox.Y,
                            Top = imgHeight - boundingBox.Y - boundingBox.Height,
                            LineColor = BoundingBoxColor,
                            LineDisplayPixelThickness = LineThickness
                        }
                    }
                };

                if(DrawText)
                {
                    onvifAppearance.Description = new DisplayText()
                    {
                        Value = $"{(headDetections[i].Confidence * 100).ToString("0")}%",
                        Color = TextColor
                    };
                }

                onvifObjects.Add(new OnvifObject(i)
                {
                    Appearance = onvifAppearance
                });
            }

            var metadata = new MetadataStream()
            {
                VideoAnalyticsItems =
                    {
                        new VideoAnalytics
                        {
                            Frames =
                            {
                                new Frame(dt)
                                {
                                    Transformation = transformation
                                }
                            }
                        }
                    },
            };
            metadata.VideoAnalyticsItems[0].Frames[0].Objects.AddRange(onvifObjects);

            return Encoding.UTF8.GetBytes(new MetadataSerializer().WriteMetadataXml(metadata));
        }


        protected override void InnerClose()
        {
            KafkaConsumer.Close();
        }

        // Update our values when the settings are changed by the user in the Management Client
        protected override void MetadataSettingChanged(MetadataSetting setting)
        {
            settings[setting.Key] = setting.Value;
            switch(setting.Key)
            {
                case Constants.UvapTextColorSetting:
                case Constants.UvapTextColorTransparencySetting:
                    UpdateTextColor();
                    break;
                case Constants.UvapBoundingBoxColorTransparencySetting:
                case Constants.UvapBoundingBoxColorSetting:
                    UpdateColor();
                    break;
                case Constants.UvapHeaddetectionNativemodeNameSetting:
                    UseNativeMode = bool.Parse(settings[Constants.UvapHeaddetectionNativemodeNameSetting]);
                    break;
                case Constants.UvapFrameWidthSetting:
                case Constants.UvapFrameHeightSetting:
                    int.TryParse(settings[Constants.UvapFrameWidthSetting], out FrameWidth);
                    int.TryParse(settings[Constants.UvapFrameHeightSetting], out FrameHeight);
                    break;
                case Constants.UvapShowTextInNativeNameSetting:
                    DrawText = bool.Parse(settings[Constants.UvapShowTextInNativeNameSetting]);
                    break;
                case Constants.UvapLineThicknessNameSetting:
                    LineThickness = uint.Parse(settings[Constants.UvapLineThicknessNameSetting]);
                    break;
            }
        }

        void UpdateTextColor()
        {
            try
            {
                string textColor = $"#{PercentToAlphaHex(int.Parse(settings[Constants.UvapTextColorTransparencySetting]))}{settings[Constants.UvapTextColorSetting].Replace("#", "")}";
                TextColor = DisplayColor.ParseArgbString(textColor);
            }
            catch
            {
                Toolbox.Log.Trace("Unable to parse text color and transparency string");
                TextColor = new DisplayColor(255, 0, 0);
            }
        }

        void UpdateColor()
        {
            try
            {
                string lineColor = $"#{PercentToAlphaHex(int.Parse(settings[Constants.UvapBoundingBoxColorTransparencySetting]))}{settings[Constants.UvapBoundingBoxColorSetting].Replace("#", "")}";
                BoundingBoxColor = DisplayColor.ParseArgbString(lineColor);
            }
            catch
            {
                Toolbox.Log.Trace("Unable to parse boundingbox color and transparency string");
                BoundingBoxColor = new DisplayColor(255, 0, 0);
            }
        }

        string PercentToAlphaHex(int p)
        {
            int percent = Math.Max(0, Math.Min(100, p));
            int alphaInt = (int)Math.Round((double)percent / 100 * 255);
            return alphaInt.ToString("X");
        }
    }
}
