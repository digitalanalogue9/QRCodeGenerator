namespace QRCodeGenerator.Web.Pages.Calendar
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    using QRCodeGenerator.Web.Features;
    using QRCodeGenerator.Web.Infrastructure;
    using QRCodeGenerator.Web.Models;

    using QRCoder;

    [ValidateModel]
    public class Create : PageModel
    {
        private readonly IMediator _mediator;

        public Create(IMediator mediator) => _mediator = mediator;

        [BindProperty]
        public Command Data { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return new ValidationFailedResult(ModelState);
            //}
            var image = await _mediator.Send(Data);
            //return File(image, "image/png", "qrcode.png");
            return File(image, "application/octet-stream", "qrcode.png");
        }

        public class Command : IRequest<byte[]>
        {
            [Required]
            [StringLength(50, MinimumLength = 3)]
            public string Subject { get; set; }
            [Required]
            [StringLength(50, MinimumLength = 3)]
            public string Description { get; set; }
            public string Location { get; set; }
            [Required]
            [DataType(DataType.Date)]
            [DisplayName("Start Date")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
            public DateTime? StartDate { get; set; }
            public string StartTime { get; set; }
            [Required]
            [DataType(DataType.Date)]
            [DisplayName("End Date")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
            public DateTime? EndDate { get; set; }
            public string EndTime { get; set; }
            public bool AllDayEvent { get; set; }

        }

        //public class MappingProfile : Profile
        //{
        //    public MappingProfile() =>
        //        CreateMap<Command, Course>(MemberList.Source)
        //            .ForSourceMember(c => c.Number, opt => opt.Ignore());
        //}


        public class Handler : IRequestHandler<Command, byte[]>
        {
            private readonly IQRCodeHelper _qrcodeHelper;
            public Handler(IQRCodeHelper qrcodeHelper) => this._qrcodeHelper = qrcodeHelper;

            public async Task<byte[]> Handle(Command message, CancellationToken token)
            {
                char[] separator = new char[] { ':' };
                if (!message.StartDate.HasValue)
                {
                    message.StartDate = DateTime.Now; ;
                }
                if (!message.EndDate.HasValue)
                {
                    message.EndDate = DateTime.Now; ;
                }
                if (!string.IsNullOrEmpty(message.StartTime))
                {
                    string[] time = message.StartTime.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    message.StartDate = new DateTime(
                        message.StartDate.Value.Year,
                        message.StartDate.Value.Month,
                        message.StartDate.Value.Day,
                        int.Parse(time[0]),
                        int.Parse(time[1]),
                        0);
                }
                if (!string.IsNullOrEmpty(message.EndTime))
                {
                    string[] time = message.EndTime.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    message.EndDate = new DateTime(
                        message.EndDate.Value.Year,
                        message.EndDate.Value.Month,
                        message.EndDate.Value.Day,
                        int.Parse(time[0]),
                        int.Parse(time[1]),
                        0);
                }

                PayloadGenerator.CalendarEvent payloadGenerator = new PayloadGenerator.CalendarEvent(message.Subject, message.Description, message.Location, message.StartDate.Value, message.EndDate.Value, message.AllDayEvent);
                string payload = payloadGenerator.ToString();
                return this._qrcodeHelper.GenerateImageResponse(payload);
            }
        }
    }
}