using System.Threading.Tasks;
using Colin.Lottery.Models;

namespace Colin.Lottery.Collectors
{
    public abstract class Collector : ICollector
    {
        public abstract Task<IDrawCollectionModel> GetDrawNoHistory(LotteryType type);
    }
}