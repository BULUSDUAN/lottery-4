using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Colin.Lottery.WebApp.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        public ActionResult Login(string account, string password)
        {
            if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password))
            {
                return new ContentResult() { Content = "由于用户名或密码为空，登录失败。" };
            }

            ISession session = HttpContext.Session;
            string sessionId = session.Id;

            return new ContentResult();
        }
    }
}
