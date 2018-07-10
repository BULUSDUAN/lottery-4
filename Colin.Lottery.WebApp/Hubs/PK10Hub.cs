using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

using Colin.Lottery.Analyzers;
using Colin.Lottery.Models;
using Colin.Lottery.Utils;
using Colin.Lottery.Common;

namespace Colin.Lottery.WebApp.Hubs
{
    public class PK10Hub : BaseHub<PK10Hub>
    {
        /// <summary>
        /// 获取指定玩法预测数据(最近15段)
        /// </summary>
        /// <returns>The forcast data.</returns>
        /// <param name="rule">Rule.</param>
        /// <param name="startWhenBreakGua">If set to <c>true</c> start when break gua.</param>
        public async Task GetForcastData(int rule = 1, bool startWhenBreakGua = false)
        {
            var plans = await JinMaAnalyzer.Instance.GetForcastData(LotteryType.Pk10, rule);
            if (plans == null || plans.Count < 2 || plans.Any(p => p == null))
            {
                await Clients.Caller.SendAsync("NoResult");
                LogUtil.Warn("目标网站扫水接口异常，请尽快检查恢复");
            }
            else
            {
                JinMaAnalyzer.Instance.CalcuteScore(plans, startWhenBreakGua);
                await Clients.Caller.SendAsync("ShowPlans", plans);
            }


            Lotterys.Pk10Rules.ForEach(async r => await Groups.RemoveFromGroupAsync(Context.ConnectionId, r.ToString()));
            await Groups.AddToGroupAsync(Context.ConnectionId, ((Pk10Rule)rule).ToString());
        }

        /// <summary>
        /// 获取PK10所有玩法最新期预测数据
        /// </summary>
        /// <returns>The all new forcast.</returns>
        public async Task GetAllNewForcast(bool startWhenBreakGua = false)
        {
            var forcast = new List<IForcastModel>();
            var error = 0;
            error += await GetNewForcast(forcast, Pk10Rule.Champion, startWhenBreakGua);
            error += await GetNewForcast(forcast, Pk10Rule.Second, startWhenBreakGua);
            error += await GetNewForcast(forcast, Pk10Rule.Third, startWhenBreakGua);
            error += await GetNewForcast(forcast, Pk10Rule.Fourth, startWhenBreakGua);
            error += await GetNewForcast(forcast, Pk10Rule.BigOrSmall, startWhenBreakGua);
            error += await GetNewForcast(forcast, Pk10Rule.OddOrEven, startWhenBreakGua);
            error += await GetNewForcast(forcast, Pk10Rule.DragonOrTiger, startWhenBreakGua);
            error += await GetNewForcast(forcast, Pk10Rule.Sum, startWhenBreakGua);

            if (error > 0)
            {
                await Clients.Caller.SendAsync("NoResult", "allRules");
                LogUtil.Warn("目标网站扫水接口异常，请尽快检查恢复");
            }

            await Clients.Caller.SendAsync("ShowPlans", forcast);

            await RegisterAllRules();
        }

        private static async Task<int> GetNewForcast(ICollection<IForcastModel> newForcast, Pk10Rule rule, bool startWhenBreakGua)
        {
            var plans = await JinMaAnalyzer.Instance.GetForcastData(LotteryType.Pk10, (int)rule);
            if (plans == null || plans.Count < 2)
                return 1;

            JinMaAnalyzer.Instance.CalcuteScore(plans, startWhenBreakGua);
            plans.ForEach(p => newForcast.Add(p.ForcastData.LastOrDefault()));
            return 0;
        }

        /// <summary>
        /// 注册所有玩法(供模拟下注和自动下注调用)
        /// </summary>
        /// <returns>The all rules.</returns>
        public async Task RegisterAllRules() => await Groups.AddToGroupAsync(Context.ConnectionId, "AllRules");
    }
}