using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colin.Lottery.Models
{
    /// <summary>
    /// 开奖数据接口
    /// </summary>
    public interface IDrawModel
    {
        /// <summary>
        /// 期号
        /// </summary>
        long PeriodNo { get; set; }

        /// <summary>
        /// 开奖号码
        /// </summary>
        string DrawNo { get; set; }
    }

    /// <summary>
    /// 开奖数据集接口
    /// </summary>
    public interface IDrawCollectionModel
    {
        /// <summary>
        /// 开奖数据
        /// </summary>
        List<IDrawModel> DrawData { get; set; }
    }
}
