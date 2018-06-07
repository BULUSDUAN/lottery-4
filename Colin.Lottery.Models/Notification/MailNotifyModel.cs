using System.Collections.Generic;
using System.Linq;

namespace Colin.Lottery.Models.Notification
{
    /// <summary>
    /// 邮件通知模型
    /// </summary>
    public class MailNotifyModel
    {
        public string Lottery { get; }

        public string Rule { get; }

        public Plan Plan { get; }

        public Plan PartnerPlan => Plan == Plan.PlanA ? Plan.PlanB : Plan.PlanA;

        public IForcastPlanModel ForcastPlan { get; }

        public IForcastModel CurrentPeriod { get; }

        public IEnumerable<IForcastModel> PassedPeriod { get; }

        public string PartnerForcastNo { get; }

        public MailNotifyModel(LotteryType lottery, int rule, Plan plan, IForcastPlanModel forcastPlan, string partnerForcastNo)
        {
            Lottery = lottery.ToStringName();
            Rule = rule.ToStringName(lottery);
            Plan = plan;
            ForcastPlan = forcastPlan;

            CurrentPeriod = ForcastPlan.ForcastData.LastOrDefault();
            PassedPeriod = ForcastPlan.ForcastData.Take(ForcastPlan.ForcastData.Count - 1);
            PartnerForcastNo = partnerForcastNo;
        }

        #region NVelocity模板帮助方法

        public string ToPeriodNo(long periodNo) => periodNo.ToString().PadLeft(3, '0');

        #endregion

    }
}