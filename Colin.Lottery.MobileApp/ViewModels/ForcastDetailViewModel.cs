using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Colin.Lottery.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace Colin.Lottery.MobileApp.ViewModels
{
    public class ForcastDetailViewModel : BaseViewModel
    {
        public ObservableCollection<GroupedPlans> Plans { get; } =
            new ObservableCollection<GroupedPlans>();

        public Command LoadPlansCommand { get; private set; }
        private static HubConnection Connection => Application.Current.Properties["HubConnection"] as HubConnection;

        private ListView _lv;

        public ForcastDetailViewModel(Pk10Rule rule, ListView lv, Func<string, string, string, Task> alert) :
            base(alert)
        {
            _lv = lv;
            Title = $"{rule.ToStringName()}预测历史";

            Connection.On<JArray>("ShowPlans", ShowPlans);
            LoadPlansCommand = new Command(async () =>
            {
                if (IsBusy)
                    return;

                IsBusy = true;
                try
                {
                    await Connection.InvokeAsync("GetForcastDataByRule", (int) rule);
                }
                catch
                {
                    Alert("服务器错误", "获取计划数据失败,正在尝试重新连接", "确定");
                    try
                    {
                        await Connection.StopAsync();
                        await Connection.StartAsync();
                        await Connection.InvokeAsync("GetForcastDataByRule", (int) rule);
                    }
                    catch
                    {
                        Alert("服务器错误", "尝试重新连接失败,请重试重启程序", "确定");
                    }

                    IsBusy = false;
                }
            });

            LoadPlansCommand.Execute(null);
        }

        private void ShowPlans(JArray planArray)
        {
            var plans = JsonConvert.DeserializeObject<List<QxForcastPlanModel>>(planArray.ToString());
            if (plans.Any())
            {
                /*
                 * ListView分组数据源绑定后不可动态修改，需要先手动断开数据关联后方可修改数据源，然后再次绑定
                 */
                Device.BeginInvokeOnMainThread(() => _lv.ItemsSource = null);
                foreach (var plan in plans)
                {
                    var pl = new GroupedPlans(plan.Plan, plan.TotalCount, plan.WinCount, plan.LoseCount,
                        plan.WinProbability,
                        plan.Score, plan.GuaScore, plan.RepetitionScore, plan.BetChaseScore);
                    plan.ForcastData.ForEach(p => pl.Add(p));

                    Plans.Add(pl);
                }

                Device.BeginInvokeOnMainThread(() => _lv.ItemsSource = Plans);
            }

            IsBusy = false;
        }
    }

    public class QxForcastModel
    {
        public long LastPeriod { get; set; }
        public string ForcastNo { get; set; }
        public int ChaseTimes { get; set; }
        public bool? IsWin { get; set; }
    }

    public class QxForcastPlanModel
    {
        public Plan Plan { get; set; }
        public int TotalCount { get; set; }
        public int WinCount { get; set; }
        public int LoseCount { get; set; }
        public float WinProbability { get; set; }
        public float Score { get; set; }
        public float GuaScore { get; set; }
        public float RepetitionScore { get; set; }
        public float BetChaseScore { get; set; }

        public List<QxForcastModel> ForcastData { get; set; }
    }

    public class GroupedPlans : ObservableCollection<QxForcastModel>
    {
        public GroupedPlans(Plan plan, int totalCount, int winCount, int loseCount, float winProbability, float score,
            float guaScore, float repetitionScore, float betChaseScore)
        {
            Plan = plan;
            TotalCount = totalCount;
            WinCount = winCount;
            LoseCount = loseCount;
            WinProbability = winProbability;
            Score = score;
            GuaScore = guaScore;
            RepetitionScore = repetitionScore;
            BetChaseScore = betChaseScore;
        }

        public Plan Plan { get; set; }
        public int TotalCount { get; set; }
        public int WinCount { get; set; }
        public int LoseCount { get; set; }
        public float WinProbability { get; set; }
        public float Score { get; set; }
        public float GuaScore { get; set; }
        public float RepetitionScore { get; set; }
        public float BetChaseScore { get; set; }
    }
}