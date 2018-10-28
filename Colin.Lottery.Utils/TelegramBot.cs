using System.Threading.Tasks;

namespace Colin.Lottery.Utils
{
    public static class TelegramBot
    {
        private static readonly string Url;
        private static readonly dynamic Content;

        static TelegramBot()
        {
            Url = $"https://api.telegram.org/bot{ConfigUtil.Configuration["TelegramBot:Token"]}/sendMessage";
            Content = new
            {
                chat_id = -223016500,
                parse_mode = "HTML",
                disable_web_page_preview = false,
                disable_notification = false
            };
        }

        public static async Task SendMessageAsync(string msg)
        {
            Content.text = msg;
            await HttpUtil.PostAsync(Url, Content);
        }
    }
}