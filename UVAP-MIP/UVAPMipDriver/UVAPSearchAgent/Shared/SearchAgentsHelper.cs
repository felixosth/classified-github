using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform;
using VideoOS.Platform.Data;
using VideoOS.Platform.Search.FilterValues;

namespace UVAPSearchAgent.Shared
{
    // Helper class for both search agents.
    public static class SearchAgentsHelper
    {
        // Check if the x and y is within a selected area of the regionselected mask
        public static bool IsWithinMask(RegionSelection regionSelection, Size imageResolution, float posX, float posY)
        {
            int blockWidth = imageResolution.Width / regionSelection.Size.Width;
            int blockHeight = imageResolution.Height / regionSelection.Size.Height;

            for (int i = 0; i < regionSelection.Mask.Length; i++)
            {
                if (regionSelection.Mask[i] == '0')
                {
                    int x = (i % regionSelection.Size.Width);
                    int y = (i / regionSelection.Size.Height);

                    int fromX = blockWidth * x;
                    int toX = fromX + blockWidth;

                    int fromY = blockHeight * y;
                    int toY = fromY + blockHeight;

                    if (fromX <= posX && posX <= toX && fromY <= posY && posY <= toY)
                        return true;
                }
            }

            return false;
        }

        // Get the nearest video frame from the chosen DateTime and return the pixel-size of the frame
        public static Size GetResolution(Item item, DateTime from)
        {
            JPEGVideoSource source = new JPEGVideoSource(item);
            source.Init();
            var frame = source.GetNearest(from) as JPEGData;
            source.Close();
            return new Size() { Width = frame.Width, Height = frame.Height };
        }

        // Make a guid based on the parameters
        public static Guid MakeId(string title, Item item, DateTime triggerTime, DateTime endTime, string customParam)
        {
            string stringId = $"{title}:{item.FQID.ObjectId}:{triggerTime:s}:{endTime:s}:{customParam}";
            // generate id using MD5 hash function
            var provider = new MD5CryptoServiceProvider();
            byte[] input = Encoding.Default.GetBytes(stringId);
            byte[] hashBytes = provider.ComputeHash(input);
            return new Guid(hashBytes);
        }
    }
}
