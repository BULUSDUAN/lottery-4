using Colin.Lottery.Models;
using Colin.Lottery.Models.BrowserModels;
using Colin.Lottery.Utils;
using MailKit.Security;
using OpenQA.Selenium;

namespace Colin.Lottery.Common
{
    public class ElephantBet
    {
        private readonly BrowserUtil _browser;
        public ElephantBet(string url,string userName,string password)
        {
            _browser = new BrowserUtil();

            ChooseRoute(url);
            LogIn(userName,password);
        }

        

        public bool Bet(LotteryType type, int rule, long periodNo, string betNo, decimal betMoney)
        {
            return false;
        }

        //选择线路
        private void ChooseRoute(string url)
        {
            const string script = @"
                (function(){
                    var jq=document.createElement('script');
                    jq.setAttribute('src','//code.jquery.com/jquery-3.2.1.min.js');
                    document.body.appendChild(jq);
                
                    var chooseRouteTimer= setInterval(function(){
                        if(!$.ready)
                            return;
                        clearInterval(chooseRouteTimer);
                        
                        var minTime=999999;
                        var link;
                        $('.main>div:first >div.main-div .tabboxX2:lt(4) .sitename').each(function(){
                           var time= $(this).next().text().replace('秒','');
                           if(time<minTime){
                               minTime=time;
                               link=$(this).parent().parent('a').attr('href');
                           }
                        });
                        
                        if(!link){
                            console.error('没有提取到有效的线路地址');
                            return;
                        }
                        
                        location.href=link;
                    },1000);
                })();    
            ";
            _browser.Explore(url,null,new Script(script)).Wait();
        }
        
        //登录
        private bool LogIn(string userName,string password)
        {
            const string script = @"

            ";
            _browser.ExecuteScript(new Script(script));

            return true;
        }
    }
}