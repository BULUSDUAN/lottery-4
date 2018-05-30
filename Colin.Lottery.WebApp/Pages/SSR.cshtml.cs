using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Newtonsoft.Json;

using Colin.Lottery.Utils;
using Colin.Lottery.Models;
using Colin.Lottery.Models.BrowserModels;


namespace Colin.Lottery.WebApp.Pages
{
    public class SsrModel : PageModel
    {
        private static readonly string SsrSrcUrl = ConfigUtil.Configuration["SsrSrcUrl"];
        public static IEnumerable<SsrSource> SsrRegs { get; } 
            = JsonConvert.DeserializeObject<List<SsrSource>>(ConfigUtil.Configuration["SsrReg"]);
        public string Ssrs { get; private set; }

        public async Task OnGetAsync(int src)
        {
            var bu = new BrowserUtil();
            void GetLinks(object sender, ExploreCompleteEventArgs e)
            {
                e.WebBrower
                .FindElementsByCssSelector(".mdui-textfield-input")
                .Select(ele => ele.GetAttribute("value"))
                .Take(2)
                .ToList()
                .ForEach(link => SsrRegs.FirstOrDefault(r => link.Contains(r.Url)).Url = link);
            }
            bu.Completed += GetLinks;
            await bu.Explore(SsrSrcUrl);
            var url = (src + 1 > SsrRegs.Count() || src < 0 ? SsrRegs.FirstOrDefault() : SsrRegs.FirstOrDefault(ssr => ssr.Id == src))?.Url;

            bu.Completed -= GetLinks;
            bu.Completed += (s, e) => Ssrs = e.WebBrower.FindElementByTagName("body").Text;
            await bu.Explore(url);
        }
    }
}
