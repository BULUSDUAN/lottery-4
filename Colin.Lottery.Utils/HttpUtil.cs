using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Colin.Lottery.Utils
{
    public static class HttpUtil
    {
        public static async Task<string> GetAsync(string url)
        {
            return await RequestAsync(url, null, "get", TimeSpan.FromMinutes(5));
        }

        public static async Task<string> PostAsync(string url, object parameter)
        {
            return await RequestAsync(url, parameter, "post", TimeSpan.FromMinutes(1));
        }

        private static async Task<string> RequestAsync(string url, object parameter, string method, TimeSpan? timeout)
        {
            using (var hc = new HttpClient())
            {
                if (timeout != null)
                    hc.Timeout = (TimeSpan)timeout;

                HttpResponseMessage response;
                try
                {
                    if (string.Equals(method, "get", StringComparison.OrdinalIgnoreCase))
                        response = await hc.GetAsync(url);
                    else if (string.Equals(method, "post", StringComparison.OrdinalIgnoreCase))
                    {
                        var content = new StringContent(JsonConvert.SerializeObject(parameter), Encoding.UTF8,
                            "application/json");
                        response = await hc.PostAsync(url, content);
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
}