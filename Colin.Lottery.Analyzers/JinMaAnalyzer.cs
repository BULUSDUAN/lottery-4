using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Colin.Lottery.Collectors;
using Colin.Lottery.Models;

namespace Colin.Lottery.Analyzers
{
    public class JinMaAnalyzer : Analyzer<JinMaAnalyzer>
    {
        #region 查询预测数据

        /// <summary>
        /// 获取给定彩种/玩法/计划员的预测数据
        /// </summary>
        /// <param name="type">彩种</param>
        /// <param name="planer">计划员</param>
        /// <param name="rule">玩法(请使用具体玩法枚举,如"PK10Rule","CQSSCRule")</param>
        /// <returns>预测数据</returns>
        public override async Task<IForcastPlanModel> GetForcastData(LotteryType type, Planner planer, int rule)
        {
            return await JinMaCollector.Instance.GetForcastData(type, planer, rule);
        }

        public override async Task<(IForcastPlanModel Plan1, IForcastPlanModel Plan2)> GetForcastData(LotteryType type, int rule)
        {
            var plan1 = await GetForcastData(type, Planner.Planner1, rule);
            var plan2 = await GetForcastData(type, Planner.Planner2, rule);
            return (plan1, plan2);
        }

        public override async Task<(IForcastPlanModel Plan1, IForcastPlanModel Plan2)> GetForcastData(LotteryType type)
        {
            int rule;
            switch (type)
            {
                case LotteryType.CQSSC:
                    rule = (int)CQSSCRule.One;
                    break;
                case LotteryType.PK10:
                    rule = (int)PK10Rule.Champion;
                    break;
                default:
                    throw new Exception("彩种暂不支持");
            }
            return await GetForcastData(type, rule);
        }

        public override async Task<(IForcastPlanModel Plan1, IForcastPlanModel Plan2)> GetForcastData()
        {
            return await GetForcastData(LotteryType.PK10, (int)PK10Rule.Champion);
        }

        #endregion

        /*
         * 北京赛车 投注策略
         * 
         * 投注策略由以下因素影响
         * 1.预测历史15段中"挂"的位置和数量
         * 2.两位计划员最新期预测内容相同比例
         * 3.第几次跟投追号
         * 4.计划员预测准确率                  
         * 
         * 评分策略
         * 1.每一个挂为 _GUA 分
         *  距最新开奖段每远一段权重下降 _GUA_PRIORITY(0%<=_GUA_PRIORITY<=100%),从最近一期开始的连挂,每个挂权重均为100%
         *  历史记录(不包含从最新期连挂的情况)出现连挂，每个连股期数都附加_KEEP_GUA_EXTRA分
         *  所有挂分数超过100分按100分计算
         *  出现多期连挂，最后单独处理
         * 2.两个计划员预测当前期结果完全相同为 _REPETITION 分,权重为预测结果相同比例 X(0%<=X<=100%) 
         * 3.单次跟投为 _BET_CHASE 分,第 N 次跟投 为 (N-1)*_BET_CHASE 分
         * 4.计划员预测准确率 P 为全局系数 
         *  正常情况下 MIN_PRPBABILITY <= P <= MAX_PRPBABILITY 我们将MIN_PRPBABILITY ~ MIN_PRPBABILITY映射为0%-100%，换算公式如下 
         *  (P-MIN_PRPBABILITY)*1/(MAX_PRPBABILITY-MIN_PRPBABILITY)
         * 5. 1，2，3的计算分数分别占比为 PG:PR:PC
         * 若出现最近期连挂或者计划员预测号码重复度100%则分数为100
         * 若计划员号码重复度X>=80%则分数为 _REPETITION*X
         */

        /*
        * 连挂结束跟投策略
        * 
        * 假定有效连挂(距离当前期最近的已经结束的连挂)次数为N
        * 1.N >= _START_GUA_TIME
        * 2.有效连挂基础分算法
        *   N >= _KEEP_GUA_TIME 时基础分满分_GU
        *   N < _KEEP_GUA_TIME 时基础分按照 (_KEEP_GUA_TIME-N)*_DELTA_REDUCE 递减
        * 3.段位分递减算法
        *   我们认为连挂N段后，可以安全跟投N+1段，因为结束连挂已经过了一段，所以还可以安全跟投 N 次，结束连挂后的第二段段位分不减
        *   每多跟投一段安全性降低一点，所以段位分随着跟投段位递减，假定当前为第 M 次跟投(1< M <= N+1) 段位分递减公式为
        *   (M-2)*_DELTA_REDUCE*_GUA
        * 4.有效连挂之前的历史记录中每出现一个挂记录记为_GUA_BASE分，过期有效挂视为普通挂
        * 
        */

        #region 评分参数

