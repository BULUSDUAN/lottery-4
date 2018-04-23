using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Colin.Lottery.WebApp.Hubs
{
    public class PK10Hub : BaseHub<PK10Hub>
    {
        public async Task Test()
        {
            await Clients.Caller.SendAsync("ShowServerTime", DateTime.Now);
        }
    }
}