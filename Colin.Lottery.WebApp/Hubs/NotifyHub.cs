using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

using Colin.Lottery.WebApp.Hubs;
using Colin.Lottery.Analyzers;
using Colin.Lottery.Models;

namespace Colin.Lottery.WebApp
{
    public class NotifyHub : BaseHub<NotifyHub>
    {
        public async Task GetNotifications(float criticalScore, bool startWhenBreakGua = false)
        {
            var notifications = await GetAllNotifications(criticalScore, startWhenBreakGua);
            await Clients.Caller.SendAsync("Notify", notifications);

            UserSettings.TryRemove(Context.ConnectionId, out object settings);
            UserSettings.TryAdd(Context.ConnectionId, criticalScore);
        }

        /// <summary>
        /// 获取所有彩种的通知消息
        /// </summary>
        /// <returns>The all notifications.</returns>
        /// <param name="criticalScore">Critical score.</param>
        /// <param name="startWhenBreakGua">If set to <c>true</c> start when break gua.</param>
        async Task<List<string>> GetAllNotifications(float criticalScore, bool startWhenBreakGua)
        {
            var nots = new List<string>();

            await GetNotifications(nots, LotteryType.PK10, (int)PK10Rule.Champion, criticalScore, startWhenBreakGua);
            await GetNotifications(nots, LotteryType.PK10, (int)PK10Rule.Second, criticalScore, startWhenBreakGua);
            await GetNotifications(nots, LotteryType.PK10, (int)PK10Rule.Third, criticalScore, startWhenBreakGua);
            await GetNotifications(nots, LotteryType.PK10, (int)PK10Rule.Fourth, criticalScore, startWhenBreakGua);
            await GetNotifications(nots, LotteryType.PK10, (int)PK10Rule.BigOrSmall, criticalScore, startWhenBreakGua);
            await GetNotifications(nots, LotteryType.PK10, (int)PK10Rule.OddOrEven, criticalScore, startWhenBreakGua);
            await GetNotifications(nots, LotteryType.PK10, (int)PK10Rule.DragonOrTiger, criticalScore, startWhenBreakGua);
            await GetNotifications(nots, LotteryType.PK10, (int)PK10Rule.Sum, criticalScore, startWhenBreakGua);

            return nots;
        }

        /// <summary>
        /// 采集指定彩种和玩法的通知
        /// </summary>
        /// <returns>The notifications.</returns>
        /// <param name="notifications">Notifications.</param>
        /// <param name="lottery">Lottery.</param>
        /// <param name="rule">Rule.</param>
        /// <param name="criticalScore">Critical score.</param>
        /// <param name="startWhenBreakGua">If set to <c>true</c> start when break gua.</param>
        async Task GetNotifications(List<string> notifications, LotteryType lottery, int rule, float criticalScore, bool startWhenBreakGua)
        {
            var plans = await JinMaAnalyzer.Instance.GetForcastData(lottery, rule);
            JinMaAnalyzer.Instance.CalcuteScore(ref plans, startWhenBreakGua);
            (IForcastPlanModel planA, IForcastPlanModel planB) = plans;
            if (planA.Score >= criticalScore)
                notifications.Add(JinMaStrategyService.Instance.CreateNotification(lottery, rule, Plan.PlanA, planA.ForcastDrawNo, planA.Score));
            if (planB.Score >= criticalScore)
                notifications.Add(JinMaStrategyService.Instance.CreateNotification(lottery, rule, Plan.PlanB, planB.ForcastDrawNo, planB.Score));
        }

        /// <summary>
        /// 获取通知临界分数满足当前预测分数的所有连接ID
        /// </summary>
        /// <returns>The connection identifiers.</returns>
        /// <param name="score">预测分数</param>
        public static IReadOnlyList<string> GetConnectionIds(float score)
        {
            var list = new List<string>();
            foreach (var connId in UserSettings.Keys)
            {
                var criticalScore = float.Parse(UserSettings[connId].ToString());
                if (criticalScore > score)
                    continue;

                list.Add(connId);
            }
            return list;
        }
    }
}
