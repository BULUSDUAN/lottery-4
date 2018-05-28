using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Colin.Lottery.Models;
using Colin.Lottery.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.UI;

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

        //static async void Test()
        //{
        //    await MailUtil.MailAsync("zhangcheng5468@163.com",
        //    "zhangcheng5468@163.com",
        //    "TestSending",
        //    "<h1 style='color:red'>测试内容1</h1>",
        //    MailContentType.Html,
        //    "smtp.163.com",
        //    465,
        //    "zhangcheng5468@163.com",
        //    "xinzhe&468163"
        //    );
        //}

        static async Task Test()
        {
            //var watch = new Stopwatch();
            //watch.Start();

            //var options = new ChromeOptions();
            //options.AddArguments("headless", "disable-gpu", "disable-infobars", "--disable-extensions");
            //using (var chrome = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory, options))
            //{
            //    chrome.Navigate().GoToUrl("https://tool.ssrshare.com/tool/free_ssr");

            //    var inputs = chrome.FindElements(By.CssSelector(".mdui-textfield-input"));
            //    foreach (var input in inputs)
            //    {
            //        Console.WriteLine(input.GetAttribute("value"));
            //    }
            //}

            //watch.Stop();
            //Console.WriteLine(watch.ElapsedMilliseconds);

            var brower = new BrowserUtil();
            await brower.Explore("https://ssrshare.xyz/freessr");
            brower.Completed += (brw, args) =>
            {
                Console.WriteLine(DateTime.Now);
                //Console.WriteLine(args.PageSource);
            };
        }
    }
}
