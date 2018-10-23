using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colin.Lottery.Analyzers;
using Colin.Lottery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Colin.Lottery.WebApp.Pages
{
    public class AppModel : PageModel
    {
        public async Task<IActionResult> OnGetAppPlansAsync(int rule = 0)
        {
            var forecast = new List<IForecastModel>();
            await GetNewForecast(forecast, Pk10Rule.Champion);
            await GetNewForecast(forecast, Pk10Rule.Second);
            await GetNewForecast(forecast, Pk10Rule.Third);
            await GetNewForecast(forecast, Pk10Rule.Fourth);

            if (rule > 0)
            {
                await GetNewForecast(forecast, Pk10Rule.BigOrSmall);
                await GetNewForecast(forecast, Pk10Rule.OddOrEven);
                await GetNewForecast(forecast, Pk10Rule.DragonOrTiger);
            }

            return Content(JsonConvert.SerializeObject(forecast));
        }

        private static async Task GetNewForecast(ICollection<IForecastModel> newForecast, Pk10Rule rule)
        {
            var plans = await JinMaAnalyzer.Instance.GetForecastData(LotteryType.Pk10, (int) rule);
            if (plans == null || plans.Count < 2 || plans.Any(p => p == null))
                return;

            JinMaAnalyzer.Instance.CalcuteScore(plans);
            plans.ForEach(p => newForecast.Add(p.ForecastData.LastOrDefault()));
        }

        public async Task<IActionResult> OnGetAppPlanDetailsAsync(int rule = 1)
        {
            var plans = await JinMaAnalyzer.Instance.GetForecastData(LotteryType.Pk10, rule);
            if (plans == null || plans.Count() < 2 || plans.Any(p => p == null))
                return null;

            JinMaAnalyzer.Instance.CalcuteScore(plans);
            return Content(JsonConvert.SerializeObject(plans));
        }
    }
}