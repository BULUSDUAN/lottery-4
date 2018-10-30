namespace Robin.Lottery.WebApp.Models
{
    public sealed class AppConfig
    {
        public string QuartzCronExp { get; set; }
        
        public int MinLongQueue { get; set; }
        
        public string PlanSiteDomain { get; set; }
    }
}