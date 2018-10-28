using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Colin.Lottery.Models;
using Colin.Lottery.Common;
using Microsoft.Extensions.Caching.Memory;

namespace Colin.Lottery.WebApp.Hubs
{
    public class PK10Hub : BaseHub<PK10Hub>
    {
        private static readonly IMemoryCache Cache = Startup.GetService<IMemoryCache>();

        /// <summary>
        /// 获取指定玩法预测数据(最近15段)并订阅玩法 Web端详情页调用
        /// </summary>
        /// <returns>The forecast data.</returns>
        /// <param name="rule">Rule.</param>
        public async Task GetForecastData(int rule = 1)
        {
            List<IForecastPlanModel> plans = null;
            if (Cache.TryGetValue<ConcurrentDictionary<int, List<IForecastPlanModel>>>(LotteryType.Pk10,
                out var ps))
                ps.TryGetValue(rule, out plans);
            if (plans == null)
                await Clients.Caller.SendAsync("NoResult");
            else
                await Clients.Caller.SendAsync("ShowPlans", plans);

            Lotterys.Pk10Rules.ForEach(async r =>
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, r.ToString()));
            await Groups.AddToGroupAsync(Context.ConnectionId, ((Pk10Rule) rule).ToString());
        }

        /// <summary>
        /// 获取PK10所有玩法最新期预测数据并订阅所有玩法 Web端彩票大厅调用
        /// </summary>
        /// <returns>The all new forecast.</returns>
        public async Task GetAllNewForecast()
        {
            var forecasts = new List<IForecastModel>();
            if (Cache.TryGetValue<ConcurrentDictionary<int, List<IForecastPlanModel>>>(LotteryType.Pk10,
                out var ps))
            {
                for (var i = 1; i <= (int) Pk10Rule.Sum; i++)
                {
                    if (!ps.TryGetValue(i, out var plans))
                        continue;

                    plans.ForEach(p => forecasts.Add(p.ForecastData.LastOrDefault()));
                }
            }

            if (forecasts.Any())
                await Clients.Caller.SendAsync("ShowPlans", forecasts);
            else
                await Clients.Caller.SendAsync("NoResult", "allRules");

            await RegisterAllRules();
        }

        /// <summary>
        /// 注册所有玩法(供模拟下注和自动下注调用)
        /// </summary>
        /// <returns>The all rules.</returns>
        public async Task RegisterAllRules() => await Groups.AddToGroupAsync(Context.ConnectionId, "AllRules");

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AllRules");
            Lotterys.Pk10Rules.ForEach(async rule =>
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, rule.ToString()));

            await base.OnDisconnectedAsync(exception);
        }
    }
}