using System;
using System.Dynamic;
using System.Threading.Tasks;
using Colin.Lottery.Models.Notification;
using Newtonsoft.Json;

namespace Colin.Lottery.Utils
{
    public static class TelegramBotUtil
    {
        private static readonly string Url;
        private static readonly dynamic Content;

        static TelegramBotUtil()
        {
            Url = $"https://api.telegram.org/bot{ConfigUtil.Configuration["TelegramBot:Token"]}/sendMessage";
            Content = new ExpandoObject();
            Content.chat_id = ConfigUtil.Configuration["TelegramBot:ChatId"];
            Content.parse_mode = "HTML";
            Content.disable_web_page_preview = false;
            Content.disable_notification = false;
        }

        public static async Task<bool> SendMessageAsync(string msg)
        {
            Content.text = msg;
            string result = await HttpUtil.PostJsonAsync(Url, Content);
            if (string.IsNullOrWhiteSpace(result))
                return false;

            try
            {
                var response = JsonConvert.DeserializeObject<TelegramBotResponse>(result);
                if (response.OK)
                    return true;

                ExceptionlessUtil.Warn(new Exception(response.Description), "TG消息推送失败");
                return false;
            }
            catch (Exception ex)
            {
                ExceptionlessUtil.Warn(ex, "TG消息推送失败");
                return false;
            }
        }
    }
}