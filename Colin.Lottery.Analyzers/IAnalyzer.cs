using System.Collections.Generic;
using System.Threading.Tasks;
using Colin.Lottery.Models;

namespace Colin.Lottery.Analyzers
{
    public interface IAnalyzer
    {
        /// <summary>
        /// 开始数据分析（默认为北京赛车冠军定位胆）
        /// </summary>
        void Start();

        /// <summary>
        /// 开始数据分析
        /// </summary>
        /// <param name="typeRules">彩种和玩法</param>
        void Start(Dictionary<LotteryType, List<int>> typeRules);

        /// <summary>
        /// 查询预测数据
        /// </summary>
        /// <param name="type">彩种</param>
        /// <param name="planer">计划员</param>
        /// <param name="rule">彩票玩法</param>
        /// <returns>彩票预测数据</returns>
        Task<IForcastPlanModel> GetForcastData(LotteryType type, Planner planer, int rule);

        /// <summary>
        /// 查询预测数据
        /// </summary>
        /// <param name="type">彩种</param>
        /// <param name="rule">彩票玩法</param>
        /// <returns>彩票预测数据</returns>
        Task<(IForcastPlanModel Plan1, IForcastPlanModel Plan2)> GetForcastData(LotteryType type, int rule);

        /// <summary>
        /// 查询预测数据（默认为北京赛车冠军定位胆或重庆时时彩个位定位胆）
        /// </summary>
        /// <param name="type">彩种</param>
        /// <returns>彩票预测数据</returns>
        Task<(IForcastPlanModel Plan1, IForcastPlanModel Plan2)> GetForcastData(LotteryType type);

        /// <summary>
        /// 查询预测数据（默认为北京赛车冠军定位胆）
        /// </summary>
        /// <returns>彩票预测数据</returns>
        Task<(IForcastPlanModel Plan1, IForcastPlanModel Plan2)> GetForcastData();
    }
}
