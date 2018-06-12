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

             await MailUtil.MailAsync(config.ApiKey, config.From, config.To.Split(','), config.Subject, "123123", config.ContentType);
        }
    }
}
