using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Colin.Lottery.Utils;
using Colin.Lottery.Models;
using OpenQA.Selenium.Internal;


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
        private static readonly string RootUrl = ConfigUtil.Configuration["CollectRootUrl"];

        /// <summary>
        /// 获取开奖号码历史记录接口地址
        /// </summary>
        /// <param name="type">彩种</param>
        /// <returns>开奖号码历史记录接口地址</returns>
        private static string GetHistoryUrl(LotteryType type)
        {
            var baseUrl = $"{RootUrl}ajax_getlotry.php";

            switch (type)
            {
                case LotteryType.Cqssc:
                    return baseUrl;
                case LotteryType.Pk10:
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
        private static string GetForecastUrl(LotteryType type, Planner planner, int rule)
        {
            var format =
                $"{RootUrl}ajax_getapi.php?type={{0}}{((int) planner > 1 ? ((int) planner).ToString() : string.Empty)}&a={rule}&t=0.{new Random().Next()}";

            switch (type)
            {
                case LotteryType.Cqssc:
                    return string.Format(format, "ssc");
                case LotteryType.Pk10:
                    return string.Format(format, "pk10");
                default:
                    return null;
            }
        }


        public override async Task<IDrawCollectionModel> GetDrawNoHistory(LotteryType type)
        {
            var response = await HttpUtil.GetAsync(GetHistoryUrl(type));
            return JsonConvert.DeserializeObject<JinMaLotteryModelCollection>(response);
        }

        public static async Task<IForecastPlanModel> GetForecastData(LotteryType type, Planner planer, int rule)
        {
            var response = await HttpUtil.GetAsync(GetForecastUrl(type, planer, rule));
            try
            {
                if (string.IsNullOrWhiteSpace(response))
                    return null;

                var result = JsonConvert.DeserializeObject<JinMaForecastPlanModel>(response);
                result.Plan = planer.GetPlan();
                if (result.ForecastData != null && result.ForecastData.Any())
                {
                    var current = result.ForecastData.LastOrDefault();
                    var pn = current.LastDrawnPeriod % 1000 >= current.LastPeriod
                        ? current.LastPeriod / 1000 * 1000 + current.LastPeriod - 1
                        : current.LastDrawnPeriod;

                    current.LastDrawnPeriod = pn;
                    result.LastDrawnPeriod = pn;
                }

                return result;
            }
            catch (Exception ex)
            {
                ExceptionlessUtil.Warn(ex,$"预测数据反序列化失败，内容:{response}");
                return null;
            }
        }
    }
}