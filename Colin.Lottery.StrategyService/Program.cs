using System;
using Microsoft.Owin.Hosting;

namespace Colin.Lottery.StrategyService
{
    class Program
    {
        static void Main(string[] args)
        {
            // This will *ONLY* bind to localhost, if you want to bind to all addresses
            // use http://*:8080 or http://+:8080 to bind to all addresses. 
            // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx 
            // for more information.
            using (WebApp.Start<Startup>("http://localhost:8080/"))
            {
                Console.WriteLine("SignalR Server running at http://localhost:8080/");
            }

            Start();
            Console.ReadKey();
        }

        async static void Start()
        {
            await JinMaStrategyService.Instance.Start();
        }
    }
}
