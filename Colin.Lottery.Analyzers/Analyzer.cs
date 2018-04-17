using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Colin.Lottery.Models;

namespace Colin.Lottery.Analyzers
{
    public abstract class Analyzer<T>
        : Singleton<T>, IAnalyzer
        where T : Analyzer<T>, new()
    {
        public abstract Task<IForcastPlanModel> GetForcastData(LotteryType type, Planner planer, int rule);

        public abstract Task<(IForcastPlanModel Plan1, IForcastPlanModel Plan2)> GetForcastData(LotteryType type, int rule);

        public abstract Task<(IForcastPlanModel Plan1, IForcastPlanModel Plan2)> GetForcastData(LotteryType type);

        public abstract Task<(IForcastPlanModel Plan1, IForcastPlanModel Plan2)> GetForcastData();

        public abstract void Start();

        public abstract void Start(Dictionary<LotteryType, List<int>> typeRules);
    }
}