        /// <summary>
        /// 单挂分数
        /// </summary>
        private const int _GUA = 100;
        /// <summary>
        /// 挂降权比例
        /// </summary>
        private const float _GUA_PRIORITY = 0.15f;
        /// <summary>
        /// 挂降权到0下时，每个挂的基础分数
        /// </summary>
        private const int _GUA_BASE = 5;
        /// <summary>
        /// 连挂附加分
        /// </summary>
        private const int _KEEP_GUA_EXTRA = 15;
        /// <summary>
        /// 重复号码总分
        /// </summary>
        private const int _REPETITION = 100;
        /// <summary>
        /// 追号次数
        /// </summary>
        private const int _BET_CHASE = 50;


        /// <summary>
        /// 起投连挂次数
        /// </summary>
        private const int _START_GUA_TIME = 1;
        /// <summary>
        /// 几次有效连挂以上为满分挂
        /// </summary>
        private const int _KEEP_GUA_TIME = 3;
        /// <summary>
        /// 有效挂递减比例
        /// </summary>
        private const float _DELTA_REDUCE = 0.1f;


        /// <summary>
        /// 挂 权重(出现挂优先)
        /// </summary>
        private const float PG = 0.45f;
        /// <summary>
        /// 重复号码 权重(出现挂优先)
        /// </summary>
        private const float PR = 0.35f;
        /// <summary>
        /// 追号 权重(出现挂优先)
        /// </summary>
        private const float PC = 0.2f;
        /// <summary>
        /// 挂 权重(结束挂优先)
        /// </summary>
        private const float PGB = 0.7f;
        /// <summary>
        /// 重复号码 权重(结束挂优先)
        /// </summary>
        private const float PRB = 0.2f;
        /// <summary>
        /// 追号 权重(结束挂优先)
        /// </summary>
        private const float PCB = 0.1f;
        /// <summary>
        /// 计划员最小胜率
        /// </summary>
        private const float MIN_PRPBABILITY = 0.3f;
        /// <summary>
        /// 计划员最大胜率
        /// </summary>
        private const float MAX_PRPBABILITY = 0.9f;

        #endregion

        public override void CalcuteScore(ref (IForcastPlanModel Plan1, IForcastPlanModel Plan2) plans, bool startWhenBreakGua = false)
        {
            (IForcastPlanModel Plan1, IForcastPlanModel Plan2) = plans;
            var p1Forcast = Plan1.ForcastData.LastOrDefault();
            var p2Forcast = Plan2.ForcastData.LastOrDefault();

            var repetition = CalcuteRepetition(p1Forcast.ForcastNo, p2Forcast.ForcastNo);
            var gua1 = CalcuteGua(Plan1.ForcastData, startWhenBreakGua, out int keepGuaCnt1, out int keepHisGuaCnt1);
            var gua2 = CalcuteGua(Plan2.ForcastData, startWhenBreakGua, out int keepGuaCnt2, out int keepHisGuaCnt2);
            var chase1 = CalcuteBetChase(p1Forcast.ChaseTimes);
            var chase2 = CalcuteBetChase(p2Forcast.ChaseTimes);

            var rp = _REPETITION * 0.8;
            if (!startWhenBreakGua)
            {
                if (keepGuaCnt1 > 1 || repetition >= rp)
                    Plan1.Score = 100;
                else
                {
                    var s1 = (gua1 * PG + repetition * PR + chase1 * PC) * (Plan1.WinProbability - MIN_PRPBABILITY) / (MAX_PRPBABILITY - MIN_PRPBABILITY);
                    Plan1.Score = repetition >= rp ? repetition : s1;
                }
                if (keepGuaCnt2 > 1 || repetition >= rp)
                    Plan2.Score = 100;
                else
                {
                    var s2 = (gua2 * PG + repetition * PR + chase2 * PC) * (Plan2.WinProbability - MIN_PRPBABILITY) / (MAX_PRPBABILITY - MIN_PRPBABILITY);
                    Plan2.Score = repetition >= rp ? repetition : s2;
                }
            }
            else
            {
                if (gua1 >= 90 || repetition >= rp)
                    Plan1.Score = 100;
                else
                {
                    var s1 = (gua1 * PGB + repetition * PRB + chase1 * PCB) * (Plan1.WinProbability - MIN_PRPBABILITY) / (MAX_PRPBABILITY - MIN_PRPBABILITY);
                    Plan1.Score = repetition >= rp ? repetition : s1;
                }
                if (gua2 >= 90 || repetition >= rp)
                    Plan2.Score = 100;
                else
                {
                    var s2 = (gua2 * PGB + repetition * PRB + chase2 * PCB) * (Plan2.WinProbability - MIN_PRPBABILITY) / (MAX_PRPBABILITY - MIN_PRPBABILITY);
                    Plan2.Score = repetition >= rp ? repetition : s2;
                }
            }

            Plan1.KeepGuaCnt = keepGuaCnt1;
            Plan1.KeepHisGuaCnt = keepHisGuaCnt1;
            Plan2.KeepGuaCnt = keepGuaCnt2;
            Plan2.KeepHisGuaCnt = keepHisGuaCnt2;
        }


