using System.Collections.Generic;
using System.Threading.Tasks;
using Colin.Lottery.Models;

namespace Colin.Lottery.Analyzers
{
    public interface IAnalyzer
    {
        /// <summary>
        /// 查询预测数据
        /// </summary>
        /// <param name="type">彩种</param>
        /// <param name="planer">计划员</param>
        /// <param name="rule">彩票玩法</param>
        /// <returns>彩票预测数据</returns>
        Task<IForecastPlanModel> GetForecastData(LotteryType type, Planner planer, int rule);

        /// <summary>
        /// 查询预测数据
        /// </summary>
        /// <param name="type">彩种</param>
        /// <param name="rule">彩票玩法</param>
        /// <returns>彩票预测数据</returns>
        Task<List<IForecastPlanModel>> GetForecastData(LotteryType type, int rule);

        /// <summary>
        /// 查询预测数据（默认为北京赛车冠军定位胆或重庆时时彩个位定位胆）
        /// </summary>
        /// <param name="type">彩种</param>
        /// <returns>彩票预测数据</returns>
        Task<List<IForecastPlanModel>> GetForecastData(LotteryType type);

        /// <summary>
        /// 查询预测数据（默认为北京赛车冠军定位胆）
        /// </summary>
        /// <returns>彩票预测数据</returns>
        Task<List<IForecastPlanModel>> GetForecastData();

        /// <summary>
        /// 对计划评分
        /// </summary>
        /// <param name="plans">Plans.</param>
        void CalcuteScore(List<IForecastPlanModel> plans);
    }
}
