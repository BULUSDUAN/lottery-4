using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
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
        /// 获取指定玩法预测数据(最近15段)并订阅玩法 Web端详情页调用
        /// </summary>
        /// <returns>The forcast data.</returns>
        /// <param name="rule">Rule.</param>
        public async Task GetForcastData(int rule = 1)
        {
            await GetForcastDataByRule(rule);

            Lotterys.Pk10Rules.ForEach(async r =>
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, r.ToString()));
            await Groups.AddToGroupAsync(Context.ConnectionId, ((Pk10Rule)rule).ToString());
        }

        /// <summary>
        /// 获取PK10所有玩法最新期预测数据并订阅所有玩法 Web端彩票大厅调用
        /// </summary>
        /// <returns>The all new forcast.</returns>
        public async Task GetAllNewForcast()
        {
            var forcast = new List<IForcastModel>();
            var error = 0;
            error += await GetNewForcast(forcast, Pk10Rule.Champion);
            error += await GetNewForcast(forcast, Pk10Rule.Second);
            error += await GetNewForcast(forcast, Pk10Rule.Third);
            error += await GetNewForcast(forcast, Pk10Rule.Fourth);
            error += await GetNewForcast(forcast, Pk10Rule.BigOrSmall);
            error += await GetNewForcast(forcast, Pk10Rule.OddOrEven);
            error += await GetNewForcast(forcast, Pk10Rule.DragonOrTiger);
            error += await GetNewForcast(forcast, Pk10Rule.Sum);

            if (error > 0)
            {
                await Clients.Caller.SendAsync("NoResult", "allRules");
                LogUtil.Warn("目标网站扫水接口异常，请尽快检查恢复");
            }

            await Clients.Caller.SendAsync("ShowPlans", forcast);

            await RegisterAllRules();
        }

        private static async Task<int> GetNewForcast(ICollection<IForcastModel> newForcast, Pk10Rule rule)
        {
            var plans = await JinMaAnalyzer.Instance.GetForcastData(LotteryType.Pk10, (int)rule);
            if (plans == null || plans.Count < 2 || plans.Any(p => p == null))
                return 1;

            JinMaAnalyzer.Instance.CalcuteScore(plans);
            plans.ForEach(p => newForcast.Add(p.ForcastData.LastOrDefault()));
            return 0;
        }

        /// <summary>
        /// 注册所有玩法(供模拟下注和自动下注调用)
        /// </summary>
        /// <returns>The all rules.</returns>
        public async Task RegisterAllRules() => await Groups.AddToGroupAsync(Context.ConnectionId, "AllRules");

        /// <summary>
        /// 获取PK10前四名最新期预测数据 App端使用
        /// </summary>
        /// <returns></returns>
        public async Task GetAppNewForcast()
        {
            var forcast = new List<IForcastModel>();
            var error = 0;
            error += await GetNewForcast(forcast, Pk10Rule.Champion);
            error += await GetNewForcast(forcast, Pk10Rule.Second);
            error += await GetNewForcast(forcast, Pk10Rule.Third);
            error += await GetNewForcast(forcast, Pk10Rule.Fourth);

            if (error > 0)
            {
                await Clients.Caller.SendAsync("NoResult", "allRules");
                LogUtil.Warn("目标网站扫水接口异常，请尽快检查恢复");
            }

            var liangua =
                forcast.Where(f => f.KeepGuaCnt >= Convert.ToInt32(ConfigUtil.Configuration["AppNotify:Min"]));
            if (liangua.Any())
                await Clients.Caller.SendAsync("ShowForcasts", liangua.ToList());
            else
                await Clients.Caller.SendAsync("NoResult");

            await Groups.AddToGroupAsync(Context.ConnectionId, "App");
        }

        //心跳检测  App端使用
        public void Heartbeat(){}

        /// <summary>
        /// 获取指定玩法预测数据(最近15段)
        /// </summary>
        /// <returns>The forcast data by rule.</returns>
        /// <param name="rule">Rule.</param>
        public async Task GetForcastDataByRule(int rule)
        {
            var plans = await JinMaAnalyzer.Instance.GetForcastData(LotteryType.Pk10, rule);
            if (plans == null || plans.Count() < 2 || plans.Any(p => p == null))
            {
                await Clients.Caller.SendAsync("NoResult");
                LogUtil.Warn("目标网站扫水接口异常，请尽快检查恢复");
            }
            else
            {
                JinMaAnalyzer.Instance.CalcuteScore(plans);
                await Clients.Caller.SendAsync("ShowPlans", plans);
            }
        }

        //        /// <summary>
        //        /// 投注
        //        /// </summary>
        //        /// <param name="periodNo">期号</param>
        //        /// <param name="rule">玩法</param>
        //        /// <param name="numberRange">02 04 08 09 10</param>
        //        /// <param name="money">下注金额</param>
        //        public void BetDa2088(long periodNo, int rule, string numberRange, decimal money)
        //        {
        //            if (Da2088Helper.SiteBetDa2088.LoginTimeout)
        //            {
        //                Da2088Helper.SiteBetDa2088.Login();
        //            }
        //
        //            string betNumbers = numberRange;
        //
        //            Pk10Rule pk10Rule = (Pk10Rule) rule;
        //            if (pk10Rule < Pk10Rule.BigOrSmall)
        //            {
        //                var arr = numberRange.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x));
        //                betNumbers = string.Join(",", arr);
        //            }
        //
        //            if (money <= 0) money = 1;
        //
        //            string notifyMessage = string.Empty;
        //            try
        //            {
        //                // TODO: 当网页上登录时，此处会报“账号在异地登陆”的异常，需要才重新登录
        //                Da2088Helper.SiteBetDa2088.Bet(periodNo, pk10Rule, betNumbers, money);
        //
        //                var lotteryData = Da2088Helper.SiteBetDa2088.GetLotteryData();
        //                notifyMessage = $"投注成功！ 当前余额: ￥{lotteryData.Balance}";
        //                Clients.Caller.SendAsync("ShowBetResult", notifyMessage, NotifyLevel.success.ToString());
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"异常! {ex}");
        //
        //                notifyMessage = ex.Message;
        //                Clients.Caller.SendAsync("ShowBetResult", notifyMessage, NotifyLevel.danger.ToString());
        //            }
        //        }
        //
        //

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AllRules");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "App");
            Lotterys.Pk10Rules.ForEach(async rule =>
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, ((Pk10Rule)rule).ToString()));

            await base.OnDisconnectedAsync(exception);
        }
    }
}