using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Colin.Lottery.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Plugin.LocalNotifications;

namespace Colin.Lottery.MobileApp.ViewModels
{
    public class ForcastViewModel : BaseViewModel
    {
        private HubConnection _connection;

        public ObservableCollection<JinMaForcastModel> Forcasts { get; } =
            new ObservableCollection<JinMaForcastModel>();

        public Command LoadForcastsCommand { get; private set; }

        public ForcastViewModel(Func<string, string, string, Task> alert) : base(alert)
        {
            Title = "PK10连挂";

            InitConnection();
            LoadForcastsCommand = new Command(async () =>
            {
                if (IsBusy)
                    return;

                IsBusy = true;
                try
                {
                    Forcasts?.Clear();
                    await _connection.InvokeAsync("GetAppNewForcast");
                }
                catch
                {
                    Alert("服务器错误", "获取预测数据失败,正在尝试重新连接", "确定");
                    try
                    {
                        await _connection.StopAsync();
                        await _connection.StartAsync();
                        await _connection.InvokeAsync("GetAppNewForcast");
                    }
                    catch
                    {
                        Alert("服务器错误", "尝试重新连接失败,请重试重启程序", "确定");
                    }

                    IsBusy = false;
                }
            });
            LoadForcastsCommand.Execute(null);
        }

        async void InitConnection()
        {
            if (Application.Current.Properties.ContainsKey("HubConnection") &&
                Application.Current.Properties["HubConnection"] != null)
                return;
            
            _connection = new HubConnectionBuilder()
                .WithUrl("http://bet518.win/hubs/pk10")
                .Build();

            _connection.On<JArray>("ShowForcasts", ShowForcasts);
            _connection.On("NoResult", () => IsBusy = false);
            _connection.Closed += async (error) =>
            {
                await _connection.StartAsync();
                Alert("提醒", "与服务器连接断开，已尝试重新连接", "确定");
            };

            try
            {
                await _connection.StartAsync();
                Application.Current.Properties["HubConnection"] = _connection;
            }
            catch
            {
                Alert("服务器错误", "与服务器建立连接失败", "确定");
            }
        }


        //处理接收到的消息
        void ShowForcasts(JArray forcastsArray)
        {
            var forcasts = JsonConvert.DeserializeObject<List<JinMaForcastModel>>(forcastsArray.ToString());
            if (forcasts.Any())
            {
                var refresh = forcasts.Count > 2;
                if (refresh)
                {
                    Forcasts.Clear();
                }
                else
                {
                    var currentPeriod = forcasts.Max(f => f.LastDrawedPeriod);
                    Forcasts.ToList().ForEach(f =>
                    {
                        if (f.LastDrawedPeriod < currentPeriod)
                            Forcasts.Remove(f);
                    });
                }

                foreach (var fc in forcasts)
                    Forcasts.Add(fc);

                if (!refresh)
                {
                    var forcast = forcasts.LastOrDefault();
                    if (forcast != null)
                        CrossLocalNotifications.Current.Show($"PK10连挂提醒",
                            $"{forcast.LastDrawedPeriod + 1}期 {forcast.Rule} {forcast.KeepGuaCnt}连挂 {forcast.ForcastNo} 「{(DateTime.Now.ToString("t"))}」");
                }
            }

            IsBusy = false;
        }
    }
}