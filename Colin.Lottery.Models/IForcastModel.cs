using System.Collections.Generic;

namespace Colin.Lottery.Models
{
    /// <inheritdoc />
    /// <summary>
    /// 预测数据模型接口
    /// </summary>
    public interface IForcastModel:IForcastSharedModel
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

    /// <inheritdoc />
    /// <summary>
    /// 预测数据集模型接口
    /// </summary>
    public interface IForcastPlanModel:IForcastSharedModel
    {
        /// <summary>
        /// 预测数据
        /// </summary>
        List<IForcastModel> ForcastData { get; set; }

        /// <summary>
        /// 最新预测开奖号码
        /// </summary>
        string ForcastDrawNo { get; }
    }

    /// <summary>
    /// 预测数据Plan级与最新期共享字段
    /// </summary>
    public interface IForcastSharedModel
    {
        /// <summary>
        /// 玩法规则
        /// </summary>
        string Rule { get; set; }
        
        /// <summary>
        /// 计划
        /// </summary>
        Plan Plan { get; set; }

        /// <summary>
        /// 最新开奖期号
        /// </summary>
        long LastDrawedPeriod { get; set; }

        /// <summary>
        /// 最新开奖号码
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
        /// 挂-分数
        /// </summary>
        float GuaScore { get; set; }

        /// <summary>
        /// 号码重复度-分数
        /// </summary>
        float RepetitionScore { get; set; }

        /// <summary>
        /// 追号次数-分数
        /// </summary>
        float BetChaseScore { get; set; }

        /// <summary>
        /// 最新预测评分
        /// </summary>
        float Score { get; set; }

        /// <summary>
        /// 有效连挂次数(连挂已结束)
        /// </summary>
        int KeepGuaCnt { get; set; }

        /// <summary>
        /// 第几段跟投(连挂结束后)
        /// </summary>
        int ChaseTimesAfterEndGua { get; set; }

        /// <summary>
        /// 连挂次数(连挂中)
        /// </summary>
        int KeepGuaingCnt { get; set; }
    }
}
