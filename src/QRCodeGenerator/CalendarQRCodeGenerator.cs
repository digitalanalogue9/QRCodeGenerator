using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using QRCoder;

namespace QRCodeGenerator
{
    public static class CalendarQRCodeGenerator
    {

        [FunctionName("CalendarQRCodeGenerator")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get",  Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Started processing CalendarQRCodeGenerator request.");

            string subject = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "subject", StringComparison.CurrentCultureIgnoreCase) == 0).Value;
            string body = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "body", StringComparison.CurrentCultureIgnoreCase) == 0).Value;
            string location = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "location", StringComparison.CurrentCultureIgnoreCase) == 0).Value;
            string startDateString = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "startDate", StringComparison.CurrentCultureIgnoreCase) == 0).Value;
            string endDateString = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "endDate", StringComparison.CurrentCultureIgnoreCase) == 0).Value;
            string alldayeventString = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "alldayevent", StringComparison.CurrentCultureIgnoreCase) == 0).Value;

            string howToMessage = $"To create a QR code (that will create a calendar event) all the parameters must be provided e.g. {req.RequestUri.Scheme}://{req.RequestUri.Authority}{req.RequestUri.AbsolutePath}?code=[TalkToDevelopmentForCode]&subject=Meet Now&description=This is a meeting request&location=Silks&startDate=2018-12-13 15:30&endDate=2018-12-13 16:30&alldayevent=false";

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
            if (string.IsNullOrEmpty(location))
            {
                log.Info("No location parameter provided");
                return req.CreateResponse(HttpStatusCode.BadRequest, $"No location parameter provided.  {howToMessage}");

            }
            if (string.IsNullOrEmpty(startDateString))
            {
                log.Info("No startDate parameter provided");
                return req.CreateResponse(HttpStatusCode.BadRequest, $"No startDate parameter provided.  {howToMessage}");
            }
            if (!DateTime.TryParseExact(startDateString, "yyyy-MM-dd HH:mm", new CultureInfo("en-US"),
                DateTimeStyles.None, out var startDate))
            {
                log.Info("Start date format was incorrect");
                return req.CreateResponse(HttpStatusCode.BadRequest, $"The startDate parameter provided must be in the format YYYY-MM-DD HH:MM.  {howToMessage}");
            }
            if (string.IsNullOrEmpty(endDateString))
            {
                log.Info("No endDate parameter provided");
                return req.CreateResponse(HttpStatusCode.BadRequest, $"No endDate parameter provided.  {howToMessage}");
            }
            if (!DateTime.TryParseExact(endDateString, "yyyy-MM-dd HH:mm", new CultureInfo("en-US"),
                DateTimeStyles.None, out var endDate))
            {
                log.Info("End date format was incorrect");
                return req.CreateResponse(HttpStatusCode.BadRequest, $"The endDate parameter provided must be in the format YYYY-MM-DD HH:MM.  {howToMessage}");
            }
            if (string.IsNullOrEmpty(alldayeventString))
            {
                log.Info("No alldayevent parameter provided");
                return req.CreateResponse(HttpStatusCode.BadRequest, $"No alldayevent parameter provided.  {howToMessage}");
            }
            if (!Boolean.TryParse(alldayeventString, out bool allDayEvent))
            {
                log.Info("alldayevent format was incorrect");
                return req.CreateResponse(HttpStatusCode.BadRequest, $"The alldayevent parameter provided must be in the format true or false.  {howToMessage}");
            }
            try
            {
                PayloadGenerator.CalendarEvent payloadGenerator = new PayloadGenerator.CalendarEvent(subject, body, location, startDate, endDate, allDayEvent);
                string payload = payloadGenerator.ToString();
                HttpResponseMessage result = QRCodeHelper.GenerateImageResponse(req, payload);
                log.Info("Successfully processed CalendarQRCodeGenerator request.");
                return result;
            }
            catch (Exception ex)
            {
                log.Error($"Failed to process CalendarQRCodeGenerator request. {ex.Message}", ex);
                return req.CreateResponse(HttpStatusCode.BadRequest, $"Error creating Calendar QR code.  {ex.Message}");
            }
        }
    }
}
