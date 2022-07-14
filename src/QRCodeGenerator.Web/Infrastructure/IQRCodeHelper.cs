namespace QRCodeGenerator.Web.Infrastructure
{
    public interface IQRCodeHelper
    {
        byte[] GenerateImageResponse(string payload);
    }
}