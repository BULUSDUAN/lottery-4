using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Colin.Lottery.MobileApp.Models;
using Colin.Lottery.MobileApp.Views;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Colin.Lottery.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Plugin.LocalNotifications;
using Xamarin.Forms.Internals;

namespace Colin.Lottery.MobileApp.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private HubConnection connection;

        public ObservableCollection<JinMaForcastModel> Forcasts { get; } =
            new ObservableCollection<JinMaForcastModel>();

        public Command LoadForcastsCommand { get; private set; }
        private Func<string, string, string, Task> Alert;

        public ItemsViewModel(Func<string, string, string, Task> alert)
        {
            Title = "PK10连挂";
            Alert = alert;

            InitConnection();
            LoadForcastsCommand = new Command(async () =>
            {
                if (IsBusy)
                    return;

                IsBusy = true;
                try
                {
                    Forcasts.Clear();
                    await connection.InvokeAsync("GetAppNewForcast");
                }
                catch
                {
                    Device.BeginInvokeOnMainThread(() => Alert("服务器错误", "获取服务器预测数据失败", "确定"));
                    IsBusy = false;
                }
            });
            LoadForcastsCommand.Execute(null);
        }

        private async void InitConnection()
        {
            if (Application.Current.Properties.ContainsKey("HubConnection") &&
                Application.Current.Properties["HubConnection"] != null)
                return;

            connection = new HubConnectionBuilder()
                .WithUrl("http://bet518.win/hubs/pk10")
                .Build();

            connection.On<JArray>("ShowPlans", ShowPlans);
            connection.On("NoResult", () => IsBusy = false);
            connection.Closed += async (error) =>
            {
                await connection.StartAsync();
                Device.BeginInvokeOnMainThread(() => Alert("提醒", "与服务器连接断开，已尝试重新连接", "确定"));
            };

            try
            {
                await connection.StartAsync();
                Application.Current.Properties["HubConnection"] = connection;
            }
            catch
            {
                Device.BeginInvokeOnMainThread(() => Alert("服务器错误", "与服务器建立连接失败", "确定"));
            }
        }


        //处理接收到的消息
        void ShowPlans(JArray planArray)
        {
            var forcasts = JsonConvert.DeserializeObject<List<JinMaForcastModel>>(planArray.ToString());
            if (forcasts.Any())
            {
                bool refresh = forcasts.Count > 1;
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
                    CrossLocalNotifications.Current.Show($"PK10连挂提醒", $"{forcast.LastDrawedPeriod + 1}期 {forcast.Rule} {forcast.KeepGuaCnt}连挂 {forcast.ForcastNo} 「{(DateTime.Now.ToString("t"))}」");
                }
            }

            IsBusy = false;
        }
    }
}