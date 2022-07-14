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
    public static class PhoneQRCodeGenerator
    {

        [FunctionName("PhoneQRCodeGenerator")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get",  Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Started processing PhoneQRCodeGenerator request.");
            string phonenumber = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "phonenumber", StringComparison.CurrentCultureIgnoreCase) == 0).Value;
            
            
            string howToMessage = $"To create a QR code (that will call the given number) all the parameters must be provided e.g. {req.RequestUri.Scheme}://{req.RequestUri.Authority}{req.RequestUri.AbsolutePath}?code=[TalkToDevelopmentForCode]&phonenumber=02074563922";

            if (string.IsNullOrEmpty(phonenumber))
            {
                log.Info("No phonenumber parameter provided");
                return req.CreateResponse(HttpStatusCode.BadRequest, $"No phonenumber parameter provided.  {howToMessage}" );

            }
            try
            {

                PayloadGenerator.PhoneNumber payloadGenerator = new PayloadGenerator.PhoneNumber(phonenumber);
                string payload = payloadGenerator.ToString();
                HttpResponseMessage result = QRCodeHelper.GenerateImageResponse(req, payload);
                log.Info("Successfully processed PhoneQRCodeGenerator request.");
                return result;
            }
            catch (Exception ex)
            {
                log.Error($"Failed to process PhoneQRCodeGenerator request. {ex.Message}", ex);
                return req.CreateResponse(HttpStatusCode.BadRequest, $"Error creating Phone QR code.  {ex.Message}");
            }
        }
    }
}
