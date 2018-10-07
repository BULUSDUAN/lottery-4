using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colin.Lottery.MobileApp.Models;
using Colin.Lottery.Models;

namespace Colin.Lottery.MobileApp.Services
{
    public class MockDataStore : IDataStore<JinMaForcastModel>
    {
        List<JinMaForcastModel> forcasts;

        public MockDataStore()
        {
            forcasts = new List<JinMaForcastModel>();
            var mockForcasts = new List<JinMaForcastModel>
            {
                new JinMaForcastModel {Rule = "冠军",Plan = Plan.PlanA,LastDrawedPeriod =707990,KeepGuaCnt = 3,ChaseTimesAfterEndGua = 2,ChaseTimes = 3,RepetitionScore = 80,WinProbability = 0.875f,TotalCount = 7986,ForcastNo = "01 02 03 07 09"},
                new JinMaForcastModel {Rule = "亚军",Plan = Plan.PlanB,LastDrawedPeriod =707990,KeepGuaCnt = 3,ChaseTimesAfterEndGua = 2,ChaseTimes = 3,RepetitionScore = 80,WinProbability = 0.875f,TotalCount = 7986,ForcastNo = "01 02 03 07 09"},
                new JinMaForcastModel {Rule = "季军",Plan = Plan.PlanA,LastDrawedPeriod =707990,KeepGuaCnt = 3,ChaseTimesAfterEndGua = 2,ChaseTimes = 3,RepetitionScore = 80,WinProbability = 0.875f,TotalCount = 7986,ForcastNo = "01 02 03 07 09"},
                new JinMaForcastModel {Rule = "第四名",Plan = Plan.PlanB,LastDrawedPeriod =707990,KeepGuaCnt = 3,ChaseTimesAfterEndGua = 2,ChaseTimes = 3,RepetitionScore = 80,WinProbability = 0.875f,TotalCount = 7986,ForcastNo = "01 02 03 07 09"},
            };

            foreach (var forcast in mockForcasts)
            {
                forcasts.Add(forcast);
            }
        }

//        public async Task<bool> AddItemAsync(JinMaForcastModel forcast)
//        {
//            forcasts.Add(forcast);
//
//            return await Task.FromResult(true);
//        }
//
//        public async Task<bool> UpdateItemAsync(JinMaForcastModel forcast)
//        {
//            var oldItem = forcasts.Where((JinMaForcastModel arg) => arg.Id == forcast.Id).FirstOrDefault();
//            forcasts.Remove(oldItem);
//            forcasts.Add(forcast);
//
//            return await Task.FromResult(true);
//        }
//
//        public async Task<bool> DeleteItemAsync(string id)
//        {
//            var oldItem = forcasts.Where((JinMaForcastModel arg) => arg.Id == id).FirstOrDefault();
//            forcasts.Remove(oldItem);
//
//            return await Task.FromResult(true);
//        }
//
//        public async Task<JinMaForcastModel> GetItemAsync(string id)
//        {
//            return await Task.FromResult(forcasts.FirstOrDefault(s => s.Id == id));
//        }
//
        public async Task<IEnumerable<JinMaForcastModel>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(forcasts);
        }
    }
}