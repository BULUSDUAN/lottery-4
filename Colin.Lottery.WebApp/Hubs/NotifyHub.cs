using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colin.Lottery.Analyzers;
using Colin.Lottery.Models;

namespace Colin.Lottery.WebApp.Hubs
{
    public class NotifyHub : BaseHub<NotifyHub>
    {
        public void GetNotifications(float criticalScore)
        {
            //var notifications = await GetAllNotifications(criticalScore);
            //await Clients.Caller.SendAsync("Notify", notifications);

            UserSettings.TryRemove(Context.ConnectionId, out _);
            UserSettings.TryAdd(Context.ConnectionId, criticalScore);
        }

        /// <summary>
        /// 获取所有彩种的通知消息
        /// </summary>
        /// <returns>The all notifications.</returns>
        /// <param name="criticalScore">Critical score.</param>
        private async Task<List<string>> GetAllNotifications(float criticalScore)
        {
            var nots = new List<string>();

            await GetNotifications(nots, LotteryType.Pk10, (int)Pk10Rule.Champion, criticalScore);
            await GetNotifications(nots, LotteryType.Pk10, (int)Pk10Rule.Second, criticalScore);
            await GetNotifications(nots, LotteryType.Pk10, (int)Pk10Rule.Third, criticalScore);
            await GetNotifications(nots, LotteryType.Pk10, (int)Pk10Rule.Fourth, criticalScore);
            await GetNotifications(nots, LotteryType.Pk10, (int)Pk10Rule.BigOrSmall, criticalScore);
            await GetNotifications(nots, LotteryType.Pk10, (int)Pk10Rule.OddOrEven, criticalScore);
            await GetNotifications(nots, LotteryType.Pk10, (int)Pk10Rule.DragonOrTiger, criticalScore);
            await GetNotifications(nots, LotteryType.Pk10, (int)Pk10Rule.Sum, criticalScore);

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
        private static async Task GetNotifications(ICollection<string> notifications, LotteryType lottery, int rule, float criticalScore)
        {
            var plans = await JinMaAnalyzer.Instance.GetForcastData(lottery, rule);
            JinMaAnalyzer.Instance.CalcuteScore(plans);

            //plans.ForEach(p =>
            //{
            //    if (p.Score >= criticalScore)
            //        notifications.Add(JinMaStrategyService.Instance.CreateNotification(lottery, rule, Plan.PlanA, p.ForcastDrawNo, p.Score));
            //});
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
