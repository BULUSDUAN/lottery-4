using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Colin.Lottery.Utils;
using Colin.Lottery.Models;


namespace Colin.Lottery.Collectors
{
    /// <summary>
    /// 金马团队分析计划采集器 (https://3530.net)
    /// </summary>
    public class JinMaCollector : Collector<JinMaCollector>
    {
        /// <summary>
        /// 异步接口根地址
        /// </summary>
        private static readonly string _ROOT_URL = "https://47927.com/ajax/";

        /// <summary>
        /// 获取开奖号码历史记录接口地址
        /// </summary>
        /// <param name="type">彩种</param>
        /// <returns>开奖号码历史记录接口地址</returns>
        private string GetHistoryUrl(LotteryType type)
        {
            string baseUrl = $"{_ROOT_URL}ajax_getlotry.php";

            switch (type)
            {
                case LotteryType.CQSSC:
                    return baseUrl;
                case LotteryType.PK10:
                    return $"{baseUrl}?cai=pk10";
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取预测开奖号码接口地址
        /// </summary>
        /// <param name="type">彩种</param>
        /// <param name="planner">计划员</param>
        /// <param name="rule">玩法(请使用具体玩法枚举,如"PK10Rule","CQSSCRule")</param>
        /// <returns>预测开奖号码接口地址</returns>
        private string GetForecastUrl(LotteryType type, Planner planner, int rule)
        {
            string format = $"{_ROOT_URL}ajax_getapi.php?type={{0}}{((int)planner > 1 ? ((int)planner).ToString() : string.Empty)}&a={(int)rule}&t=0.{new Random().Next()}";

            switch (type)
            {
                case LotteryType.CQSSC:
                    return string.Format(format, "ssc");
                case LotteryType.PK10:
                    return string.Format(format, "pk10");
                default:
                    return null;
            }
        }


        public override async Task<IDrawCollectionModel> GetDrawNoHistory(LotteryType type)
        {
            string response = await HttpUtil.GetStringAsync(GetHistoryUrl(type));
            return JsonConvert.DeserializeObject<JinMaLotteryModelCollection>(response);
        }

        public async Task<IForcastPlanModel> GetForcastData(LotteryType type, Planner planer, int rule)
        {
            string response = await HttpUtil.GetStringAsync(GetForecastUrl(type, planer, rule));
            try
            {
                if (string.IsNullOrWhiteSpace(response))
                    return null;

                return JsonConvert.DeserializeObject<JinMaForcastPlanModel>(response);
            }
            catch (Exception ex)
            {
                LogUtil.Warn($"预测数据反序列化失败，内容:{response}\r\n{ex.Message}\r\n{ex.StackTrace}");
                return null;
            }
        }
    }
}
