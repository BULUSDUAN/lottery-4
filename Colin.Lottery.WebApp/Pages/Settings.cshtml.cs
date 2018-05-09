using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colin.Lottery.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Colin.Lottery.WebApp.Pages
{
    public class SettingsModel : PageModel
    {
        public List<LotteryType> LotteryTypes { get; set; } = new List<LotteryType> { LotteryType.PK10, LotteryType.CQSSC };
        public List<Plan> Plans { get; set; } = new List<Plan> { Plan.PlanA, Plan.PlanB };
        public List<PK10Rule> PK10_Rules { get; set; } = new List<PK10Rule> { PK10Rule.Champion, PK10Rule.Second, PK10Rule.Third, PK10Rule.Fourth, PK10Rule.BigOrSmall, PK10Rule.OddOrEven, PK10Rule.DragonOrTiger, PK10Rule.Sum };
        public List<CQSSCRule> CQSSC_Rules { get; set; } = new List<CQSSCRule> { CQSSCRule.OddOrEven, CQSSCRule.BigOrSmall, CQSSCRule.DragonOrTiger, CQSSCRule.Last2Group, CQSSCRule.Last3Group, CQSSCRule.OneOddOrEven, CQSSCRule.OneBigOrSmall, CQSSCRule.One };
        public void OnGet()
        {

        }
    }
}
