using System;

namespace Colin.Lottery.StrategyService
{
    class Program
    {
        static readonly DateTime _START_DATETIME = new DateTime(2018, 4, 18, 11, 0, 0);
        static void Main(string[] args)
        {
            //Start();

            var span = DateTime.Now - _START_DATETIME;

            Console.ReadKey();
        }

        async static void Start()
        {
            await JinMaStrategyService.Instance.Start();
        }
    }
}
