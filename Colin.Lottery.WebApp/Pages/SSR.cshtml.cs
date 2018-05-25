using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Colin.Lottery.Utils;
using System;

namespace Colin.Lottery.WebApp.Pages
{
    public class SSRModel : PageModel
    {
        public object SSRS { get; private set; }

        public async Task OnGetAsync()
        {
            SSRS = await HttpUtil.GetStringAsync("https://yzzz.ml/freessr/");
        }
    }
}
