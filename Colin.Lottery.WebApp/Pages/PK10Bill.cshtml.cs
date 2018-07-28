using Colin.Lottery.Common.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Colin.Lottery.WebApp.Pages
{
    public class PK10BillModel : PageModel
    {
        public BetBills Bills { get; set; }
        public void OnGet()
        {
            Bills = Da2088Helper.SiteBetDa2088.GetBetBills();
        }
    }
}
