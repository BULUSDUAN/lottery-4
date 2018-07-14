using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colin.Lottery.Models;

namespace Colin.Lottery.DataService
{
    public abstract class DataService<T>
        : Singleton<T>, IDataService
        where T : DataService<T>, new()
    {
        public abstract event EventHandler<DataCollectedEventArgs> DataCollectedSuccess;
        public abstract event EventHandler<CollectErrorEventArgs> DataCollectedError;

        public abstract Task Start(bool startWhenBreakGua = false);

        public abstract void Start(Dictionary<LotteryType, List<int>> typeRules, bool startWhenBreakGua = false);

    }
}
