using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Colin.Lottery.Utils
{
    public static class HttpUtil
    {
        private static readonly HttpClient HttpClient;

        static HttpUtil()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            HttpClient = new HttpClient(handler);
        }


        public static async Task<string> GetAsync(string url)
        {
            return await RequestAsync(url, null, "get");
        }

        public static async Task<string> PostAsync(string url, object parameter)
        {
            return await RequestAsync(url, parameter, "post");
        }

        private static async Task<string> RequestAsync(string url, object parameter, string method)
        {
            try
            {
                HttpResponseMessage response;
                if (string.Equals(method, "get", StringComparison.OrdinalIgnoreCase))
                    response = await HttpClient.GetAsync(url);
                else if (string.Equals(method, "post", StringComparison.OrdinalIgnoreCase))
                {
                    var content = new StringContent(JsonConvert.SerializeObject(parameter), Encoding.UTF8,
                        "application/json");
                    response = await HttpClient.PostAsync(url, content);
                }
                else
                {
                    throw new ArgumentException($"暂不支持{method}请求方式");
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                LogUtil.Warn($"网络请求出错{url},错误消息:{ex.Message},堆栈信息:{ex.StackTrace}");
                return null;
            }
        }
    }
}