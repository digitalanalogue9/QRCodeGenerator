namespace QRCodeGenerator.Web.Pages.Email
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading;
    using System.Threading.Tasks;

    using MediatR;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    using QRCodeGenerator.Web.Features;
    using QRCodeGenerator.Web.Infrastructure;
    using QRCodeGenerator.Web.Models;

    using QRCoder;


    public class Create : PageModel
    {
        private readonly IMediator _mediator;

        public Create(IMediator mediator) => _mediator = mediator;

        [BindProperty]
        public Command Data { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return new ValidationFailedResult(ModelState);
            }
            var image = await _mediator.Send(Data);
            //return File(image, "image/png", "qrcode.png");
            return File(image, "application/octet-stream", "qrcode.png");


        }

        public class Command : IRequest<byte[]>
        {
            [Required]
            [EmailAddress]
            public string EmailTo { get; set; }
            public string EventNumber { get; set; }
            [Required]
            [StringLength(50, MinimumLength = 3)]
            public string EventName { get; set; }
            [Required]
            [StringLength(50, MinimumLength = 3)]
            public string Message { get; set; }
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
                string subject = string.IsNullOrEmpty(message.EventNumber) ? message.EventName : $"{message.EventNumber} - {message.EventName}";
                PayloadGenerator.Mail payloadGenerator = new PayloadGenerator.Mail(message.EmailTo, subject, message.Message);
                string payload = payloadGenerator.ToString();
                return this._qrcodeHelper.GenerateImageResponse(payload);
            }
        }
    }




}