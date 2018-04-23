using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

using Colin.Lottery.WebApp.Hubs;

namespace Colin.Lottery.WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHubContext<PK10Hub> _hubContext;
        public IndexModel(IHubContext<PK10Hub> hubContext)
        {
            _hubContext = hubContext;
        }
        public void OnGet()
        {
        }

        public void OnPost()
        {
            _hubContext.Clients.All.SendAsync("ShowServerTime", DateTime.Now);
        }
    }
}
