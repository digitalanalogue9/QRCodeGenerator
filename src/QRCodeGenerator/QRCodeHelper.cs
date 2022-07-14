using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using QRCoder;

namespace QRCodeGenerator
{
    public class QRCodeHelper
    {
        public static HttpResponseMessage GenerateImageResponse(HttpRequestMessage req, string payload)
        {
            HttpResponseMessage result = null;
            using (var generator = new QRCoder.QRCodeGenerator())
            {
                QRCoder.QRCodeGenerator.ECCLevel eccLevel = QRCoder.QRCodeGenerator.ECCLevel.H;
                using (var data = generator.CreateQrCode(payload, eccLevel))
                {
                    using (var code = new QRCode(data))
                    {
                        int pixelsPerModule = 20;
                        string foregroundColor = "#000000";
                        string backgroundColor = "#FFFFFF";
                        using (var bitmap = code.GetGraphic(pixelsPerModule, foregroundColor, backgroundColor))
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                bitmap.Save(stream, ImageFormat.Png);
                                result = new HttpResponseMessage(HttpStatusCode.OK)
                                {
                                    Content = new ByteArrayContent(stream.ToArray())
                                };
                                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                                { FileName = "qrcode.png" };
                                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            }

                        }
                    }
                }
            }
            return result;
        }
    }
}
