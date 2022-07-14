using System;
using System.Drawing.Imaging;

namespace QRCodeGenerator.Shell
{
    public class OptionSetter
    {
        public QRCoder.QRCodeGenerator.ECCLevel GetECCLevel(string value)
        {
            QRCoder.QRCodeGenerator.ECCLevel level;

            Enum.TryParse(value, out level);

            return level;
        }

        public ImageFormat GetImageFormat(string value)
        {
            switch (value.ToLower())
            {
                case "jpg":
                    return ImageFormat.Jpeg;
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "gif":
                    return ImageFormat.Gif;
                case "bmp":
                    return ImageFormat.Bmp;
                case "tiff":
                    return ImageFormat.Tiff;
                case "png":
                default:
                    return ImageFormat.Png;
            }
        }
    }
}