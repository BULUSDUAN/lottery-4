using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg;
using RestSharp;

namespace Colin.Lottery.Utils
{
    public class RestClientHelper
    {
        private const string UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36";

        private const string AcceptLanguage = "zh-CN,zh;q=0.9,en;q=0.8,zh-TW;q=0.7,en-US;q=0.6";
        private const string Accept = "application/json, text/plain, */*";

        private RestClient _client;

        /// <summary>
        /// 初始化一个 RestClientHelper 对象
        /// </summary>
        /// <param name="domain">域名</param>
        public RestClientHelper(string domain)
        {
            _client = new RestClient(domain);
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
            request.AddHeader("User-Agent", UserAgent);
            request.AddHeader("Accept-Language", AcceptLanguage);
            request.AddHeader("Accept", Accept);

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
        public string Post(string relativeUrl, Dictionary<string, object> parameters = null)
        {
            var request =
                new RestRequest(relativeUrl, Method.POST).AddParameters(parameters);
            request.AddHeader("User-Agent", UserAgent);
            request.AddHeader("Accept-Language", AcceptLanguage);
            request.AddHeader("Accept", Accept);

//            request.RequestFormat=DataFormat.

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
            T data = JsonConvert.DeserializeObject<T>(content);
            return data;
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
                request.AddHeader("content-type", "application/x-www-form-urlencoded");

                foreach (var parameter in parameters)
                {
//                    if (parameter.Value is string)
//                    {
                    request.AddParameter(parameter.Key, parameter.Value, parameterType);
//                    }
//                    else if (parameter.Value is IList values)
//                    {
//                        for (int idx = 0; idx < values.Count; idx++)
//                        {
//                            request.AddParameter($"{parameter.Key}[{idx}]", values[idx], parameterType);
//                        }
//                    }
                }
            }

            return request;
        }
    }
}