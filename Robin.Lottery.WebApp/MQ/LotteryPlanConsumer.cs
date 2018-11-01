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

        private readonly IDictionary<string, string> _latestQihao
            = new Dictionary<string, string>();

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

            if (message.ListPlanNo == null || message.ListPlanNo.Count() <= 3)
            {
                _logger.LogWarning($"{logPrefix} 计划号码为空, 订阅者停止本次任务.");
                return;
            }

            var listPlanNo = message.ListPlanNo.Reverse().ToList();
            var latestQihao = listPlanNo.First().Qihao;

            //
            // 判断是否最新期号.
            // 如果不是，则返回
            //
            var key = $"{planner}_{rule}";
            if (_latestQihao.ContainsKey(key) && _latestQihao[key]==latestQihao)
            {
                _logger.LogDebug($"{logPrefix} 不是最新期号.");
                return;
            }
            
            _latestQihao[key] = latestQihao;    // 更新当前计划-玩法的最新期号

            var second = listPlanNo.Skip(1).First();

            var count = 0;
            foreach (var planNo in listPlanNo)
            {
                if (planNo.Ready != 1) break;

                count++;
            }

            count = count - 1; // 减去未开奖的一期
            if (count >= _config.MinLongQueue)
            {
                var info = $"{logPrefix}，{latestQihao}期, 一字长龙： {count.ToString()}";
                _logger.LogInformation(info);
            }
        }
    }
}