using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using LotteryFun.Web.MessageService;
using LotteryFun.Web.Models;
using LotteryFun.Web.Utils;
using Microsoft.Extensions.Logging;

namespace LotteryFun.Web.MQ
{
    /// <summary>
    ///     计划号码的消费者
    /// </summary>
    public class LotteryPlanConsumer
    {
        private readonly IBus _bus;
        private readonly ILogger<LotteryPlanConsumer> _logger;
        private readonly AppConfig _config;
        private readonly IMessageService _botService;

        private readonly IDictionary<string, int> _latestQihao
            = new ConcurrentDictionary<string, int>();

        public LotteryPlanConsumer(IBus bus, ILogger<LotteryPlanConsumer> logger,
            AppConfig config, IMessageService botService)
        {
            _bus = bus;
            _logger = logger;
            _config = config;
            _botService = botService;
        }

        public void Subscribe()
        {
            _bus.SubscribeAsync<LotteryPlanMessage>("LotteryPlanConsumer", Handler);

            _logger.LogInformation($"已开始订阅 {nameof(LotteryPlanMessage)} 消息.");
        }

        private Task Handler(LotteryPlanMessage message)
        {
            var planner = message.Planner;
            var rule = message.Pk10Rule;

            string logPrefix = $"「{rule.GetDescription()}-{planner}」";

            const int minElemCount = 4;
            if (message.ListPlanNo == null || message.ListPlanNo.Count <= minElemCount)
            {
                _logger.LogWarning($"{logPrefix} 计划号码为空, 或历史追号小于 {minElemCount} 段, 停止本次任务.");
                return TaskUtils.CompletedTask;
            }

            var first = message.ListPlanNo.First();
            var nowQihao = first.NowQihao.Value + 1; // 下一期期号
            if (nowQihao == 0)
            {
                _logger.LogWarning($"{logPrefix} 最新期号为0, 订阅者停止本次任务.");
                return TaskUtils.CompletedTask;
            }

            //
            // 判断是否最新期号. 
            // 如果不是，则返回
            //
            var key = $"{planner}_{rule}";
            if (_latestQihao.ContainsKey(key) && _latestQihao[key] == nowQihao)
            {
                return TaskUtils.CompletedTask;
            }

            // 更新当前计划-玩法的最新期号
            _latestQihao[key] = nowQihao;


            var otherElems = message.ListPlanNo.Skip(1).ToList();
            var nextNumber = otherElems.First().Number;

            //
            // 遍历剩余元素, 计算一字长龙个数
            //
            var dragonCount = -1;
            foreach (var planNo in otherElems)
            {
                if (planNo.Ready != 1) break;
                dragonCount++;
            }

            var msgFormat = "{0}{1} 期: [{2}], {3}: {4}]";
            var msg = string.Empty;

            msg = string.Format(msgFormat, logPrefix, nowQihao, nextNumber, "一字长龙", dragonCount.ToString());
            SendNotification(dragonCount, _config.MinLongQueue, msg);


            //
            // 遍历剩余元素, 计算连挂个数
            //
            var guaCount = 0;
            foreach (var planNo in otherElems.Skip(1))
            {
                if (planNo.Ready != 3 || planNo.Resalt != "挂") break;
                guaCount++;
            }

//            msg = $"{logPrefix}{nowQihao} 期: [{nextNumber}], 连挂: [{guaCount.ToString()}]";
            msg = string.Format(msgFormat, logPrefix, nowQihao, nextNumber, "连挂", guaCount.ToString());
            SendNotification(guaCount, _config.MinGuaQueue, msg);

            return Task.CompletedTask;
        }

        private void SendNotification(int count, int configCount, string msg)
        {
            if (count >= configCount)
            {
                _logger.LogInformation(msg);

                _botService.Send(msg);
            }
        }
    }
}