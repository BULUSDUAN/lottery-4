using System.Threading.Tasks;

namespace LotteryFun.Web.MessageService
{
    public interface IMessageService
    {
        Task Send(string message);
    }
}