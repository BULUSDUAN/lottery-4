using System;
using Colin.Lottery.Models;
using Colin.Lottery.SampleBetService.DataModels;

namespace Colin.Lottery.SampleBetService
{
    class Program
    {
        static void Main(string[] args)
        {
            SampleBet.StartConnection();

            Console.WriteLine("OK");
            Console.ReadKey();
        }
    }
}