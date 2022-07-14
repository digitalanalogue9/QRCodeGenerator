using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QRCodeGenerator.Web.Models;

namespace QRCodeGenerator.Web.Pages.Web
{
    using System.ComponentModel.DataAnnotations;

    using MediatR;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using QRCodeGenerator.Web.Infrastructure;

    using QRCoder;
    using System.Threading;

    using QRCodeGenerator.Web.Features;

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
            return File(image, "image/png", "qrcode.png");
            //return File(image, "application/octet-stream", "qrcode.png");
        }

       public class Command : IRequest<byte[]>
        {
            [Required]
            [Url]
            public string Url {get;set;}

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
                PayloadGenerator.Url payloadGenerator = new PayloadGenerator.Url(message.Url);
                string payload = payloadGenerator.ToString();
                return this._qrcodeHelper.GenerateImageResponse(payload);
            }
        }
    }

}
