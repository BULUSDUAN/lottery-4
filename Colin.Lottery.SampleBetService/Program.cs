using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Sockets;

namespace Colin.Lottery.SampleBetService
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() => Run());

            Console.ReadKey();
        }

        static async Task Run()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:52648/hubs/pk10")
                .WithConsoleLogger()
                //.WithMessagePackProtol()
                .WithTransport(Microsoft.AspNetCore.Http.Connections.TransportType.LongPolling)
                .Build();

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, a) =>
            {
                a.Cancel = true;
                cts.Cancel();
            };

            connection.On<string>("ShowServerTime", (time) =>
            {
                Console.WriteLine($"Server Time Is {time} now.");
            });

            connection.Closed += e =>
            {
                Console.WriteLine("Connection closed with error: {0}", e);

                cts.Cancel();
            };

            await connection.StartAsync();
            Console.WriteLine("Starting connection. Press Ctrl-C to close.");

        }
    }
}
