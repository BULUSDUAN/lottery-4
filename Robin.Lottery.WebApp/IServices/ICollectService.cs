namespace Robin.Lottery.WebApp.IServices
{
    public interface ICollectService
    {
        string Request(string ruleValue, string game = "pk10");
    }
}