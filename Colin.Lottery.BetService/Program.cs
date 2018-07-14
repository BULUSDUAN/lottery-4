using System;

namespace Colin.Lottery.BetService
{
    class Program
    {
        static void Main(string[] args)
        {
            AutoBet.StartConnection();

            Console.WriteLine("OK");
            Console.ReadKey();
        }
    }
}