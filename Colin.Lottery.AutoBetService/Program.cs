using System;
using Colin.Lottery.Models;
using Colin.Lottery.Utils;

namespace Colin.Lottery.AutoBetService
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            Test();
            Console.WriteLine("OK");
            Console.ReadKey();
        }

        static async void Test()
        {
            await MailUtil.MailAsync("zhangcheng5468@163.com",
            "zhangcheng5468@163.com",
            "TestSending",
            "<h1 style='color:red'>测试内容1</h1>",
            MailContentType.Html,
            "smtp.163.com",
            465,
            "zhangcheng5468@163.com",
            "xinzhe&468163"
            );
        }
    }
}
