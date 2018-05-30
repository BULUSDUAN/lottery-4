using System;
using OpenQA.Selenium;

namespace Colin.Lottery.Models.BrowserModels
{
    /// <summary>
    /// 对浏览器请求结果的业务操作
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// 要浏览器返回内容进行的处理
        /// </summary>
        public Action<IWebDriver> Action { get; set; }

        /// <summary>
        /// 返回结果的等待条件（如等待Ajax请求数据渲染UI）
        /// </summary>
        public Func<IWebDriver, bool> Condition { get; set; }

        /// <summary>
        /// 等待Condition的超时时间
        /// </summary>
        public int Timeout { get; set; }
    }
}
