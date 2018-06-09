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

        private static void Test()
        {
            using (var db = new SampleBetContext())
            {
                db.BetAll.Add(new BeAllRecord(LotteryType.Pk10,1,Plan.PlanA,123,"1 2 3 4 5",2,1,1.956f,10000));
                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);

                Console.WriteLine("AllRecords in database:");
                foreach (var record in db.BetAll)
                {
                    Console.WriteLine(" - {0}",record.Id);
                }
            }
        }
    }
}