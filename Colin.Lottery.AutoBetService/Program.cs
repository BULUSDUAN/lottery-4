using System;
using Colin.Lottery.Models.Notification;
using Colin.Lottery.Utils;

namespace Colin.Lottery.AutoBetService
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();

            Console.WriteLine("OK");

            Console.ReadKey();
        }

        private static async void Test()
        {
            var config = ConfigUtil.GetAppSettings<MailNotifyConfig>("MailNotify");
            SendGridConfig sendgridApiKey = ConfigUtil.GetAppSettings<SendGridConfig>("SendGridConfig");

            await MailUtil.MailAsync(
                sendgridApiKey.ApiKey, config.From, config.To.Split(','), config.Subject, config.Content, config.ContentType);
            //Console.WriteLine($"邮件已发送，状态 : {respone.StatusCode} ");
        }
    }
}
