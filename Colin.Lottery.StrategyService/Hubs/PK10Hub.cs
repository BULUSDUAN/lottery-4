
using System;
using System.Threading.Tasks;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Colin.Lottery.StrategyService
{
    [HubName("PK10Service")]
    public class PK10Hub : BaseHub
    {
        public async Task Broadcast(object forcastData)
        {
            await Clients.All.ShowForcast(forcastData);
        }

        public async override Task OnConnected()
        {
            await Groups.Add(Context.ConnectionId, "PK10");

            await base.OnConnected();
        }

        public async override Task OnDisconnected(bool stopCalled)
        {
            await Groups.Remove(Context.ConnectionId, "PK10");

            await base.OnDisconnected(stopCalled);
        }
    }
}
