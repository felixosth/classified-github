using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TryggSTORE.Http;

namespace TryggSTORE.MJPEG.FrameCreators
{
    internal class OccupancyFrameCreator : FrameCreatorBase
    {
        internal override string Name => nameof(OccupancyFrameCreator);
        Font dateTimeFont, stopFont, underLogoFont;

        Image image;

        Color redGradientColor1 = Color.FromArgb(210, 50, 50), redGradientColor2 = Color.FromArgb(145, 10, 10);
        Color yellowGradientColor1 = Color.FromArgb(255, 220, 0), yellowGradientColor2 = Color.FromArgb(255, 150, 0);
        Color greenGradientColor1 = Color.FromArgb(50, 210, 50), greenGradientColor2 = Color.FromArgb(10, 145, 10);

        Image groupImage, companyLogo;
        PrivateFontCollection privateFontCollection = new PrivateFontCollection();

        ThresholdMode lastThresholdMode = ThresholdMode.UnderThreshold;

        public OccupancyFrameCreator()
        {
            this.UpdateInterval = 500;
            this.WaitInterval = 50;


            dateTimeFont = new Font("Helvetica", 56, FontStyle.Regular, GraphicsUnit.Pixel);
            stopFont = new Font("Helvetica", 110, FontStyle.Regular, GraphicsUnit.Pixel);


            image = CreateImage(1920, 1080, 300.0F);

            //var fontPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources\\calibre-bold.ttf");

            //if (File.Exists(fontPath))
            //{
            //    privateFontCollection.AddFontFile(fontPath);

            //    stopFont = new Font(privateFontCollection.Families.First(), 110, FontStyle.Regular, GraphicsUnit.Pixel);
            //    dateTimeFont = new Font(privateFontCollection.Families.First(), 48, FontStyle.Regular, GraphicsUnit.Pixel);
            //    underLogoFont = new Font(privateFontCollection.Families.First(), 28, FontStyle.Regular, GraphicsUnit.Pixel);
            //}

            groupImage = Http.Properties.Resources.group;
            //companyLogo = Http.Properties.Resources.insupport_logo3;
            //companyLogo = Http.Properties.Resources.citygross_logo;
            companyLogo = Http.Properties.Resources.Maxi_Östersund1;

        }

