using System.Threading.Tasks;

using Colin.Lottery.Models;

namespace Colin.Lottery.Analyzers
{
    public abstract class Analyzer<T>
        : Singleton<T>, IAnalyzer
        where T : Analyzer<T>, new()
    {
        public abstract Task<IForcastPlanModel> GetForcastData(LotteryType type, Planner planer, int rule);

        public abstract Task<(IForcastPlanModel PlanA, IForcastPlanModel PlanB)> GetForcastData(LotteryType type, int rule);

        public abstract Task<(IForcastPlanModel PlanA, IForcastPlanModel PlanB)> GetForcastData(LotteryType type);

        public abstract Task<(IForcastPlanModel PlanA, IForcastPlanModel PlanB)> GetForcastData();

        public abstract void CalcuteScore(ref (IForcastPlanModel PlanA, IForcastPlanModel PlanB) plans, bool startWhenBreakGua);
    }
}
