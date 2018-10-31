using System;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Robin.Lottery.WebApp.Exceptions;
using Robin.Lottery.WebApp.Models;
using Robin.Lottery.WebApp.MQ;
using Robin.Lottery.WebApp.Services;

namespace Robin.Lottery.WebApp.Infrastructure
{
    /// <summary>
    ///     采集计划号码 JOB
    /// </summary>
    [DisallowConcurrentExecution]
    public sealed class LotteryPlanJob : IJob
    {
        private readonly ICollectService _collectService;
        private readonly IBus _bus;
        private readonly ILogger<LotteryPlanJob> _logger;

        public LotteryPlanJob(ILogger<LotteryPlanJob> logger, ICollectService collectService, IBus bus)
        {
            _logger = logger;
            _collectService = collectService;
            _bus = bus;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var jobDataMap = context.JobDetail.JobDataMap;

            var ruleValue = jobDataMap.GetString("ruleValue");

            var parameter = new { RuleValue = ruleValue, Bus = _bus };
            return Task.Factory.StartNew(Start, parameter);
        }

        /// <summary>
        ///     开始
        /// </summary>
        /// <param name="state">需要的参数</param>
        private void Start(object state)
        {
            var parameter = (dynamic)state;

            string strRule = parameter.RuleValue;
            if (!Enum.TryParse(strRule, out Pk10Rule rule))
            {
                _logger.LogError($"试图将字符串[{strRule}]转换为类型{typeof(Pk10Rule).FullName}时出错, 本次任务终止.");
                return;
            }

            IBus bus = parameter.Bus;

            var responseJson = string.Empty;
            try
            {
                foreach (var planner in (Planner[])Enum.GetValues(typeof(Planner)))
                {
                    // 采集计划员发布的号码
                    CollectPlan(rule, bus, planner, out responseJson);
                }
            }
            catch (WebRequestException webEx)
            {
                _logger.LogError(webEx.Message + Environment.NewLine + "响应内容:" + Environment.NewLine +
                                 webEx.ResponseContent);
            }
            catch (JsonReaderException jsEx)
            {
                _logger.LogError(jsEx, $"解析服务器返回的JSON时出错。实际字符串如下：{Environment.NewLine}{responseJson}" +
                                       $"{Environment.NewLine}{jsEx}");
            }
            catch (Exception e)
            {
                _logger.LogError("采集计划号码时抛出未处理异常。响应内容如下：" +
                                 $"{Environment.NewLine}{responseJson}{Environment.NewLine}{e}");
            }
        }

        /// <summary>
        ///     采集一个计划员的开奖号码
        /// </summary>
        /// <param name="rule">玩法</param>
        /// <param name="bus"></param>
        /// <param name="planner"></param>
        /// <param name="responseJson"></param>
        private void CollectPlan(Pk10Rule rule, IBus bus, Planner planner, out string responseJson)
        {
            string logPrefix = $"「{planner}计划 - {rule.GetDescription()}」";

            // 执行采集
            responseJson = _collectService.Execute(rule, planner);

            // 解析响应JSON
            var jsonObj = JObject.Parse(responseJson);

            // 构建计划号码列表
            var listPlanNo = from elem in jsonObj["APICode"]
                             where string.Equals(elem["apiname"].Value<string>(), "data", StringComparison.OrdinalIgnoreCase) ==
                                   false
                             select new PlanNo
                             {
                                 Zhouqi = elem["zhouqi"].Value<string>(),
                                 Apiname = elem["apiname"].Value<string>(),
                                 Number = elem["number"].Value<string>(),
                                 Ready = elem["ready"].Value<int>(),
                                 Resalt = elem["resalt"].Value<string>(),
                                 Qihao = elem["qihao"].Value<string>(),
                                 Opencode = elem["opencode"].Value<string>()
                             };

            // 将开奖号码加入队列
            bus.Publish(new LotteryPlanMessage(planner, listPlanNo.ToList(), rule));
            _logger.LogDebug($"{logPrefix} 进入队列~");
        }
    }
}