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

        public LotteryPlanConsumer(IBus bus, ILogger<LotteryPlanConsumer> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        public void Subscribe()
        {
            ISubscriptionResult result = _bus.Subscribe<LotteryPlanMessage>("LotteryPlanConsumer", Handler);
            _logger.LogInformation($"已开始订阅{nameof(LotteryPlanMessage)}消息.");
        }

        private void Handler(LotteryPlanMessage message)
        {
            var planner = message.Planner;
            var rule = message.Pk10Rule;

            string logPrefix = $"「{planner}计划 - {rule.GetDescription()}」";

            if (message.ListPlanNo == null || message.ListPlanNo.Count() <= 3)
            {
                _logger.LogDebug($"{logPrefix} 计划号码为空, 订阅者停止本次任务.");
                return;
            }

            var listPlanNo = message.ListPlanNo.Reverse().ToList();
            var first = listPlanNo.Skip(1).First();

            var count = 0;
            foreach (var planNo in listPlanNo)
            {
                if (planNo.Ready != 1) break;

                count++;
            }

            count = count - 1; // 减去未开奖的一期
            if (count >= 3)
            {
                var info = $"{logPrefix}，{first.Qihao}期, 一字长龙： {count.ToString()}";
                _logger.LogInformation(info);
            }
        }
    }
}