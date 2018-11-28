using System;
using System.Diagnostics;
using System.Security;
using System.Threading.Tasks;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Colin.Lottery.Models;
using Colin.Lottery.Models.BrowserModels;

namespace Colin.Lottery.Utils
{
    public class BrowserUtil
    {
        private readonly ChromeDriver _chromeDriver;

        public BrowserUtil()
            : this(null)
        {
        }

        public BrowserUtil(Proxy proxy)
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless", "disable-gpu", "disable-infobars", "--disable-extensions","--remote-debugging-port=9222");
            if (proxy != null)
                chromeOptions.Proxy = proxy;

            _chromeDriver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory, chromeOptions);
        }

        /// <summary>
        /// 开始访问前
        /// </summary>
        public event EventHandler<ExploreStartingEventArgs> Starting;

        /// <summary>
        /// 访问结束后
        /// </summary>
        public event EventHandler<ExploreCompleteEventArgs> Completed;

        /// <summary>
        /// 使用Headless Chrome访问给定地址
        /// </summary>
        /// <param name="url">目标地址</param>
        /// <param name="operation">请求URL后等待完成的业务处理操作，如等待异步数据渲染到UI</param>
        /// <param name="script">执行JavaScript脚本(operation完成后执行)</param>
        public async Task Explore(string url, Operation operation, Script script)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            await Task.Run(() =>
            {
                var watch = new Stopwatch();
                watch.Start();

                Starting?.Invoke(this, new ExploreStartingEventArgs(url));
                try
                {
                    //请求URL
                    _chromeDriver.Navigate().GoToUrl(url);

                    //执行JavaScript
                    if (script != null)
                        _chromeDriver.ExecuteScript(script.Code, script.Args);

                    //返回条件和超时时间
                    if (operation?.Condition != null)
                    {
                        var driverWait =new WebDriverWait(_chromeDriver, TimeSpan.FromMilliseconds(operation.Timeout));
                        driverWait.Until(driver =>operation.Condition.Invoke(driver));
                        operation.Action?.Invoke(_chromeDriver);
                    }

                    watch.Stop();

                    var threadId = Thread.CurrentThread.ManagedThreadId;
                    var elapsed = watch.ElapsedMilliseconds;
                    var pageSource = _chromeDriver.PageSource;

                    Completed?.Invoke(this,
                        new ExploreCompleteEventArgs(url, _chromeDriver, threadId, elapsed, pageSource));
                }
                catch (Exception ex)
                {
                    ExceptionlessUtil.Warn(ex,$"Browser访问{url}出错");
                }
            });
        }

        public async Task Explore(string url, Operation operation)
        {
            await Explore(url, operation, null);
        }

        public async Task Explore(string url, Script script)
        {
            await Explore(url, null, script);
        }

        public async Task Explore(string url)
        {
            await Explore(url, null, null);
        }

        public void ExecuteScript(Script script)
        {
            //执行JavaScript
            if (script != null)
                _chromeDriver.ExecuteScript(script.Code, script.Args);
        }

        public void Quit()
        {
            _chromeDriver.Quit();
        }
    }
}