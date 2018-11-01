using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using Robin.Lottery.WebApp.Models;

namespace Robin.Lottery.WebApp.MQ
{
    /// <summary>
    ///     计划号码的消费者
    /// </summary>
    public class LotteryPlanConsumer
    {
        private readonly IBus _bus;
        private readonly ILogger<LotteryPlanConsumer> _logger;
        private readonly AppConfig _config;

        private readonly IDictionary<string, int> _latestQihao
            = new ConcurrentDictionary<string, int>();

        public LotteryPlanConsumer(IBus bus, ILogger<LotteryPlanConsumer> logger, AppConfig config)
        {
            _bus = bus;
            _logger = logger;
            _config = config;
        }

        public void Subscribe()
        {
            ISubscriptionResult result = _bus.Subscribe<LotteryPlanMessage>("LotteryPlanConsumer", Handler);
            _logger.LogInformation($"已开始订阅 {nameof(LotteryPlanMessage)} 消息.");
        }

        private void Handler(LotteryPlanMessage message)
        {
            var planner = message.Planner;
            var rule = message.Pk10Rule;

            string logPrefix = $"「{planner}计划 - {rule.GetDescription()}」";

            const int minElemCount = 4;
            if (message.ListPlanNo == null || message.ListPlanNo.Count <= minElemCount)
            {
                _logger.LogWarning($"{logPrefix} 计划号码为空, 或历史追号小于{minElemCount}段, 订阅者停止本次任务.");
                return;
            }

            var first = message.ListPlanNo.First();
            //var second = message.ListPlanNo.Skip(1).First();
            var nowQihao = first.NowQihao.Value + 1;   // 下一期期号
            if (nowQihao == 0)
            {
                _logger.LogWarning($"{logPrefix} 最新期号 nowqihao 为 null, 订阅者停止本次任务.");
                return;
            }

            //
            // 判断是否最新期号. 
            // 如果不是，则返回
            //
            var key = $"{planner}_{rule}";
            if (_latestQihao.ContainsKey(key) && _latestQihao[key] == nowQihao)
            {
                //_logger.LogDebug($"{logPrefix} {nowQihao}期不是最新期号.");
                return;
            }

            // 更新当前计划-玩法的最新期号
            _latestQihao[key] = nowQihao;

            //
            // 遍历剩余元素, 计算一字长龙个数
            //
            var count = 0;
            var otherElems = message.ListPlanNo.Skip(1).ToList();
            foreach (var planNo in otherElems)
            {
                if (planNo.Ready != 1) break;
                count++;
            }


            count = count - 1;
            if (count >= _config.MinLongQueue)
            {
                var targetElem = otherElems[1];

                var logInfo = $"{logPrefix}，一字长龙： {count.ToString()}, 预测 {nowQihao} 期号码: {targetElem.Number}";
                _logger.LogInformation(logInfo);

                // TODO 如果一字长龙次数达到设定, 则通知
            }
        }
    }
}