using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Colin.Lottery.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Colin.Lottery.WebApp.Pages
{
    public class AppModel : PageModel
    {
        private IMemoryCache _cache;
        private IHostingEnvironment _host;

        public AppModel(IMemoryCache memoryCache, IHostingEnvironment host)
        {
            _cache = memoryCache;
            _host = host;
        }

        public IActionResult OnGetPlans(int lottery = 0, int rule = 0)
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

        public IActionResult OnGetPlanDetails(int lottery = 0, int rule = 1)
        {
            List<IForecastPlanModel> plans = null;

            if (_cache.TryGetValue<ConcurrentDictionary<int, List<IForecastPlanModel>>>((LotteryType) lottery,
                out var ps))
                ps.TryGetValue(rule, out plans);

            return Content(JsonConvert.SerializeObject(plans ?? new List<IForecastPlanModel>()));
        }

        public IActionResult OnGetDownload(string lottery)
        {
            if (!string.Equals(lottery, "android", StringComparison.OrdinalIgnoreCase))
                return Content("仅支持Android平台下载");

            var virtualPath = "lib/apps/";
            var apk = Directory.GetFiles(Path.Combine(_host.WebRootPath, virtualPath), "*.apk")?.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(apk))
                return NotFound();


            var file = Path.GetFileName(apk);
            return File(Path.Combine(virtualPath, file), "application/vnd.android.package-archive", file);
        }
    }
}