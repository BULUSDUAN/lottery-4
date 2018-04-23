using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Colin.Lottery.StrategyService
{
    class Program
    {
        static void Main(string[] args)
        {

            Test();

            //Start();
            Console.ReadKey();
        }

        async static void Start()
        {
            await JinMaStrategyService.Instance.Start();
        }

        static async void Test()
        {
            var hubConn = new HubConnectionBuilder()
                .WithUrl("https://localhost:44383/hubs/pk10")
                .Build();

            
            //供服务端调用的客户端方法
            hubConn.On<DateTime>("ShowServerTime", data => Console.WriteLine(data));

            await hubConn.StartAsync();

            //调用服务端方法
            await hubConn.InvokeAsync("Test");
            

            //停止链接
            //await hubConn.DisposeAsync();
        }
    }
}
