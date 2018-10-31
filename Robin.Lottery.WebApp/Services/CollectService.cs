using System;
using Microsoft.Extensions.Logging;
using Robin.Lottery.WebApp.Models;
using Robin.Lottery.WebApp.Utils;

namespace Robin.Lottery.WebApp.Services
{
    /// <summary>
    ///     采集服务
    /// </summary>
    public class CollectService : ICollectService
    {
        private readonly AppConfig _config;
        private readonly ILogger<CollectService> _logger;
        private readonly IWebRequest _webRequest;

        public CollectService(AppConfig config, ILogger<CollectService> logger)
        {
            _config = config;
            _logger = logger;
            _webRequest = new WebRequest(config.SourceSiteDomain);
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
            string logPrefix = $"「{planner}计划 - {rule.GetDescription()}」";

            var urlFormat =
                string.Concat(_config.SourceSiteDomain, "ajax_getapi.php?type={0}{1}&a={2}&t=", DateTime.Now.Ticks
                    .ToString());
            var strPlanner = planner == Planner.B ? planner.ToStringValue() : string.Empty;
            var url = string.Format(urlFormat, game, strPlanner, (int)rule);

            string responseJson;

            try
            {
                // Trim 掉 UTF8 BOM 中的特殊字符， 否则 Linux/Mac 下不是标准的 JSON, Json.Net 解析会抛异常
                responseJson = _webRequest.GetString(url, null);
            }
            catch (Exception e)
            {
                throw new ApplicationException($"采集{logPrefix}号码时抛出未处理异常。", e);
            }

            if (string.IsNullOrEmpty(responseJson)) return responseJson;

            _logger.LogDebug(logPrefix + Environment.NewLine + responseJson);

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