using System;
using System.Net;

namespace LotteryFun.Web.Exceptions
{
    /// <summary>
    ///     Web 请求过程中抛出的异常
    /// </summary>
    public class WebRequestException : ApplicationException
    {
        public WebRequestException(string url, HttpStatusCode statusCode, string responseContent)
            : base($"请求 url {url} 时发生错误, 响应状态码: {(int) statusCode}")
        {
            ResponseContent = responseContent;
        }

        public string ResponseContent { get; }
    }
}