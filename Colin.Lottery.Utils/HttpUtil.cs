using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Colin.Lottery.Utils
{
    public static class HttpUtil
    {
        public async static Task<string> GetStringAsync(string url)
        {
            return await GetStringAsync(url, null);
        }

        public async static Task<string> GetStringAsync(string url, TimeSpan? timeout)
        {
            var hc = new HttpClient();
            if (timeout != null)
                hc.Timeout = (TimeSpan)timeout;
            try
            {
                var response = await hc.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                LogUtil.Warn($"{ex.Message}\r\n{ex.StackTrace}");
                return null;
            }
        }
    }
}
