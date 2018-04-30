using System.Collections.Generic;
using System.Threading.Tasks;

using Colin.Lottery.Models;
using Microsoft.AspNetCore.SignalR;

namespace Colin.Lottery.WebApp
{
    public abstract class StrategyService<T>
        : Singleton<T>, IStrategyService
        where T : StrategyService<T>, new()
    {
        public IHubContext<NotifyHub> NotifyContext => Startup.GetService<IHubContext<NotifyHub>>();

        public string CreateNotification(LotteryType lottery, int rule, Plan plan, string forcastDrawNo, float score) =>
        $"{lottery.ToStringName()} {rule.ToStringName(lottery)} {plan.ToString()} 预测号码:{forcastDrawNo} 评分:{score} 推荐下注";

        public abstract Task Start(bool startWhenBreakGua = false);

        public abstract void Start(Dictionary<LotteryType, List<int>> typeRules, bool startWhenBreakGua = false);

    }
}
