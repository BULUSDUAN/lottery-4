using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Colin.Lottery.Collectors;
using Colin.Lottery.Models;
using Colin.Lottery.Models.AnalyzerModels;
using Colin.Lottery.Models.BetService;
using Colin.Lottery.Utils;

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
            return await JinMaCollector.GetForcastData(type, planer, rule);
        }

        public override async Task<List<IForcastPlanModel>> GetForcastData(LotteryType type,
            int rule)
        {
            var planA = await GetForcastData(type, Planner.Planner1, rule);
            var planB = await GetForcastData(type, Planner.Planner2, rule);
            return new List<IForcastPlanModel>{planA,planB};
        }

        public override async Task<List<IForcastPlanModel>> GetForcastData(LotteryType type)
        {
            int rule;
            switch (type)
            {
                case LotteryType.Cqssc:
                    rule = (int) CqsscRule.One;
                    break;
                case LotteryType.Pk10:
                    rule = (int) Pk10Rule.Champion;
                    break;
                default:
                    throw new Exception("彩种暂不支持");
            }

            return await GetForcastData(type, rule);
        }

        public override async Task<List<IForcastPlanModel>> GetForcastData()
        {
            return await GetForcastData(LotteryType.Pk10, (int) Pk10Rule.Champion);
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
         * 1.每一个挂为 GUA 分
         *  距最新开奖段每远一段权重下降 GUA_PRIORITY(0%<=GUA_PRIORITY<=100%),从最近一期开始的连挂,每个挂权重均为100%
         *  历史记录(不包含从最新期连挂的情况)出现连挂，每个连股期数都附加KEEPGUA_EXTRA分
         *  所有挂分数超过100分按100分计算
         *  出现多期连挂，最后单独处理
         * 2.两个计划员预测当前期结果完全相同为 REPETITION 分,权重为预测结果相同比例 X(0%<=X<=100%) 
         * 3.单次跟投为 BET_CHASE 分,第 N 次跟投 为 (N-1)*BET_CHASE 分
         * 4.计划员预测准确率 P 为全局系数 
         *  正常情况下 MIN_PRPBABILITY <= P <= MAX_PRPBABILITY 我们将MIN_PRPBABILITY ~ MIN_PRPBABILITY映射为0%-100%，换算公式如下 
         *  (P-MIN_PRPBABILITY)*1/(MAX_PRPBABILITY-MIN_PRPBABILITY)
         * 5. 1，2，3的计算分数分别占比为 PG:PR:PC
         * 若出现最近期连挂或者计划员预测号码重复度100%则分数为100
         * 若计划员号码重复度X>=80%则分数为 REPETITION*X
         */

        /*
        * 连挂结束跟投策略
        * 
        * 假定有效连挂(距离当前期最近的已经结束的连挂)次数为N
        * 1.N >= START_GUA_TIME
        * 2.有效连挂基础分算法
        *   N >= KEEP_GUA_TIME 时基础分满分GUA
        *   N < KEEP_GUA_TIME 时基础分按照 (KEEP_GUA_TIME-N)*DELTA_REDUCE 递减
        * 3.段位分递减算法
        *   我们认为连挂N段后，可以安全跟投N段，因为结束连挂已经过了一段，所以还可以安全跟投 N-1 次
        *   每多跟投一段安全性降低一点，所以段位分随着跟投段位递减，假定当前为第 M 次跟投(1< M < N) 段位分递减公式为
        *   (M-1)*DELTA_REDUCE*GUA
        * 4.有效连挂之前的历史记录中每出现一个挂记录记为GUA_BASE分，过期有效挂视为普通挂
        * 
        */

        private static readonly AnalyzeConfig Config = ConfigUtil.GetAppSettings<AnalyzeConfig>("AnalyzeConfig");
        public override void CalcuteScore(List<IForcastPlanModel> plans,
            bool startWhenBreakGua)
        {
            if (plans == null || plans.Count<2)
            return;            

            var repetition = CalcuteRepetition(plans[0].ForcastDrawNo,plans[1].ForcastDrawNo);
            plans.ForEach(plan =>
            {
                plan.RepetitionScore = repetition;
                plan.GuaScore = CalcuteGua(plan.ForcastData, startWhenBreakGua, out var keepGuaCnt, out var currentGuaCnt);
                plan.KeepGuaCnt = keepGuaCnt;
                plan.BetChaseScore = CalcuteBetChase(plan.ForcastData.LastOrDefault().ChaseTimes);
                
                if (!startWhenBreakGua)
                {
                    if (keepGuaCnt > 1 || repetition >=Config.Repetition)
                        plan.Score = 100;
                    else
                    {
                        var score = (plan.GuaScore * Config.Pg + repetition * Config.Pr + plan.BetChaseScore * Config.Pc) * (plan.WinProbability - Config.MinPrpbability) /
                                 (Config.MaxPrpbability - Config.MinPrpbability);
                        plan.Score = repetition >= Config.Repetition ? repetition : score;
                    }
                }
                else
                {
                    if (plan.GuaScore >= 90 || repetition >= Config.Repetition)
                        plan.Score = 100;
                    else
                    {
                        var score = (plan.GuaScore * Config.Pgb + repetition * Config.Prb + plan.BetChaseScore * Config.Pcb) * (plan.WinProbability - Config.MinPrpbability) /
                                 (Config.MaxPrpbability - Config.MinPrpbability);
                        plan.Score = repetition >= Config.Repetition ? repetition : score;
                    }
                }
            });
        }


        /// <summary>
        /// 计算"挂"分数 (出现挂优先)
        /// </summary>
        /// <param name="forcastData"></param>
        /// <param name="keepGuaCnt"></param>
        /// <param name="keepHisGuaCnt"></param>
        /// <returns></returns>
        private static float CalcuteGua(IReadOnlyCollection<IForcastModel> forcastData, out int keepGuaCnt,
            out int keepHisGuaCnt)
        {
            float total = 0;
            //从最新期开始连挂次数
            keepGuaCnt = 0;
            //历史记录(不包含从最新期连挂的情况)出现连挂次数
            keepHisGuaCnt = 0;

            if (forcastData.Count < 2)
                throw new Exception("预测历史数据不足,无法进行评估");

            var results = forcastData.Take(forcastData.Count - 1).Select(f => f.IsWin);
            var enumerable = results as bool?[] ?? results.ToArray();
            var lastIndex = enumerable.Length - 1;
            for (var i = lastIndex; i >= 0; i--)
            {
                var isWin = enumerable.ElementAt(i);
                if (isWin != false)
                    continue;

                //从最新一期非连挂
                var breakGua = enumerable.Skip(i).Take(enumerable.Length - i).Any(w => w != false);

                float priority;

                //从最近一期连挂 所有挂的期数权重均为100%
                if (!breakGua)
                {
                    priority = 1;
                    keepGuaCnt++;
                }
                else
                {
                    //历史记录(不包含从最新期连挂的情况)出现连挂，每个连挂期数都附加相应分数
                    if (enumerable.Skip(i + 1).Take(1).FirstOrDefault() == false)
                    {
                        total += Config.KeepGuaExtra;
                        keepHisGuaCnt++;
                    }

                    //正常每个挂 权重递减
                    priority = 1 - (lastIndex - i) * Config.GuaPriority;
                }

                if (priority > 0)
                {
                    var score = Config.Gua * priority;
                    total += score > Config.GuaBase ? score : Config.GuaBase;
                }
                else
                    total += Config.GuaBase;
            }

            return total > 100 ? 100 : total;
        }


        /// <summary>
        /// 计算"挂"分数 (结束挂优先)
        /// </summary>
        /// <param name="forcastData"></param>
        /// <param name="startWhenBreakGua"></param>
        /// <param name="keepGuaCnt">有效挂(连挂已结束并仍处于有效期内)数量</param>
        /// <param name="currentGuaCnt"></param>
        /// <returns></returns>
        private static float CalcuteGua(IReadOnlyCollection<IForcastModel> forcastData, bool startWhenBreakGua, out int keepGuaCnt,
            out int currentGuaCnt)
        {
            if (!startWhenBreakGua)
                return CalcuteGua(forcastData, out keepGuaCnt, out currentGuaCnt);

            float total = 0;
            //从最新期开始连挂次数
            keepGuaCnt = 0;
            //历史记录(不包含从最新期连挂的情况)出现连挂次数
            currentGuaCnt = 0;

            if (forcastData.Count < 2)
            {
                LogUtil.Error("预测历史数据不足,无法进行评估");
                return 0;
            }


            var results = forcastData.Take(forcastData.Count - 1).Select(f => f.IsWin).Reverse().ToList();
            //第一个非挂
            var firstWinIndex = results.FindIndex(f => f == true);
            //最近挂尚未结束
            if (firstWinIndex > 0)
            {
                total = results.Count(f => f == false) * Config.GuaBase;
                currentGuaCnt = firstWinIndex;
            }
            else
            {
                //第一个有效挂
                var firstValidWrongIndex =
                    results.Skip(firstWinIndex + 1).ToList().FindIndex(f => f == false) + firstWinIndex + 1;
                //有效连挂次数
                var validGua = results.Skip(firstValidWrongIndex + 1).ToList().FindIndex(f => f == true) + 1;
                //普通挂次数
                var normalGua = results.Skip(firstValidWrongIndex + validGua + 1).Count(f => f == false);

                if (validGua >= Config.StartGuaTime && firstValidWrongIndex > 0 && firstValidWrongIndex < validGua)
                {
                    var baseScore = validGua >= Config.KeepGuaTime
                        ? Config.Gua
                        : (1 - (Config.KeepGuaTime - validGua) * Config.DeltaReduce) * Config.Gua;
                    total += baseScore - (firstValidWrongIndex+1) * Config.DeltaReduce * Config.Gua;

                    keepGuaCnt = validGua;
                }

                //过期有效挂视为普通挂
                if (firstValidWrongIndex >= validGua)
                    normalGua += validGua;
                if (normalGua > 0)
                    total += Config.GuaBase * normalGua;
            }

            return total;
        }

        /// <summary>
        /// 计算"预测重复率"分数
        /// </summary>
        /// <param name="no1"></param>
        /// <param name="no2"></param>
        /// <returns></returns>
        private static float CalcuteRepetition(string no1, string no2)
        {
            var repetition = new HashSet<string>(no1.Split(' '));
            var n2 = no2.Split(' ').ToList();
            n2.ForEach(n => repetition.Add(n));

            return float.Parse((Config.Repetition * (n2.Count * 2 - repetition.Count) * 1.0 / n2.Count).ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 计算追号分数
        /// </summary>
        /// <param name="chaseTime"></param>
        /// <returns></returns>
        private static int CalcuteBetChase(int chaseTime) => Config.BetChase * (chaseTime - 1);
    }
}