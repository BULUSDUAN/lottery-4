using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colin.Lottery.Analyzers;
using Colin.Lottery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;

namespace Colin.Lottery.WebApp.Pages
{
    public class AppModel : PageModel
    {
        private IMemoryCache _cache;

        public AppModel(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public async Task<IActionResult> OnGetAppPlansAsync(int lottery = 0, int rule = 0)
        {
            var forecasts = new List<IForecastModel>();

            if (_cache.TryGetValue<ConcurrentDictionary<int, List<IForecastPlanModel>>>((LotteryType) lottery,
                out var ps))
            {
                var endRule = rule > 0 ? Pk10Rule.DragonOrTiger : Pk10Rule.Fourth;
                for (var i = 1; i <= (int) endRule; i++)
                {
                    if (!ps.TryGetValue(i, out var plans))
                        continue;

                    plans.ForEach(p => forecasts.Add(p.ForecastData.LastOrDefault()));
                }
            }

            return Content(JsonConvert.SerializeObject(forecasts));
        }

        public async Task<IActionResult> OnGetAppPlanDetailsAsync(int lottery = 0, int rule = 1)
        {
            List<IForecastPlanModel> plans = null;

            if (_cache.TryGetValue<ConcurrentDictionary<int, List<IForecastPlanModel>>>((LotteryType) lottery,
                out var ps))
                ps.TryGetValue(rule, out plans);

            return Content(JsonConvert.SerializeObject(plans ?? new List<IForecastPlanModel>()));
        }
    }
}