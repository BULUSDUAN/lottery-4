using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Robin.Lottery.WebApp.Utils
{
    public interface IWebClient
    {
        Task<T> Get<T>(string relativeUrl, IDictionary<string, string> param) where T : new();
        string GetString(string relativeUrl, IDictionary<string, string> param);
    }

    /// <summary>
    /// Http 请求处理
    /// </summary>
    public class WebClient : IWebClient
    {
        private readonly RestClient _client;
        private const string FormUrlEncoded = "application/x-www-form-urlencoded";

        private const string UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36";

        private const string AcceptLanguage = "zh-CN,zh;q=0.9,en;q=0.8,zh-TW;q=0.7,en-US;q=0.6";


        public WebClient(string domain)
        {
            _client = new RestClient(domain);
            _client.Timeout = 5 * 60 * 1000;
            _client.UserAgent = UserAgent;
            _client.AddDefaultHeader("Accept-Language", AcceptLanguage);
            _client.CookieContainer = new CookieContainer();
            _client.Encoding = Encoding.UTF8;
        }

        public async Task<T> Get<T>(string relativeUrl, IDictionary<string, string> param) where T : new()
        {
            var request = new RestRequest(relativeUrl);
            AddParameter(request, param);

            return await _client.GetAsync<T>(request);
        }

        public string GetString(string relativeUrl, IDictionary<string, string> param)
        {
            var request = new RestRequest(relativeUrl);
            AddParameter(request, param);

            var response = _client.Get(request);

            // 转换 Unicode \u51a0\u519b\u5355\u53cc

            string newContent = response.Content.UnicodeDencode();
            return newContent;
        }


        #region Private Methods

        private void AddParameter(IRestRequest request, IDictionary<string, string> param)
        {
            if (param != null)
            {
                foreach (var kv in param)
                {
                    request.AddQueryParameter(kv.Key, kv.Value);
                }
            }
        }

        #endregion
    }
}