using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colin.Lottery.DataService;
using Colin.Lottery.Models;
using Colin.Lottery.Utils;
using Colin.Lottery.WebApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Colin.Lottery.WebApp.Helpers
{
    public class JinMaHelper
    {
        private readonly IHubContext<PK10Hub> _pk10Context;
        private readonly IMemoryCache _cache;
        private readonly IDataService _service;

        private readonly int _minKeepGua;
//        private readonly int MinRepetition;

        public JinMaHelper(IMemoryCache cache, IDataService service, IHubContext<PK10Hub> pk10Context)
        {
            _pk10Context = pk10Context;
            _cache = cache;
            _service = service;

            _minKeepGua = Convert.ToInt32(ConfigUtil.Configuration["TelegramBot:MinKeepGua"]);
//            MinRepetition = Convert.ToInt32(ConfigUtil.Configuration["TelegramBot:MinRepetition"]);
        }

        /// <summary>
        /// 启动 金马自动扫水/通知服务
        /// </summary>
        public async Task StartService()
        {
            _service.DataCollectedSuccess += Service_DataCollectedSuccess;
            _service.DataCollectedError += Service_DataCollectedError;
            await _service.Start();
        }

        private async void Service_DataCollectedSuccess(object sender, DataCollectedEventArgs e)
        {
            //推送Web通知
            await PushWebNotification(e);

            //推送App消息
//            await PushAppNotification(e);

            //更新缓存
            UpdateCache(e);
        }


        private void UpdateCache(DataCollectedEventArgs e)
        {
            lock (_cache)
            {
                if (_cache.TryGetValue(e.Lottery, out ConcurrentDictionary<int, List<IForecastPlanModel>> ps))
                    ps[(int) e.Rule] = e.Plans;
                else
                    _cache.Set(e.Lottery,
                        new ConcurrentDictionary<int, List<IForecastPlanModel>>() {[(int) e.Rule] = e.Plans});
            }
        }

        private async Task PushWebNotification(DataCollectedEventArgs e)
        {
            //1.推送完整15期计划
            await _pk10Context.Clients.Group(e.Rule.ToString()).SendAsync("ShowPlans", e.Plans);

            //2.推送最新期计划
            await _pk10Context.Clients.Group("AllRules").SendAsync("ShowPlans", e.LastForecastData);
        }

        private async Task PushAppNotification(DataCollectedEventArgs e)
        {
            if (e.Rule == Pk10Rule.Sum)
                return;

            var plans = e.LastForecastData;
            bool isTwoSide = (int) e.Rule > 4;

            //实时更新
            var realTimeAudience = isTwoSide
                ? new {tag_and = new string[] {"realtime", "twoside"}}
                : (object) new {tag = new string[] {"realtime"}};
            await JPushUtil.PushMessageAsync("PK10最新预测", JsonConvert.SerializeObject(plans), realTimeAudience);

            //连挂提醒
            plans.ForEach(async p =>
            {
                var tags = new List<string>();
                for (var i = 1; i <= 4; i++)
                {
                    if (p.KeepGuaCnt < i)
                        break;

                    tags.Add($"liangua{i}");
                }

                var msg = $"{p.KeepGuaCnt}连挂 {p.LastDrawnPeriod + 1}期 {p.Rule} {p.ForecastNo}";
                if (tags.Any())
                {
                    var audience = isTwoSide
                        ? new {tag = tags, tag_and = new string[] {"twoside"}}
                        : (object) new {tag = tags};
                    await JPushUtil.PushNotificationAsync("PK10连挂提醒", msg, audience);
                }

                if (p.KeepGuaCnt >= _minKeepGua)
                    await TelegramBotUtil.SendMessageAsync(msg);
            });

            //重复度提醒
            if (isTwoSide)
                return;

            var repetition = new List<string>();
            var plan = plans.FirstOrDefault();
            for (var i = 60; i <= 100; i += 20)
            {
                if (plan.RepetitionScore < i)
                    break;

                repetition.Add($"repetition{i}");
            }

            var rMsg =
                $"重{plan.RepetitionScore}% {plan.LastDrawnPeriod + 1}期 {plan.Rule} {plan.ForecastNo}/{plans.LastOrDefault().ForecastNo}";
            if (repetition.Any())
                await JPushUtil.PushNotificationAsync("PK10高重提醒", rMsg, new {tag = repetition});

//            if (plan.RepetitionScore >= MinRepetition)
//                await TelegramBotUtil.SendMessageAsync(rMsg);
        }


        private async void Service_DataCollectedError(object sender, CollectErrorEventArgs e)
        {
            await _pk10Context.Clients.Groups(new List<string> {e.Rule.ToString(), "AllRules"})
                .SendAsync("NoResult", e.Rule.ToStringName());

            ExceptionlessUtil.Warn("目标网站扫水接口异常，请尽快检查恢复");
        }
    }
}