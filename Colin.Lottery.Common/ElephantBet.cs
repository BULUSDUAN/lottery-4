using Colin.Lottery.Models;
using Colin.Lottery.Models.BrowserModels;
using Colin.Lottery.Utils;

namespace Colin.Lottery.Common
{
    public class ElephantBet
    {
        private BrowserUtil _browser;
        private string _url;
        public ElephantBet(string url,string userName,string password)
        {
            _browser = new BrowserUtil();
            _url = url;
        }

        public bool Bet(LotteryType type, int rule, long periodNo, string betNo, decimal betMoney)
        {
            return false;
        }

//        private void ChooseRoute()
//        {
//            _browser.Explore(_url, new Operation{}, new Script()).Wait();
//        }
    }
}