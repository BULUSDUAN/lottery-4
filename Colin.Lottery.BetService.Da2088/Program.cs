using System;
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
                
                //autoBet.Bet();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Program Ends!");
        }
    }
}