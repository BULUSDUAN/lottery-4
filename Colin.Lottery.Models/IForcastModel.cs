using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colin.Lottery.Models
{
    /// <summary>
    /// 预测数据模型接口
    /// </summary>
    public interface IForcastModel
    {
        /// <summary>
        /// 期号范围
        /// </summary>
        string PeriodRange { get; set; }

        /// <summary>
        /// 预测号码
        /// </summary>
        string ForcastNo { get; set; }

        /// <summary>
        /// 追号次数
        /// </summary>
        int ChaseTimes { get; set; }

        /// <summary>
        /// 是否中奖
        /// </summary>
        bool? IsWin { get; set; }

        /// <summary>
        /// 截至期号
        /// </summary>
        long LastPeriod { get; set; }

        /// <summary>
        /// 开奖号码
        /// </summary>
        string DrawNo { get; set; }
    }

    /// <summary>
    /// 预测数据集模型接口
    /// </summary>
    public interface IForcastPlanModel
    {
        /// <summary>
        /// 预测数据
        /// </summary>
        List<IForcastModel> ForcastData { get; set; }

        /// <summary>
        /// 最新一期开奖期号
        /// </summary>
        long LastDrawedPeriod { get; set; }

        /// <summary>
        /// 最新一期开奖号码
        /// </summary>
        string LastDrawNo { get; set; }

        /// <summary>
        /// 统计有效期数
        /// </summary>
        int TotalCount { get; set; }

        /// <summary>
        /// 中奖期数
        /// </summary>
        int WinCount { get; set; }

        /// <summary>
        /// 未中奖期数
        /// </summary>
        int LoseCount { get; set; }

        /// <summary>
        /// 中奖概率
        /// </summary>
        float WinProbability { get; set; }

        /// <summary>
        /// 最新预测评分
        /// </summary>
        float Score { get; set; }

        /// <summary>
        /// 从最新期开始连挂次数
        /// </summary>
        int KeepGuaCnt { get; set; }

        /// <summary>
        /// 历史记录(不包含从最新期连挂的情况)出现连挂次数
        /// </summary>
        int KeepHisGuaCnt { get; set; }
    }
}
