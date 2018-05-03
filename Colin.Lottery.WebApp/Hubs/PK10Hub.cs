using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

using Colin.Lottery.Analyzers;
using Colin.Lottery.Models;
using Colin.Lottery.Utils;

namespace Colin.Lottery.WebApp.Hubs
{
    public class PK10Hub : BaseHub<PK10Hub>
    {
        public async Task GetForcastData(int rule = 1, bool startWhenBreakGua = false)
        {
            var plans = await JinMaAnalyzer.Instance.GetForcastData(LotteryType.PK10, rule);
            if (plans.Plan1 == null || plans.Plan2 == null)
            {
                await Clients.Caller.SendAsync("NoResult");
                LogUtil.Fatal("目标网站扫水接口异常，请尽快检查恢复");
            }
            else
            {
                JinMaAnalyzer.Instance.CalcuteScore(ref plans, startWhenBreakGua);
                await Clients.Caller.SendAsync("ShowPlans", plans);
            }


            await Groups.AddAsync(Context.ConnectionId, ((PK10Rule)rule).ToString());
        }

        /// <summary>
        /// 获取PK10所有玩法最新期预测数据
        /// </summary>
        /// <returns>The all new forcast.</returns>
        public async Task GetAllNewForcast(bool startWhenBreakGua = false)
        {
            var forcast = new List<IForcastModel>();
            int error = 0;
            error += await GetNewForcast(forcast, PK10Rule.Champion, startWhenBreakGua);
            error += await GetNewForcast(forcast, PK10Rule.Second, startWhenBreakGua);
            error += await GetNewForcast(forcast, PK10Rule.Third, startWhenBreakGua);
            error += await GetNewForcast(forcast, PK10Rule.Fourth, startWhenBreakGua);
            error += await GetNewForcast(forcast, PK10Rule.BigOrSmall, startWhenBreakGua);
            error += await GetNewForcast(forcast, PK10Rule.OddOrEven, startWhenBreakGua);
            error += await GetNewForcast(forcast, PK10Rule.DragonOrTiger, startWhenBreakGua);
            error += await GetNewForcast(forcast, PK10Rule.Sum, startWhenBreakGua);

            if (error > 0)
            {
                await Clients.Caller.SendAsync("NoResult", "allRules");
                LogUtil.Fatal("目标网站扫水接口异常，请尽快检查恢复");
            }

            await Clients.Caller.SendAsync("ShowPlans", forcast);

            await Groups.AddAsync(Context.ConnectionId, "AllRules");
        }

        async Task<int> GetNewForcast(List<IForcastModel> newForcast, PK10Rule rule, bool startWhenBreakGua)
        {
            var plans = await JinMaAnalyzer.Instance.GetForcastData(LotteryType.PK10, (int)rule);
            if (plans.Plan1 == null || plans.Plan2 == null)
                return 1;

            JinMaAnalyzer.Instance.CalcuteScore(ref plans, startWhenBreakGua);
            newForcast.Add(plans.Plan1.ForcastData.LastOrDefault());
            newForcast.Add(plans.Plan2.ForcastData.LastOrDefault());
            return 0;
        }
    }
}