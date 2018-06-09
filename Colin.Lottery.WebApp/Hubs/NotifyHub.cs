using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colin.Lottery.Analyzers;
using Colin.Lottery.Models;
using Colin.Lottery.WebApp.Services;

namespace Colin.Lottery.WebApp.Hubs
{
    public class NotifyHub : BaseHub<NotifyHub>
    {
        public void GetNotifications(float criticalScore, bool startWhenBreakGua = false)
        {
            //var notifications = await GetAllNotifications(criticalScore, startWhenBreakGua);
            //await Clients.Caller.SendAsync("Notify", notifications);

            UserSettings.TryRemove(Context.ConnectionId, out _);
            UserSettings.TryAdd(Context.ConnectionId, criticalScore);
        }

        /// <summary>
        /// 获取所有彩种的通知消息
        /// </summary>
        /// <returns>The all notifications.</returns>
        /// <param name="criticalScore">Critical score.</param>
        /// <param name="startWhenBreakGua">If set to <c>true</c> start when break gua.</param>
        private async Task<List<string>> GetAllNotifications(float criticalScore, bool startWhenBreakGua)
        {
            var nots = new List<string>();

            await GetNotifications(nots, LotteryType.Pk10, (int)Pk10Rule.Champion, criticalScore, startWhenBreakGua);
            await GetNotifications(nots, LotteryType.Pk10, (int)Pk10Rule.Second, criticalScore, startWhenBreakGua);
            await GetNotifications(nots, LotteryType.Pk10, (int)Pk10Rule.Third, criticalScore, startWhenBreakGua);
            await GetNotifications(nots, LotteryType.Pk10, (int)Pk10Rule.Fourth, criticalScore, startWhenBreakGua);
            await GetNotifications(nots, LotteryType.Pk10, (int)Pk10Rule.BigOrSmall, criticalScore, startWhenBreakGua);
            await GetNotifications(nots, LotteryType.Pk10, (int)Pk10Rule.OddOrEven, criticalScore, startWhenBreakGua);
            await GetNotifications(nots, LotteryType.Pk10, (int)Pk10Rule.DragonOrTiger, criticalScore, startWhenBreakGua);
            await GetNotifications(nots, LotteryType.Pk10, (int)Pk10Rule.Sum, criticalScore, startWhenBreakGua);

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
        private static async Task GetNotifications(ICollection<string> notifications, LotteryType lottery, int rule, float criticalScore, bool startWhenBreakGua)
        {
            var plans = await JinMaAnalyzer.Instance.GetForcastData(lottery, rule);
            JinMaAnalyzer.Instance.CalcuteScore(plans, startWhenBreakGua);
            
            plans.ForEach(p =>
            {
                if (p.Score >= criticalScore)
                    notifications.Add(JinMaStrategyService.Instance.CreateNotification(lottery, rule, Plan.PlanA, p.ForcastDrawNo, p.Score));
            });
        }

        /// <summary>
        /// 获取通知临界分数满足当前预测分数的所有连接ID
        /// </summary>
        /// <returns>The connection identifiers.</returns>
        /// <param name="score">预测分数</param>
        public static IReadOnlyList<string> GetConnectionIds(float score)
        {
            return (from connId in UserSettings.Keys let criticalScore = float.Parse(UserSettings[connId].ToString()) where !(criticalScore > score) select connId).ToList();
        }
    }
}
