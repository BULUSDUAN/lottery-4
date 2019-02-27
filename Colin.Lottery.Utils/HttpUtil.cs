using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Colin.Lottery.Utils
{
    public static class HttpUtil
    {
        public static async Task<string> GetAsync(string url)
        {
            var response = await RequestAsync(url);
            return response.StringContent;
        }

        public static async Task<Response> GetAsync(string url, object _)
        {
            return await RequestAsync(url, HttpMethod.Get);
        }


        public static async Task<Response> PostJsonAsync(string url, object content)
        {
            return await PostJsonAsync(url, JsonConvert.SerializeObject(content));
        }

        public static async Task<Response> PostJsonAsync(string url, string json)
        {
            var requestContent =
                new StringContent(json, Encoding.UTF8, "application/json");
            // requestContent.Headers.ContentType=MediaTypeHeaderValue.Parse("application/json");

            return await RequestAsync(url, HttpMethod.Post, requestContent);
        }

        public static async Task<Response> PostFormAsync(string url, Dictionary<string, string> formContent)
        {
            return await RequestAsync(url, HttpMethod.Post, new FormUrlEncodedContent(formContent));
        }

        public static async Task<Response> PostFileAsync(string url, string file,
            Dictionary<string, string> formContent = null)
        {
            if (!File.Exists(file))
                return await PostFormAsync(url, formContent);

            var content = new MultipartContent();
            if (formContent != null && formContent.Count > 0)
            {
                foreach (var name in formContent.Keys)
                    content.Headers.Add(name, formContent[name]);
            }

            using (var stream = File.OpenRead(file))
            {
                content.Add(new StreamContent(stream));
                return await RequestAsync(url, HttpMethod.Post, content);
            }
        }


        private static async Task<Response> RequestAsync(string url, HttpMethod method = null,
            HttpContent content = null)
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
                        if ((method ?? HttpMethod.Get) == HttpMethod.Get)
                            response = await http.GetAsync(url);
                        else if (method == HttpMethod.Post)
                            response = await http.PostAsync(url, content);
                        else
                            throw new ArgumentException($"暂不支持{method}请求方式");

                        response.EnsureSuccessStatusCode();
                        return new Response(response.StatusCode, response.Headers, response.Content);
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

    /// <summary>
    /// HTTP请求响应对象
    /// </summary>
    public class Response
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// 响应报文头
        /// </summary>
        public HttpResponseHeaders Headers { get; set; }

        /// <summary>
        /// 响应正文
        /// </summary>
        public HttpContent Content
        {
            set => ReadContent(value);
        }

        /// <summary>
        /// 响应正文-字符串格式
        /// </summary>
        public string StringContent { get; private set; }

        /// <summary>
        /// 响应正文-流格式
        /// </summary>
        public Stream StreamContent { get; private set; }

        /// <summary>
        /// 响应正文-字节数组格式
        /// </summary>
        public byte[] ByteArrayContent { get; private set; }

        public Response(HttpStatusCode statusCode, HttpResponseHeaders headers, HttpContent content)
        {
            StatusCode = statusCode;
            Headers = headers;
            Content = content;
        }

        private async void ReadContent(HttpContent content)
        {
            using (content)
            {
                StringContent = await content.ReadAsStringAsync();
                StreamContent = await content.ReadAsStreamAsync();
                ByteArrayContent = await content.ReadAsByteArrayAsync();
            }
        }
    }
}