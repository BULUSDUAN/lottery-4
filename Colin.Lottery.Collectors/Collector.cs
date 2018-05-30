using System.Threading.Tasks;
using Colin.Lottery.Models;

namespace Colin.Lottery.Collectors
{
    public abstract class Collector<T>
        : Singleton<T>, ICollector
        where T : Collector<T>, new()
    {
        public abstract Task<IDrawCollectionModel> GetDrawNoHistory(LotteryType type);
    }
}
