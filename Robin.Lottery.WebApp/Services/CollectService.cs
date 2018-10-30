using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Robin.Lottery.WebApp.IServices;
using Robin.Lottery.WebApp.Models;
using Robin.Lottery.WebApp.Utils;
using Newtonsoft.Json.Linq;

namespace Robin.Lottery.WebApp.Services
{
    /// <summary>
    /// 采集计划号码逻辑
    /// </summary>
    public class CollectService : ICollectService
    {
        private readonly AppConfig _config;
        private readonly ILogger<CollectService> _logger;
        private readonly IWebClient _webClient;

        public CollectService(AppConfig config, ILogger<CollectService> logger)
        {
            _config = config;
            _logger = logger;
            _webClient = new WebClient(config.PlanSiteDomain);
        }

        /// <summary>
        /// 抓取计划号码
        /// </summary>
        /// <param name="ruleValue"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public string Request(string ruleValue, string game = "pk10")
        {
            const string planA = "1", planB = "2";
            string urlFormat =
                string.Concat(_config.PlanSiteDomain, "ajax_getapi.php?type={0}{1}&a={2}&t=", DateTime.Now.Ticks
                    .ToString());
            string urlA = string.Format(urlFormat, game, string.Empty, ruleValue);
            string urlB = string.Format(urlFormat, game, planB, ruleValue);

            string responseJson;

            try
            {
                // Trim 掉 UTF8 BOM 中的特殊字符， 否则 Json.Net 解析会抛异常
                responseJson = _webClient.GetString(urlA, null)?.Trim('\uFEFF', '\u200B');
            }
            catch (Exception e)
            {
                throw new ApplicationException("抓取计划号码时抛出异常。", e);
            }

            if (string.IsNullOrEmpty(responseJson)) return responseJson;

            try
            {
                var jsonObj = JObject.Parse(responseJson);

                JArray[] jsonArray = jsonObj["APICode"].Value<JArray[]>();
                var listPlanNo = new List<PlanNo>();
                for (int idx = 0; idx < jsonArray.Length; idx++)
                {
                    var elem = jsonArray[idx];
                    if (elem["apiname"].Value<string>() != "data")
                    {
                        var planNo = new PlanNo();
                        planNo.Zhouqi = elem["zhouqi"].Value<string>();
                        planNo.Apiname = elem["apiname"].Value<string>();
                        planNo.Number = elem["number"].Value<string>();
                        planNo.Ready = elem["ready"].Value<int>();
                        planNo.Resalt = elem["resalt"].Value<string>();
                        planNo.Qihao = elem["qihao"].Value<string>();
                        planNo.Opencode = elem["opencode"].Value<string>();

                        listPlanNo.Add(planNo);
                    }
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException($"解析抓取到的计划号码的数据时抛出异常。{Environment.NewLine}JSON 数据如下：{responseJson}", e);
            }


            Enum.TryParse(ruleValue, out Pk10Rule rule);
            _logger.LogDebug($"「抓取计划号码」玩法：{rule.GetDescription()}，计划：PlanA");
            _logger.LogDebug(responseJson);

            return responseJson;
        }
    }
}