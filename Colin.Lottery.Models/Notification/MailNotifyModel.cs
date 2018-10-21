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

        public IForecastPlanModel ForecastPlan { get; }

        public IForecastModel CurrentPeriod { get; }

        public IEnumerable<IForecastModel> PassedPeriod { get; }

        public string PartnerForecastNo { get; }

        public MailNotifyModel(LotteryType lottery, int rule, Plan plan, IForecastPlanModel forecastPlan, string partnerForecastNo)
        {
            Lottery = lottery.ToStringName();
            Rule = rule.ToStringName(lottery);
            Plan = plan;
            ForecastPlan = forecastPlan;

            CurrentPeriod = ForecastPlan.ForecastData.LastOrDefault();
            PassedPeriod = ForecastPlan.ForecastData.Take(ForecastPlan.ForecastData.Count - 1);
            PartnerForecastNo = partnerForecastNo;
        }

        #region NVelocity模板帮助方法

        public string ToPeriodNo(long periodNo) => periodNo.ToString().PadLeft(3, '0');

        #endregion

    }
}