using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Colin.Lottery.Utils
{
    public static class HttpUtil
    {
        public async static Task<string> GetStringAsync(string url)
        {
            var hc = new HttpClient();
            try
            {
                var response = await hc.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                LogUtil.Error($"HttpUtil.GetStringAsync失败。{ex.Message}\r\n{ex.StackTrace}");
                return null;
            }
        }
    }
}
