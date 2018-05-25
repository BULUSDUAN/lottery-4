using System;
using System.Linq;
using Colin.Lottery.Models;
using Colin.Lottery.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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

        static void Test()
        {
            var options = new ChromeOptions();
            options.AddArguments("headless", "disable-gpu", "remote-debugging-port=9222", "window-size=1440,900", "disable-infobars", "--disable-extensions");

            using (var chrome = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory, options))
            {
                chrome.Navigate().GoToUrl("http://kj.kai861.com/view/pk10.html?1?10001?null?d?www.1680210.com");
                var curPeriod = chrome.FindElement(By.CssSelector("#preDrawIssue em")).Text;
                var drawNo = string.Join(',', chrome.FindElements(By.CssSelector("#preDrawIssue li i")).Select(i => i.Text));
                var nextPeriod = chrome.FindElement(By.CssSelector("#drawIssue em")).Text;
                var leftTime = chrome.FindElement(By.CssSelector(".ntime em")).Text;

            }
        }
    }
}
