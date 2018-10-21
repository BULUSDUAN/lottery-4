using System.Collections.Generic;
using System.Threading.Tasks;

using Colin.Lottery.Models;

namespace Colin.Lottery.Analyzers
{
    public abstract class Analyzer<T>
        : Singleton<T>, IAnalyzer
        where T : Analyzer<T>, new()
    {
        public abstract Task<IForecastPlanModel> GetForecastData(LotteryType type, Planner planer, int rule);

        public abstract Task<List<IForecastPlanModel>> GetForecastData(LotteryType type, int rule);

        public abstract Task<List<IForecastPlanModel>> GetForecastData(LotteryType type);

        public abstract Task<List<IForecastPlanModel>> GetForecastData();

        public abstract void CalcuteScore(List<IForecastPlanModel> plans);
    }
}
