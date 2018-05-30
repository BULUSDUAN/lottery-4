using OpenQA.Selenium.Remote;

namespace Colin.Lottery.Models.BrowserModels
{
    public class ExploreCompleteEventArgs
    {
        /// <summary>
        /// 目标地址
        /// </summary>
        public string Url { get; }

        public RemoteWebDriver WebBrower { get; }

        /// <summary>
        /// 线程ID
        /// </summary>
        public int ThreadId { get; }

        /// <summary>
        /// 页面源代码
        /// </summary>
        public string PageSource { get; }

        /// <summary>
        /// 请求执行时间
        /// </summary>
        public long ElapsedMilliseconds { get; }

        public ExploreCompleteEventArgs(string url, RemoteWebDriver webDriver, int threadId, long elapsedMilliseconds, string pageSource)
        {
            Url = url;
            WebBrower = webDriver;
            ThreadId = threadId;
            ElapsedMilliseconds = elapsedMilliseconds;
            PageSource = pageSource;
        }
    }
}
