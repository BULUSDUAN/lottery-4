namespace Colin.Lottery.Models.AnalyzerModels
{
    public class AnalyzeConfig
    {
        /// <summary>
        /// 单挂分数
        /// </summary>
        public int Gua {get;set;}

        /// <summary>
        /// 挂降权比例
        /// </summary>
        public float GuaPriority {get;set;}

        /// <summary>
        /// 挂降权到0下时，每个挂的基础分数
        /// </summary>
        public int GuaBase {get;set;}

        /// <summary>
        /// 连挂附加分
        /// </summary>
        public int KeepGuaExtra {get;set;}

        /// <summary>
        /// 重复号码总分
        /// </summary>
        public int Repetition {get;set;}

        /// <summary>
        /// 追号次数
        /// </summary>
        public int BetChase {get;set;}


        /// <summary>
        /// 起投连挂次数
        /// </summary>
        public int StartGuaTime {get;set;}

        /// <summary>
        /// 几次有效连挂以上为满分挂
        /// </summary>
        public int KeepGuaTime {get;set;}

        /// <summary>
        /// 有效挂递减比例
        /// </summary>
        public float DeltaReduce {get;set;}


        /// <summary>
        /// 挂 权重(出现挂优先)
        /// </summary>
        public float Pg {get;set;}

        /// <summary>
        /// 重复号码 权重(出现挂优先)
        /// </summary>
        public float Pr {get;set;}

        /// <summary>
        /// 追号 权重(出现挂优先)
        /// </summary>
        public float Pc {get;set;}

        /// <summary>
        /// 挂 权重(结束挂优先)
        /// </summary>
        public float Pgb {get;set;}

        /// <summary>
        /// 重复号码 权重(结束挂优先)
        /// </summary>
        public float Prb {get;set;}

        /// <summary>
        /// 追号 权重(结束挂优先)
        /// </summary>
        public float Pcb {get;set;}

        /// <summary>
        /// 计划员最小胜率
        /// </summary>
        public float MinPrpbability {get;set;}

        /// <summary>
        /// 计划员最大胜率
        /// </summary>
        public float MaxPrpbability {get;set;}
    }
}