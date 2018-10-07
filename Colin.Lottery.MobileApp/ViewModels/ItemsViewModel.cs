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

        public ItemsViewModel()
        {
            Title = "PK10连挂";

            InitConnection();
            LoadForcastsCommand = new Command(async () =>
            {
                if (IsBusy)
                    return;

                IsBusy = true;
                await connection.InvokeAsync("GetAppNewForcast");
            });
            LoadForcastsCommand.Execute(null);
        }

        private async void InitConnection()
        {
            if (Application.Current.Properties.ContainsKey("HubConnection") &&
                Application.Current.Properties["HubConnection"] != null)
                return;

            connection = new HubConnectionBuilder()
                .WithUrl("http://192.168.31.191/hubs/pk10")
                .Build();

            connection.On<JArray>("ShowPlans", ShowPlans);
            connection.On("NoResult", () => IsBusy = false);
            connection.Closed += async (error) => await connection.StartAsync();

            try
            {
                await connection.StartAsync();
                Application.Current.Properties["HubConnection"] = connection;
            }
            catch (Exception ex)
            {
                //TODO:优化错误消息提醒
//                CrossLocalNotifications.Current.Show("服务器错误", ex.Message);
                
            }
        }


        //处理接收到的消息
        void ShowPlans(JArray planArray)
        {
            var forcasts = JsonConvert.DeserializeObject<List<JinMaForcastModel>>(planArray.ToString());
            if (forcasts.Any())
            {
                var currentPeriod = forcasts.Max(f => f.LastDrawedPeriod);
                Forcasts.ToList().ForEach(f =>
                {
                    if (f.LastDrawedPeriod < currentPeriod)
                        Forcasts.Remove(f);
                });

                foreach (var fc in forcasts)
                    Forcasts.Add(fc);

                //TODO:根据App状态确定是否推送通知
                //CrossLocalNotifications.Current.Show("服务器消息", "牛逼了");
            }

            IsBusy = false;
        }
    }
}