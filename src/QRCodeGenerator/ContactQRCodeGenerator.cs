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
    public static class ContactQRCodeGenerator
    {
        [FunctionName("ContactQRCodeGenerator")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Started processing ContactQRCodeGenerator request.");
            string firstName = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "firstName", StringComparison.CurrentCultureIgnoreCase) == 0).Value;
            string lastName = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "lastName", StringComparison.CurrentCultureIgnoreCase) == 0).Value;
            string workPhone = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "workPhone", StringComparison.CurrentCultureIgnoreCase) == 0).Value;
            string mobilePhone = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "mobilePhone", StringComparison.CurrentCultureIgnoreCase) == 0).Value;
            string email = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "email", StringComparison.CurrentCultureIgnoreCase) == 0).Value;



            string howToMessage = $"To create a QR code (that will create a contact) all the parameters must be provided e.g. {req.RequestUri.Scheme}://{req.RequestUri.Authority}{req.RequestUri.AbsolutePath}?code=[TalkToDevelopmentForCode]&firstname=dan&lastname=hibbert&workphone=+442074563922&mobilephone=+447825057132&email=dan.hibbert@gmail.com";

            if (string.IsNullOrEmpty(firstName))
            {
                log.Info("No firstName parameter provided");
                return req.CreateResponse(HttpStatusCode.BadRequest, $"No firstName parameter provided.  {howToMessage}");
            }
            if (string.IsNullOrEmpty(lastName))
            {
                log.Info("No lastName parameter provided");
                return req.CreateResponse(HttpStatusCode.BadRequest, $"No lastName parameter provided.  {howToMessage}");
            }
            if (string.IsNullOrEmpty(workPhone))
            {
                log.Info("No workPhone parameter provided");
                return req.CreateResponse(HttpStatusCode.BadRequest, $"No workPhone parameter provided.  {howToMessage}");
            }
            if (string.IsNullOrEmpty(mobilePhone))
            {
                log.Info("No mobilePhone parameter provided");
                return req.CreateResponse(HttpStatusCode.BadRequest, $"No mobilePhone parameter provided.  {howToMessage}");
            }
            if (string.IsNullOrEmpty(email))
            {
                log.Info("No email parameter provided");
                return req.CreateResponse(HttpStatusCode.BadRequest, $"No email parameter provided.  {howToMessage}");
            }
            try
            {
                PayloadGenerator.ContactData payloadGenerator = new PayloadGenerator.ContactData(PayloadGenerator.ContactData.ContactOutputType.VCard4, firstname: firstName, lastname: lastName, workPhone: workPhone, mobilePhone: mobilePhone, email: email);
                string payload = payloadGenerator.ToString();
                HttpResponseMessage result = QRCodeHelper.GenerateImageResponse(req, payload);
                log.Info("Successfully processed ContactQRCodeGenerator request.");
                return result;
            }
            catch (Exception ex)
            {
                log.Error($"Failed to process ContactQRCodeGenerator request. {ex.Message}", ex);
                return req.CreateResponse(HttpStatusCode.BadRequest, $"Error creating Website QR code.  {ex.Message}");
            }
        }
    }
}
