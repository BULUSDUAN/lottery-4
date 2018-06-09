using System.Collections.Generic;
using System.Threading.Tasks;

using Colin.Lottery.Models;

namespace Colin.Lottery.Analyzers
{
    public abstract class Analyzer<T>
        : Singleton<T>, IAnalyzer
        where T : Analyzer<T>, new()
    {
        public abstract Task<IForcastPlanModel> GetForcastData(LotteryType type, Planner planer, int rule);

        public abstract Task<List<IForcastPlanModel>> GetForcastData(LotteryType type, int rule);

        public abstract Task<List<IForcastPlanModel>> GetForcastData(LotteryType type);

        public abstract Task<List<IForcastPlanModel>> GetForcastData();

        public abstract void CalcuteScore(List<IForcastPlanModel> plans, bool startWhenBreakGua);
    }
}
