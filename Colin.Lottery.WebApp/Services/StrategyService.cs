using System.Collections.Generic;
using System.Threading.Tasks;

using Colin.Lottery.Models;

namespace Colin.Lottery.WebApp
{
    public abstract class StrategyService<T>
        : Singleton<T>, IStrategyService
        where T : StrategyService<T>, new()
    {
        public abstract Task Start(bool startWhenBreakGua = false);

        public abstract void Start(Dictionary<LotteryType, List<int>> typeRules, bool startWhenBreakGua = false);
    }
}
