namespace QRCodeGenerator.Web.Pages
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class ContactPage : PageModel
    {
        public ContactPage()
        {
        }

        public IEnumerable<string> Data { get; private set; }

        public async Task OnGetAsync()
        {
            Data = new List<string>();
        }

        
    }

}