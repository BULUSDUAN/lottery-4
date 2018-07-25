using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace Colin.Lottery.Utils
{
    public class RestClientHelper
    {
        private const string UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36";

        private const string AcceptLanguage = "zh-CN,zh;q=0.9,en;q=0.8,zh-TW;q=0.7,en-US;q=0.6";

        private RestClient _client;

        /// <summary>
        /// 初始化一个 RestClientHelper 对象
        /// </summary>
        /// <param name="domain">域名</param>
        public RestClientHelper(string domain)
        {
            _client = new RestClient(domain);

            _client.UserAgent = UserAgent;
            _client.CookieContainer = new CookieContainer();
            _client.AddDefaultHeader("Accept-Language", AcceptLanguage);

            // 若使用Fiddler抓包，此处需要设置代理为Fiddler的监听端口
            _client.Proxy = new WebProxy("127.0.0.1", 8888);
        }

        /// <summary>
        /// 提交 GET 请求
        /// </summary>
        /// <param name="relativeUrl">相对路径</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public string Get(string relativeUrl, Dictionary<string, object> parameters = null)
        {
            var request = new RestRequest(relativeUrl, Method.GET).AddParameters(parameters);

            IRestResponse respone = _client.Execute(request);
            string content = respone.Content;
            return content;
        }

        /// <summary>
        /// 提交 GET 请求
        /// </summary>
        /// <param name="relativeUrl">相对路径</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public T Get<T>(string relativeUrl, Dictionary<string, object> parameters = null) where T : class, new()
        {
            string content = Get(relativeUrl, parameters);
            T result = JsonConvert.DeserializeObject<T>(content);
            return result;
        }

        /// <summary>
        /// 提交 POST 请求
        /// </summary>
        /// <param name="relativeUrl">请求 url 的相对路径</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public string Post(string relativeUrl, Dictionary<string, object> parameters = null)
        {
            var request =
                new RestRequest(relativeUrl, Method.POST).AddParameters(parameters);

            IRestResponse respone = _client.Execute(request);

            string content = respone.Content;
            return content;
        }

        /// <summary>
        /// 提交 POST 请求
        /// </summary>
        /// <param name="relativeUrl">请求 url 的相对路径</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public T Post<T>(string relativeUrl, Dictionary<string, object> parameters = null) where T : class, new()
        {
            string content = Post(relativeUrl, parameters);
            T result = JsonConvert.DeserializeObject<T>(content);
            return result;
        }

        public T Post<T>(string relativeUrl, string postData) where T : class, new()
        {
            var request = new RestRequest(relativeUrl, Method.POST);

            request.AddParameter("application/x-www-form-urlencoded", WebUtility.HtmlDecode(postData), ParameterType.RequestBody);

            IRestResponse respone = _client.Execute(request);

            string content = respone.Content;
            T result = JsonConvert.DeserializeObject<T>(content);

            return result;
        }
    }

    public static class RestRequestExtensions
    {
        public static RestRequest AddParameters(this RestRequest request,
            Dictionary<string, object> parameters,
            ParameterType parameterType = ParameterType.QueryString)
        {
            if (parameters != null && parameters.Count > 0)
            {
                foreach (var parameter in parameters)
                {
                    request.AddParameter(parameter.Key, parameter.Value, parameterType);
                }
            }

            return request;
        }
    }
}