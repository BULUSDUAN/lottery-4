using System.Linq;
using System.Collections.Generic;
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

            //更新用户配置
            UserSettings.TryRemove(Context.ConnectionId, out object settings);
            UserSettings.TryAdd(Context.ConnectionId, new PK10Rule[] { (PK10Rule)rule });
        }

        /// <summary>
        /// 获取关注指定玩法的所有连接ID
        /// </summary>
        /// <returns>The connection identifiers.</returns>
        /// <param name="rule">PK10玩法</param>
        public static IReadOnlyList<string> GetConnectionIds(PK10Rule rule)
        {
            var list = new List<string>();
            foreach (var connId in UserSettings.Keys)
            {
                var rules = UserSettings[connId] as PK10Rule[];
                if (!rules.Contains(rule))
                    continue;

                list.Add(connId);
            }
            return list;
        }
    }
}