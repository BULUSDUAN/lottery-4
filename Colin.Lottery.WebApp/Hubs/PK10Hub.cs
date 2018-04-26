using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

using Colin.Lottery.Analyzers;
using Colin.Lottery.Models;

namespace Colin.Lottery.WebApp.Hubs
{
    public class PK10Hub : BaseHub<PK10Hub>
    {
        public async Task GetForcastData(int rule = 1, bool startWhenBreakGua = false)
        {
            var plans = await JinMaAnalyzer.Instance.GetForcastData(LotteryType.PK10, rule);
            JinMaAnalyzer.Instance.CalcuteScore(ref plans, startWhenBreakGua);
            await Clients.Caller.SendAsync("ShowPlans", plans);
        }
    }
}