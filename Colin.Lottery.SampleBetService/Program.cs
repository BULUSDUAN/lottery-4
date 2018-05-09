using Colin.Lottery.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Colin.Lottery.SampleBetService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Task.Run(() => Run());

            Console.ReadKey();
        }

        private static async Task Run()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:5000/hubs/pk10")
                .Build();

            Console.WriteLine("Starting connection. Press Ctrl-C to close.");

            connection.On<string>("ShowServerTime", (time) =>
            {
                Console.WriteLine($"Server Time is {time} now.");
            });

            connection.On<IForcastPlanModel>("ShowPlans", async (plan) =>
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