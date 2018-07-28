using Colin.Lottery.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Colin.Lottery.Common.Models
{
    public class BetBills
    {
        public List<BetBill> Data { get; set; } = new List<BetBill>();

        /// <summary>
        /// 数据总条数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 其它数据
        /// </summary>
        public OtherData OtherData { get; set; }
    }

    public class BetBill
    {
        public long Id { get; set; }

        public long userId { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public long TurnNum { get; set; }

        /// <summary>
        /// 游戏种类ID（50表示PK10）
        /// </summary>
        public int GameId { get; set; }

        public uint playId { get; set; }

        /// <summary>
        /// 下注总金额
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 盈利
        /// </summary>
        public decimal WinMoney { get; set; }

        public decimal ResultMoney { get; set; }

        /// <summary>
        /// 下注金额
        /// </summary>
        public decimal Reward { get; set; }

        /// <summary>
        /// 赔率
        /// </summary>
        public float Odds { get; set; }

        /// <summary>
        /// 开奖号码
        /// </summary>
        public string OpenNum { get; set; }

        /// <summary>
        /// 下注时间
        /// </summary>
        public DateTime AddTime { get; set; }

    }

    public class OtherData
    {
        public decimal TotalRebateMoney { get; set; }

        /// <summary>
        /// 总盈利金额
        /// </summary>
        public decimal TotalResultMoney { get; set; }

        /// <summary>
        /// 总下注金额
        /// </summary>
        public decimal TotalBetMoney { get; set; }
    }
}