using System;
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
                var autoBet = new AutoBetMain();
                autoBet.Login();

                long periodNo = 694724;

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
                LogUtil.Error(e.StackTrace);
            }

            Console.WriteLine("Program Ends!");

            Console.ReadKey();
        }
    }
}