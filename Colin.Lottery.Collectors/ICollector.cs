using System.Threading.Tasks;

using Colin.Lottery.Models;

namespace Colin.Lottery.Collectors
{
    /// <summary>
    /// 彩票采集器接口
    /// </summary>
    public interface ICollector
    {
        /// <summary>
        /// 查询制定彩种历史开奖数据
        /// </summary>
        /// <param name="type">彩种类型</param>
        /// <returns></returns>
        Task<IDrawCollectionModel> GetDrawNoHistory(LotteryType type);
    }
}
