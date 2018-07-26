using System;
using System.Linq;
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

                /*
                 * 登录
                 */
                string account = args[0];
                string password = args[1];

                var autoBet = new SiteBet_Da2088(account, password);

                autoBet.Login();

                /*
                 * 投注
                 */
                long periodNo = 694816;

                string preNumbers = "01 02 04 05 09".Trim();
                int[] preNumbersArray = preNumbers.Split(' ').Select(x => int.Parse(x)).ToArray();
                string numbers = string.Join(',', preNumbersArray);

                decimal money = 3m; // 单注1元

                // 亚军
                autoBet.Bet(periodNo, Pk10Rule.Second, numbers, money);
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