        /// <summary>
        /// 计算"挂"分数 (出现挂优先)
        /// </summary>
        /// <param name="forcastData"></param>
        /// <returns></returns>
        float CalcuteGua(List<IForcastModel> forcastData, out int keepGuaCnt, out int keepHisGuaCnt)
        {
            float total = 0;
            //从最新期开始连挂次数
            keepGuaCnt = 0;
            //历史记录(不包含从最新期连挂的情况)出现连挂次数
            keepHisGuaCnt = 0;

            if (forcastData.Count() < 2)
                throw new Exception("预测历史数据不足,无法进行评估");

            var results = forcastData.Take(forcastData.Count() - 1).Select(f => f.IsWin);
            var lastIndex = results.Count() - 1;
            for (int i = lastIndex; i >= 0; i--)
            {
                var isWin = results.ElementAt(i);
                if (isWin != false)
                    continue;

                //从最新一期非连挂
                var breakGua = results.Skip(i).Take(results.Count() - i).Any(w => w != false);

                float priority;

                //从最近一期连挂 所有挂的期数权重均为100%
                if (!breakGua)
                {
                    priority = 1;
                    keepGuaCnt++;
                }
                else
                {
                    //历史记录(不包含从最新期连挂的情况)出现连挂，每个连股期数都附加相应分数
                    if (results.Skip(i + 1).Take(1).FirstOrDefault() == false)
                    {
                        total += _KEEP_GUA_EXTRA;
                        keepHisGuaCnt++;
                    }

                    //正常每个挂 权重递减
                    priority = 1 - (lastIndex - i) * _GUA_PRIORITY;
                }
                if (priority > 0)
                {
                    var score = _GUA * priority;
                    total += score > _GUA_BASE ? score : _GUA_BASE;
                }
                else
                    total += _GUA_BASE;

            }

            return total > 100 ? 100 : total;
        }


        /// <summary>
        /// 计算"挂"分数 (结束挂优先)
        /// </summary>
        /// <param name="forcastData"></param>
        /// <returns></returns>
        float CalcuteGua(List<IForcastModel> forcastData, bool startWhenBreakGua, out int keepGuaCnt, out int keepHisGuaCnt)
        {
            if (!startWhenBreakGua)
                return CalcuteGua(forcastData, out keepGuaCnt, out keepHisGuaCnt);

            float total = 0;
            //从最新期开始连挂次数
            keepGuaCnt = 0;
            //历史记录(不包含从最新期连挂的情况)出现连挂次数
            keepHisGuaCnt = 0;

            if (forcastData.Count() < 2)
                throw new Exception("预测历史数据不足,无法进行评估");


            var results = forcastData.Take(forcastData.Count() - 1).Select(f => f.IsWin).Reverse().ToList();
            //第一个非挂
            var firstWinIndex = results.FindIndex(f => f == true);
            //第一个有效挂
            var firstValidWrongIndex = results.Skip(firstWinIndex + 1).ToList().FindIndex(f => f == false) + firstWinIndex + 1;
            //有效连挂次数
            var validGua = results.Skip(firstValidWrongIndex + 1).ToList().FindIndex(f => f == true) + 1;
            //普通挂次数
            var normalGua = results.Skip(firstValidWrongIndex + validGua + 1).Count(f => f == false);

            if (validGua >= _START_GUA_TIME && firstValidWrongIndex > 0 && firstValidWrongIndex <= validGua)
            {
                var baseScore = validGua >= _KEEP_GUA_TIME ? _GUA : (1 - (_KEEP_GUA_TIME - validGua) * _DELTA_REDUCE) * _GUA;
                total += baseScore - (firstValidWrongIndex - 1) * _DELTA_REDUCE * _GUA;
            }
            //过期有效果视为普通挂
            if (firstValidWrongIndex > validGua)
                normalGua += validGua;
            if (normalGua > 0)
                total += _GUA_BASE * normalGua;

            return total;
        }

        /// <summary>
        /// 计算"预测重复率"分数
        /// </summary>
        /// <param name="no1"></param>
        /// <param name="no2"></param>
        /// <returns></returns>
        float CalcuteRepetition(string no1, string no2)
        {
            var repetition = new HashSet<string>(no1.Split(' '));
            var n2 = no2.Split(' ').ToList();
            n2.ForEach(n => repetition.Add(n));

            return float.Parse((_REPETITION * (n2.Count() * 2 - repetition.Count()) * 1.0 / n2.Count).ToString());
        }

        /// <summary>
        /// 计算追号分数
        /// </summary>
        /// <param name="chaseTime"></param>
        /// <returns></returns>
        int CalcuteBetChase(int chaseTime) => _BET_CHASE * (chaseTime - 1);

    }
}
