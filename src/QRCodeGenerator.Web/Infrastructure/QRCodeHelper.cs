namespace QRCodeGenerator.Web.Infrastructure
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    using QRCoder;

    public class QRCodeHelper : IQRCodeHelper
    {
        public byte[] GenerateImageResponse(string payload)
        {
            using (QRCodeGenerator generator = new QRCoder.QRCodeGenerator())
            {
                QRCoder.QRCodeGenerator.ECCLevel eccLevel = QRCoder.QRCodeGenerator.ECCLevel.H;
                using (QRCodeData data = generator.CreateQrCode(payload, eccLevel))
                {
                    using (QRCode code = new QRCode(data))
                    {
                        int pixelsPerModule = 20;
                        string foregroundColor = "#000000";
                        string backgroundColor = "#FFFFFF";
                        using (Bitmap bitmap = code.GetGraphic(pixelsPerModule, foregroundColor, backgroundColor))
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                bitmap.Save(stream, ImageFormat.Png);
                                return stream.ToArray();
                            }
                        }
                    }
                }
            }
        }
    }
}