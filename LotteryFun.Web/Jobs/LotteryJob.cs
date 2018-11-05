using System;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using LotteryFun.Web.Exceptions;
using LotteryFun.Web.Models;
using LotteryFun.Web.MQ;
using LotteryFun.Web.Services;
using LotteryFun.Web.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;

namespace LotteryFun.Web.Jobs
{
    /// <summary>
    ///     采集计划号码 JOB
    /// </summary>
    [DisallowConcurrentExecution]
    public sealed class LotteryJob : IJob
    {
        private readonly ICollectService _collectService;
        private readonly IBus _bus;
        private readonly ILogger<LotteryJob> _logger;

        public LotteryJob(ILogger<LotteryJob> logger, ICollectService collectService, IBus bus)
        {
            _logger = logger;
            _collectService = collectService;
            _bus = bus;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var jobDataMap = context.JobDetail.JobDataMap;

            Pk10Rule rule = (Pk10Rule) jobDataMap.GetIntValue("rule");
            Planner planner = (Planner) jobDataMap.GetIntValue("planner");

            var parameter = new {Rule = rule, Planner = planner, Bus = _bus};
            return Task.Factory.StartNew(Start, parameter);
        }

        /// <summary>
        ///     开始
        /// </summary>
        /// <param name="state">需要的参数</param>
        private Task Start(object state)
        {
            var parameter = (dynamic) state;

            Pk10Rule rule = parameter.Rule;
            Planner planner = parameter.Planner;
            IBus bus = parameter.Bus;

            if (!Enum.IsDefined(typeof(Planner), planner))
            {
                _logger.LogError($"计划{planner}未定义, 本次任务终止.");
                return Task.CompletedTask;
            }

            if (!Enum.IsDefined(typeof(Pk10Rule), rule))
            {
                _logger.LogError($"玩法{rule}未定义, 本次任务终止.");
                return Task.CompletedTask;
            }

            var responseJson = string.Empty;
            try
            {
                // 采集计划员发布的号码
                CollectPlan(rule, bus, planner, out responseJson);
            }
            catch (WebRequestException webEx)
            {
                _logger.LogError(webEx.Message + Environment.NewLine + "响应内容:" + Environment.NewLine +
                                 webEx.ResponseContent);
                throw new JobExecutionException(webEx, true);
            }
            catch (JsonReaderException jsEx)
            {
                _logger.LogError(jsEx, $"解析服务器返回的JSON时出错。实际字符串如下：{Environment.NewLine}{responseJson}" +
                                       $"{Environment.NewLine}{jsEx}");
                throw new JobExecutionException(jsEx, true);
            }
            catch (Exception e)
            {
                _logger.LogError("采集计划号码时抛出未处理异常。响应内容如下：" +
                                 $"{Environment.NewLine}{responseJson}{Environment.NewLine}{e}");
                throw new JobExecutionException(e, true);
            }

            return TaskUtils.CompletedTask;
        }

        /// <summary>
        ///     采集计划号码
        /// </summary>
        /// <param name="rule">玩法</param>
        /// <param name="bus"></param>
        /// <param name="planner"></param>
        /// <param name="responseJson"></param>
        private void CollectPlan(Pk10Rule rule, IBus bus, Planner planner, out string responseJson)
        {
            string logPrefix = $"「{rule.GetDescription()} - {planner}」";

            // 执行采集
            responseJson = _collectService.Execute(rule, planner);

            // 解析响应JSON
            var jsonObj = JObject.Parse(responseJson);

            // 计划号码列表
            var listPlanNo = from elem in jsonObj["APICode"]
                select new PlanNo
                {
                    Zhouqi = elem["zhouqi"]?.Value<string>(),
                    Apiname = elem["apiname"].Value<string>(),
                    Number = elem["number"]?.Value<string>(),
                    Ready = elem["ready"]?.Value<int?>(),
                    Resalt = elem["resalt"]?.Value<string>(),
                    Qihao = elem["qihao"]?.Value<int?>(),
                    Opencode = elem["opencode"].Value<string>(),
                    // 统计列
                    NowQihao = elem["nowqihao"]?.Value<int?>()
                };

            // 将反转后的开奖号码加入队列
            bus.Publish(new LotteryPlanMessage(planner, listPlanNo.Reverse().ToList(), rule));

            _logger.LogDebug(logPrefix + "采集计划号码如下：" + Environment.NewLine + responseJson);

            _logger.LogDebug($"{logPrefix} 加入队列.");
        }
    }
}