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
            return await RequestAsync(url, null, "get");
        }

        public static async Task<string> PostAsync(string url, object parameter)
        {
            return await RequestAsync(url, parameter, "post");
        }

        private static async Task<string> RequestAsync(string url, object parameter, string method)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => true;

                using (var http = HttpClientFactory.Create(handler))
                {
                    try
                    {
                        HttpResponseMessage response;
                        if (string.Equals(method, "get", StringComparison.OrdinalIgnoreCase))
                            response = await http.GetAsync(url);
                        else if (string.Equals(method, "post", StringComparison.OrdinalIgnoreCase))
                        {
                            var content = new StringContent(JsonConvert.SerializeObject(parameter), Encoding.UTF8,
                                "application/json");
                            response = await http.PostAsync(url, content);
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
                        ExceptionlessUtil.Warn(ex, $"网络请求出错{url}");
                        return null;
                    }
                }
            }
        }
    }
}