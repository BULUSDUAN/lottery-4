using System;
using LotteryFun.Web.Models;
using Microsoft.Extensions.Logging;

namespace LotteryFun.Web.Services
{
    /// <summary>
    ///     采集服务
    /// </summary>
    public class CollectService : ICollectService
    {
        private readonly AppConfig _config;
        private readonly ILogger<CollectService> _logger;
        private readonly IWebRequestService _webRequestService;

        public CollectService(AppConfig config, ILogger<CollectService> logger)
        {
            _config = config;
            _logger = logger;
            _webRequestService = new WebRequestService(config.SourceSiteDomain);
        }

        /// <summary>
        ///     执行采集
        /// </summary>
        /// <param name="rule">玩法</param>
        /// <param name="planner">计划员</param>
        /// <param name="game">游戏</param>
        /// <returns></returns>
        public string Execute(Pk10Rule rule, Planner planner, string game = "pk10")
        {
            string logPrefix = $"「{rule.GetDescription()} - {planner} 」";

            var urlFormat =
                string.Concat(_config.SourceSiteDomain, "ajax_getapi.php?type={0}{1}&a={2}&t=", DateTime.Now.Ticks
                    .ToString());
            var strPlanner = planner == Planner.B ? planner.ToStringValue() : string.Empty;
            var url = string.Format(urlFormat, game, strPlanner, (int)rule);

            string responseJson;

            try
            {
                responseJson = _webRequestService.GetString(url);
            }
            catch (Exception e)
            {
                throw new ApplicationException($"采集{logPrefix}号码时抛出未处理异常。", e);
            }

            

            return responseJson;
        }
    }

    public interface ICollectService
    {
        /// <summary>
        ///     执行采集
        /// </summary>
        /// <param name="rule">玩法</param>
        /// <param name="planner">计划员</param>
        /// <param name="game">游戏</param>
        /// <returns></returns>
        string Execute(Pk10Rule rule, Planner planner, string game = "pk10");
    }
}