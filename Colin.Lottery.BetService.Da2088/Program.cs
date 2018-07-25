using System;
using Colin.Lottery.Common.AutoBetSites;
using Colin.Lottery.Models;
using Colin.Lottery.Utils;

namespace Colin.Lottery.BetService.Da2088
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var autoBet = new SiteBet_Da2088();

                /*
                 * 登录
                 */
                string account = "song90273";
                string password = "200_daxl5306";
                autoBet.Login(account, password);
                
                /*
                 * 投注
                 */
                long periodNo = 694744;

                autoBet.Bet(periodNo, Pk10Rule.Champion, "1,2,3,4,5", 1m);
                autoBet.Bet(periodNo, Pk10Rule.Second, "1,2,3,4,5", 1m);
                autoBet.Bet(periodNo, Pk10Rule.Third, "1,2,3,4,5", 1m);
                autoBet.Bet(periodNo, Pk10Rule.Fourth, "1,2,3,4,5", 1m);
                autoBet.Bet(periodNo, Pk10Rule.BigOrSmall, "大", 1m);
                autoBet.Bet(periodNo, Pk10Rule.BigOrSmall, "小", 1m);
                autoBet.Bet(periodNo, Pk10Rule.OddOrEven, "单", 1m);
                autoBet.Bet(periodNo, Pk10Rule.OddOrEven, "双", 1m);
                autoBet.Bet(periodNo, Pk10Rule.DragonOrTiger, "龙", 1m);
                autoBet.Bet(periodNo, Pk10Rule.DragonOrTiger, "虎", 1m);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    e = e.InnerException;
                }

                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            Console.WriteLine("Program Ends!");

            Console.ReadKey();
        }
    }
}