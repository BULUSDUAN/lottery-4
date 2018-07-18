using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Colin.Lottery.FormApp
{
    public static class WebBrowerExtension
    {
        /// <summary>
        /// 监听某个条件满足后(如UI元素渲染完毕)执行指定操作(多线程)
        /// </summary>
        /// <param name="condition">待满足条件(UI资源需满足跨线程访问)</param>
        /// <param name="operation">待执行操作((UI资源需满足跨线程访问))</param>
        /// <param name="timeOut">等待条件满足的超时时间</param>
        public static void WaitUtil(this WebBrowser brower, Predicate<WebBrowser> condition, Action<WebBrowser> operation, TimeSpan timeOut)
        {
            new Thread(() =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                while (true)
                {
                    //超时
                    if (watch.Elapsed.TotalMilliseconds > timeOut.TotalMilliseconds)
                    {
                        watch.Stop();
                        break;
                    }

                    if (condition(brower))
                    {
                        operation(brower);
                        watch.Stop();
                        break;
                    }
                    Thread.Sleep(1000);
                }
            })
            { IsBackground = true }.Start();
        }


        /// <summary>
        /// 执行附加脚本
        /// </summary>
        /// <param name="browser">浏览器对象</param>
        /// <param name="script">待执行脚本</param>
        /// <param name="canBeCleared">脚本内容是否可以被清除(定时任务应设置为false)</param>
        public static void ExecuteScript(this WebBrowser browser, string script, bool canBeCleared = true)
        {
            var body = browser.Document.GetElementsByTagName("body")[0];
            var newScript = browser.Document.CreateElement("script");
            newScript.SetAttribute("canBeCleared", "true");
            newScript.InnerHtml = $"(function(){{$('[canBeCleared=true]').remove();{script}}})();";
            body.AppendChild(newScript);
        }
    }
}
