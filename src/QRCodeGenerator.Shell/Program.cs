using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mail;
using System.Text;
using QRCoder;

namespace QRCodeGenerator.Shell
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var friendlyName = AppDomain.CurrentDomain.FriendlyName;
            var newLine = Environment.NewLine;
            var setter = new OptionSetter();

            String outputFileName = null;

            QRCoder.QRCodeGenerator.ECCLevel eccLevel = QRCoder.QRCodeGenerator.ECCLevel.H;
            int pixelsPerModule = 20;
            string foregroundColor = "#000000";
            string backgroundColor = "#FFFFFF";
            outputFileName = Path.ChangeExtension(Path.GetTempFileName(), "png");
            string eventId = "1";
            string eventName = "Standard training message";
           

           
            try
            {


                PayloadGenerator.Mail mailGenerator = new PayloadGenerator.Mail("dan.hibbert@gmail.com", $"{eventId} - {eventName}", "I attended");
                string mailPayload = mailGenerator.ToString();

                

                using (var generator = new QRCoder.QRCodeGenerator())
                {
                    using (var data = generator.CreateQrCode(mailPayload, eccLevel))
                    {
                        using (var code = new QRCode(data))
                        {
                            using (var bitmap = code.GetGraphic(pixelsPerModule, foregroundColor, backgroundColor, true))
                            {
                                bitmap.Save(outputFileName, ImageFormat.Png);
                            }
                        }

                    }
                }

                Console.WriteLine($"File is available at {outputFileName}");
                Console.WriteLine("Press return to close");
                Console.ReadLine();
            }
            catch (Exception oe)
            {
                Console.Error.WriteLine(
                    $"{friendlyName}:{newLine}{oe.GetType().FullName}{newLine}{oe.Message}{newLine}{oe.StackTrace}{newLine}Try '{friendlyName} --help' for more information");
                Environment.Exit(-1);
            }
        }

        private static void GenerateQRCode(string payloadString, QRCoder.QRCodeGenerator.ECCLevel eccLevel, string outputFileName, SupportedImageFormat imgFormat, int pixelsPerModule, string foreground, string background)
        {
            using (var generator = new QRCoder.QRCodeGenerator())
            {
                using (var data = generator.CreateQrCode(payloadString, eccLevel))
                {
                    switch (imgFormat)
                    {
                        case SupportedImageFormat.Png:
                        case SupportedImageFormat.Jpg:
                        case SupportedImageFormat.Gif:
                        case SupportedImageFormat.Bmp:
                        case SupportedImageFormat.Tiff:
                            using (var code = new QRCode(data))
                            {
                                using (var bitmap = code.GetGraphic(pixelsPerModule, foreground, background, true))
                                {
                                    var actualFormat = new OptionSetter().GetImageFormat(imgFormat.ToString());
                                    bitmap.Save(outputFileName, actualFormat);
                                }
                            }
                            break;
                        case SupportedImageFormat.Svg:
                            using (var code = new SvgQRCode(data))
                            {
                                var test = code.GetGraphic(pixelsPerModule, foreground, background, true);
                                using (var f = File.CreateText(outputFileName))
                                {
                                    f.Write(test);
                                    f.Flush();
                                }
                            }
                            break;
                        //case SupportedImageFormat.Xaml:
                        //    using (var code = new QRCoder. XamlQRCode(data))
                        //    {
                        //        var test = XamlWriter.Save(code.GetGraphic(pixelsPerModule, foreground, background, true));
                        //        using (var f = File.CreateText(outputFileName))
                        //        {
                        //            f.Write(test);
                        //            f.Flush();
                        //        }
                        //    }
                        //    break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(imgFormat), imgFormat, null);
                    }

                }
            }
        }

        private static string GetTextFromFile(FileInfo fileInfo)
        {
            var buffer = new byte[fileInfo.Length];

            using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open))
            {
                fileStream.Read(buffer, 0, buffer.Length);
            }

            return Encoding.UTF8.GetString(buffer);
        }

        private static string GetTextFromStream(Stream stream)
        {
            var buffer = new byte[256];
            var bytesRead = 0;

            using (var memoryStream = new MemoryStream())
            {
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }

                var text = Encoding.UTF8.GetString(memoryStream.ToArray());

                Console.WriteLine($"text retrieved from input stream: {text}");

                return text;
            }
        }

    }
}

