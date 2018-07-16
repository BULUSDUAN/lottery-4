using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Colin.Lottery.Models;

namespace Colin.Lottery.DataService
{
    public interface IDataService
    {
        /// <summary>
        /// 开始数据分析（默认为北京赛车冠军定位胆）
        /// </summary>
        Task Start();

        /// <summary>
        /// 开始数据分析
        /// </summary>
        /// <param name="typeRules">彩种和玩法</param>
        void Start(Dictionary<LotteryType, List<int>> typeRules);

        
        /// <summary>
        /// 访问结束后
        /// </summary>
        event EventHandler<DataCollectedEventArgs> DataCollectedSuccess;

        /// <summary>
        /// 数据请求错误、CurrentGuaCnt
        /// </summary>
        event EventHandler<CollectErrorEventArgs> DataCollectedError;
    }

    public class CollectErrorEventArgs
    {
        public Pk10Rule Rule { get; }

        public Exception Exception { get; set; }

        public CollectErrorEventArgs(Pk10Rule rule,Exception exception)
        {
            this.Rule = rule;
            this.Exception = exception;
        }

        public CollectErrorEventArgs(Pk10Rule rule, string errorMessage)
        {
            this.Rule = rule;
            this.Exception = new Exception(errorMessage);
        }
    }

    public class DataCollectedEventArgs
    {
        public Pk10Rule Rule { get; }

        /// <summary>
        /// 完整15期计划
        /// </summary>
        public List<IForcastPlanModel> Plans { get; }

        /// <summary>
        /// 最新期计划
        /// </summary>
        public List<IForcastModel> LastForcastData => Plans.Select(p => p.ForcastData.LastOrDefault())?.ToList();

        public DataCollectedEventArgs(Pk10Rule rule,List<IForcastPlanModel> plans)
        {
            this.Rule = rule;
            this.Plans = plans;
        }
    }
}
