using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Colin.Lottery.MobileApp.Views;
using Microsoft.AspNetCore.SignalR.Client;
using System.Timers;
using Plugin.LocalNotifications;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Colin.Lottery.MobileApp
{
    public partial class App : Application
    {
//        private readonly Timer _timer;

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

//            _timer = new Timer(300000);
//            _timer.Elapsed += Heartbeat;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }


//        async void Heartbeat(object sender, ElapsedEventArgs er)
//        {
//            var conn = Properties["HubConnection"] as HubConnection;
//            try
//            {
//                await conn.InvokeAsync("Heartbeat");
//            }
//            catch
//            {
//                CrossLocalNotifications.Current.Show("连接断开提醒", "与服务器断开连接,正在尝试重新连接");
//                try
//                {
//                    await conn.StopAsync();
//                    await conn.StartAsync();
//                    await conn.InvokeAsync("Heartbeat");
//                }
//                catch
//                {
//                    CrossLocalNotifications.Current.Show("服务器连接错误", "尝试重新连接服务器失败,请重试重启程序");
//                }
//            }
//        }

        /*
         * Android设备
         * App进入后台运行5分钟没有任何活动，系统会降低其运行资源消耗
         * 此举将导致SignalR服务器连接失败。
         * 我们添加5分钟心跳检测，保持App活动状态即可
         */
        
        protected override void OnSleep()
        {
            // Handle when your app sleeps

//            //启动心跳检测
//            if (Device.RuntimePlatform == Device.Android)
//                _timer.Start();
        }


        protected override void OnResume()
        {
            // Handle when your app resumes

//            //停止心跳检测
//            if (Device.RuntimePlatform == Device.Android)
//                _timer.Stop();
        }
    }
}