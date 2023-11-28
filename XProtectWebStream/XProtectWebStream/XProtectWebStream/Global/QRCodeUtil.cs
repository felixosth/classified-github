using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProtectWebStream.Global
{
    internal static class QRCodeUtil
    {
        private static readonly QRCodeGenerator qrGenerator = new QRCodeGenerator();
        
        internal static byte[] GenerateQRImage(string text)
        {
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(4, Color.LightGray, Color.Black, drawQuietZones: true);

            byte[] data;
            using (var ms = new MemoryStream())
            {
                qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                data = ms.ToArray();
            }
            qrCodeImage.Dispose();
            return data;
        }
    }
}
