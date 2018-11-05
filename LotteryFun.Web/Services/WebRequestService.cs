using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LotteryFun.Web.Utils;
using RestSharp;

namespace LotteryFun.Web.Services
{
    public interface IWebRequestService
    {
        Task<T> Get<T>(string relativeUrl, IDictionary<string, string> param) where T : new();

        string GetString(string relativeUrl, IDictionary<string, string> param = null);
        string Post(string relativeUrl, IDictionary<string, string> param = null);
    }

    /// <summary>
    ///     Http 请求处理
    /// </summary>
    public class WebRequestService : IWebRequestService
    {
        //private const string FormUrlEncoded = "application/x-www-form-urlencoded";
        private const string UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36";

        private const string AcceptLanguage = "zh-CN,zh;q=0.9,en;q=0.8,zh-TW;q=0.7,en-US;q=0.6";

        private readonly RestClient _client;

        public WebRequestService(string domain)
        {
            _client = new RestClient(domain);
            _client.Timeout = 60 * 1000;
            _client.UserAgent = UserAgent;
            _client.AddDefaultHeader("Accept-Language", AcceptLanguage);
            _client.CookieContainer = new CookieContainer();
            _client.Encoding = Encoding.UTF8;
        }

        #region Public Methods

        public async Task<T> Get<T>(string relativeUrl, IDictionary<string, string> param = null) where T : new()
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

            var newContent = StringExtension.ToUtf8WithoutBom(response.RawBytes)?.UnicodeDencode();
            return newContent;
        }

        public string Post(string relativeUrl, IDictionary<string, string> param = null)
        {
            var request = new RestRequest(relativeUrl, Method.POST);
            AddParameter(request, param);

            var response = _client.Post(request);

            var newContent = StringExtension.ToUtf8WithoutBom(response.RawBytes)?.UnicodeDencode();
            return newContent;
        }

        #endregion Public Methods

        #region Private Methods

        private void AddParameter(IRestRequest request, IDictionary<string, string> param)
        {
            if (param == null) return;

            foreach (var kv in param)
            {
                request.AddQueryParameter(kv.Key, kv.Value);
            }
        }

        #endregion Private Methods
    }
}