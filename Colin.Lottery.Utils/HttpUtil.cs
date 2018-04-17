using System.Net.Http;
using System.Threading.Tasks;

namespace Colin.Lottery.Utils
{
    public class HttpUtil
    {
        public async static Task<string> GetStringAsync(string url)
        {
            HttpClient hc = new HttpClient();
            var response = await hc.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
