using System.Collections.Generic;
using Robin.Lottery.WebApp.Models;

namespace Robin.Lottery.WebApp.MQ
{
    /// <summary>
    ///     计划号码消息
    /// </summary>
    public class LotteryPlanMessage : PlanNo
    {
        public LotteryPlanMessage(Planner planner, IList<PlanNo> listPlanNo, Pk10Rule pk10Rule)
        {
            Planner = planner;
            ListPlanNo = listPlanNo;
            Pk10Rule = pk10Rule;
        }

        public Pk10Rule Pk10Rule { get; set; }

        public Planner Planner { get; set; }

        public IList<PlanNo> ListPlanNo { get; set; }
    }
}