using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using QRCoder;

namespace QRCodeGenerator
{
    public static class WebQRCodeGenerator
    {
        [FunctionName("WebQRCodeGenerator")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
           log.Info("Started processing WebQRCodeGenerator request.");
            string url = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "url", StringComparison.CurrentCultureIgnoreCase) == 0).Value;

            string howToMessage = $"To create a QR code (that will open the mobile browser) all the parameters must be provided e.g. {req.RequestUri.Scheme}://{req.RequestUri.Authority}{req.RequestUri.AbsolutePath}?code=[TalkToDevelopmentForCode]&url=https://www.google.com";

            if (string.IsNullOrEmpty(url))
            {
                log.Info("No url parameter provided");
                return req.CreateResponse(HttpStatusCode.BadRequest, $"No url parameter provided.  {howToMessage}");
            }
            try
            {
                PayloadGenerator.Url payloadGenerator = new PayloadGenerator.Url(url);
                string payload = payloadGenerator.ToString();
                HttpResponseMessage result = QRCodeHelper.GenerateImageResponse(req, payload);
                log.Info("Successfully processed WebQRCodeGenerator request.");
                return result;
            }
            catch (Exception ex)
            {
                log.Error($"Failed to process WebQRCodeGenerator request. {ex.Message}", ex);
                return req.CreateResponse(HttpStatusCode.BadRequest, $"Error creating Website QR code.  {ex.Message}" );
            }
        }
    }
}
