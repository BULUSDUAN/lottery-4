using Colin.Lottery.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Colin.Lottery.SampleBetService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadKey();
        }

        static async Task Run()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:5000/hubs/pk10")
                .Build();

            Console.WriteLine("Starting connection. Press Ctrl-C to close.");

            connection.On<string>("ShowServerTime", (time) =>
            {
                Console.WriteLine($"Server Time is {time} now.");
            });

            connection.On<IForcastPlanModel>("ShowPlans", plan =>
           {
               SampleBet.ShowPlans(plan);
               //Console.WriteLine($"Server Time is {time} now.");
           });

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, a) =>
            {
                a.Cancel = true;
                cts.Cancel();
            };

            //connection.Closed += e =>
            //{
            //    Console.WriteLine("Connection closed with error: {0}", e);

            //    cts.Cancel();
            //};

            await connection.StartAsync();
        }
    }
}