        internal override Image CreateFrame()
        {
            using (var graphics = Graphics.FromImage(image))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                ThresholdMode thresholdMode = ConfigFile.Instance.CurrentCount >= ConfigFile.Instance.MaxOccupancyThreshold ? ThresholdMode.AboveThreshold : ThresholdMode.UnderThreshold;
                if (thresholdMode == ThresholdMode.UnderThreshold && ConfigFile.Instance.CurrentCount >= ConfigFile.Instance.CloseOccupancyThreshold)
                    thresholdMode = ThresholdMode.CloseThreshold;

                if (ConfigFile.Instance.EmergencyStop)
                    thresholdMode = ThresholdMode.AboveThreshold;


                var imgRect = new Rectangle(0, 0, image.Width, image.Height);

                Color gradientColor1 = Color.White, gradientColor2 = Color.Black;

                Color smallTextColor = Color.White;
                string bigText = null;

                switch (thresholdMode)
                {
                    case ThresholdMode.AboveThreshold:
                        gradientColor1 = redGradientColor1;
                        gradientColor2 = redGradientColor2;

                        //smallTextColor = Color.FromArgb(255, 255, 255);

                        bigText = "Vänligen vänta,\r\nvi tänker på din säkerhet";
                        break;
                    case ThresholdMode.CloseThreshold:
                        gradientColor1 = yellowGradientColor1;
                        gradientColor2 = yellowGradientColor2;

                        //smallTextColor = Color.FromArgb(0, 0, 0);

                        bigText = "Vänligen tänk på avståndet, det börjar bli trångt";

                        break;
                    case ThresholdMode.UnderThreshold:
                        gradientColor1 = greenGradientColor1;
                        gradientColor2 = greenGradientColor2;

                        //smallTextColor = Color.FromArgb(0, 0, 0);

                        bigText = "Välkommen in,\r\ndet är tryggt att handla";
                        break;
                }

                graphics.FillRectangle(new LinearGradientBrush(imgRect, gradientColor1, gradientColor2, 45f), imgRect);

                var size = 128;
                var tLightGreenRect = new Rectangle(image.Width - (int)(size * 1.5), (int)(size * 0.5), size, size);
                var tLightYellowRect = new Rectangle(image.Width - (int)(size * 1.5), (int)(size * 2), size, size);
                var tLightRedRect = new Rectangle(image.Width - (int)(size * 1.5), (int)(size * 3.5), size, size);

                var tLightThickness = 5;
                switch(thresholdMode)
                {
                    case ThresholdMode.AboveThreshold:
                        graphics.FillEllipse(Brushes.White, new Rectangle(tLightRedRect.X - tLightThickness, tLightRedRect.Y - tLightThickness, 
                            tLightRedRect.Width + (tLightThickness*2), tLightRedRect.Height + (tLightThickness * 2)));
                        break;
                    case ThresholdMode.CloseThreshold:
                        graphics.FillEllipse(Brushes.White, new Rectangle(tLightYellowRect.X - tLightThickness, tLightYellowRect.Y - tLightThickness,
                            tLightYellowRect.Width + (tLightThickness * 2), tLightYellowRect.Height + (tLightThickness * 2)));
                        break;
                    case ThresholdMode.UnderThreshold:
                        graphics.FillEllipse(Brushes.White, new Rectangle(tLightGreenRect.X - tLightThickness, tLightGreenRect.Y - tLightThickness, 
                            tLightGreenRect.Width + (tLightThickness * 2), tLightGreenRect.Height + (tLightThickness * 2)));
                        break;
                }

                lastThresholdMode = thresholdMode;


                graphics.FillEllipse(new LinearGradientBrush(tLightGreenRect, greenGradientColor1, greenGradientColor2, 45f), tLightGreenRect);
                graphics.FillEllipse(new LinearGradientBrush(tLightYellowRect, yellowGradientColor1, yellowGradientColor2, 45f), tLightYellowRect);
                graphics.FillEllipse(new LinearGradientBrush(tLightRedRect, redGradientColor1, redGradientColor2, 45f), tLightRedRect);

                float comLogoScale = 0.25f;
                //float comLogoScale = 0.3f;
                graphics.DrawImage(companyLogo, new Rectangle(20, 20, (int)(companyLogo.Width * comLogoScale), (int)(companyLogo.Height * comLogoScale)));

                var smallTextBrush = new SolidBrush(smallTextColor);

                var dateTimeText = DateTime.Now.ToString();
                var dateTimeTextSize = graphics.MeasureString(dateTimeText, dateTimeFont);
                graphics.DrawString(dateTimeText, dateTimeFont, smallTextBrush, 5, image.Height - dateTimeTextSize.Height);


                // Current occ
                if(!ConfigFile.Instance.HideMaxOccText)
                {
                    //var countText = string.Format("Antal personer i butiken: {0} (Max: {1})", ConfigFile.Instance.CurrentCount, ConfigFile.Instance.MaxOccupancyThreshold);
                    var countText = string.Format("Antal personer i butiken: {0}", ConfigFile.Instance.CurrentCount);
                    //var countText = string.Format("Max antal besökare i butiken: {0}", ConfigFile.Instance.MaxOccupancyThreshold);
                    var countTextSize = graphics.MeasureString(countText, dateTimeFont);
                    graphics.DrawString(countText, dateTimeFont, smallTextBrush, image.Width - countTextSize.Width - 10, image.Height - countTextSize.Height);
                }


                StringFormat stopTextStringFormat = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                //var underLogoText = "För din trygghet håller vi koll på antalet kunder som vistas i butiken samtidigt";
                //var underLogoTextSize = graphics.MeasureString(underLogoText, dateTimeFont);
                //var underLogoTextRect = new Rectangle(20, 20 + (int)(companyLogo.Height * comLogoScale), (int)(companyLogo.Width * comLogoScale), (int)(companyLogo.Height * comLogoScale * 0.8));
                //graphics.DrawString(underLogoText, underLogoFont, smallTextBrush, underLogoTextRect, stopTextStringFormat);

                var groupImageRect = new Rectangle(image.Width / 2 - 256 / 2, image.Height / 3 - 256 / 2, 256, 256);
                graphics.DrawImage(groupImage, groupImageRect);
                graphics.DrawEllipse(
                    new Pen(Brushes.White, 18),
                    new Rectangle(groupImageRect.X - 100, groupImageRect.Y - 100, groupImageRect.Width + 200, groupImageRect.Height + 200)
                    );

                var stopTextRect = new Rectangle(image.Width / 10, image.Height / 2, image.Width / 10 * 8, image.Height / 2);
                graphics.DrawString(bigText, stopFont, Brushes.White, stopTextRect, stopTextStringFormat);

            }

            return image;
        }

        private enum ThresholdMode
        {
            UnderThreshold,
            CloseThreshold,
            AboveThreshold
        }
    }
}
