using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LotteryFun.Web.Models;
using LotteryFun.Web.Services;
using LotteryFun.Web.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LotteryFun.Web.MessageService
{
    /// <summary>
    /// Telegram Bot Service
    /// </summary>
    public class TelegramBotService : IMessageService
    {
        private readonly ILogger<TelegramBotService> _logger;
        private readonly AppConfig _config;
        private readonly ITelegramBotClient _botClient;

        public TelegramBotService(ITelegramBotClient telegramBotClient,
            ILogger<TelegramBotService> logger,
            AppConfig config)
        {
            _logger = logger;
            _config = config;

            _botClient = telegramBotClient;

            _logger.LogInformation("Telegram Bot 已经激活.");
        }


        public async Task Send(string text)
        {
            var param = new Dictionary<string, string>
            {
                {"chat_id", _config.TgChatId},
                {"text", text}
            };

            Message messageResult = await _botClient.SendTextMessageAsync(_config.TgChatId, text, ParseMode.Html);
            if (messageResult.MessageId==0)
            {
                _logger.LogWarning($"TelegramBot 发送消息失败，因为 MessageId = 0.");
            }


            await TaskUtils.CompletedTask;
        }
    }
}