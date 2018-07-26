using System;
using Colin.Lottery.Common;

namespace Colin.Lottery.BetService
{
    class Program
    {
        static void Main(string[] args)
        {
//            AutoBet.StartConnection();

            var bet= new ElephantBet("https://www-dx888.com", "ColinChang", "xinzhe468dx");
            
            
            Console.WriteLine("OK");
            Console.ReadKey();
        }
    }
}