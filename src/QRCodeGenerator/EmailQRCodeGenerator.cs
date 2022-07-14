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
    public static class EmailQRCodeGenerator
    {

        [FunctionName("EmailQRCodeGenerator")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get",  Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Started processing EmailQRCodeGenerator request.");

            string to = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "to", StringComparison.CurrentCultureIgnoreCase) == 0).Value;
            string subject = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "subject", StringComparison.CurrentCultureIgnoreCase) == 0).Value;
            string body = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "body", StringComparison.CurrentCultureIgnoreCase) == 0).Value;

            string howToMessage = $"To create a QR code (that will send an email with a subject and a message) all the parameters must be provided e.g. {req.RequestUri.Scheme}://{req.RequestUri.Authority}{req.RequestUri.AbsolutePath}?code=[TalkToDevelopmentForCode]&to=dan.hibbert@gmail.com&subject=Training&message=I Attended.";

            if (string.IsNullOrEmpty(to))
            {
                log.Info("No to parameter provided");
                return req.CreateResponse(HttpStatusCode.BadRequest, $"No to parameter provided.  {howToMessage}");

            }
            if (string.IsNullOrEmpty(subject))
            {
                log.Info("No subject parameter provided");
                return req.CreateResponse(HttpStatusCode.BadRequest, $"No subject parameter provided.  {howToMessage}");

            }

            if (string.IsNullOrEmpty(body))
            {
                log.Info("No body parameter provided");
                return req.CreateResponse(HttpStatusCode.BadRequest, $"No body parameter provided.  {howToMessage}");

            }
            try
            {
                PayloadGenerator.Mail payloadGenerator = new PayloadGenerator.Mail(to, subject, body);
                string payload = payloadGenerator.ToString();
                HttpResponseMessage result = QRCodeHelper.GenerateImageResponse(req, payload);
                log.Info("Successfully processed EmailQRCodeGenerator request.");
                return result;

            }
            catch (Exception ex)
            {
                log.Error($"Failed to process EmailQRCodeGenerator request. {ex.Message}", ex);
                return req.CreateResponse(HttpStatusCode.BadRequest, $"Error creating Email QR code.  {ex.Message}");
            }
        }
    }
}
