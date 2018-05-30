using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

using Colin.Lottery.Models;
using Colin.Lottery.WebApp.Hubs;

namespace Colin.Lottery.WebApp.Services
{
    public interface IStrategyService
    {
        IHubContext<NotifyHub> NotifyContext { get; }

        string CreateNotification(LotteryType lottery, int rule, Plan plan, string forcastDrawNo, float score);

        /// <summary>
        /// 开始数据分析（默认为北京赛车冠军定位胆）
        /// </summary>
        Task Start(bool startWhenBreakGua = false);

        /// <summary>
        /// 开始数据分析
        /// </summary>
        /// <param name="typeRules">彩种和玩法</param>
        /// <param name="startWhenBreakGua"></param>
        void Start(Dictionary<LotteryType, List<int>> typeRules, bool startWhenBreakGua = false);
    }
}
