using Colin.Lottery.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Colin.Lottery.FormApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            browser.DocumentCompleted += Brower_DocumentCompleted;
        }

        private void Brower_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (browser.ReadyState != WebBrowserReadyState.Complete)
                return;

            switch (browser.Url.LocalPath)
            {
                case "/":
                    ChooseRoutes();
                    break;
                case "/Home/TradeAgreements":
                    AgreeAgreement();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 选择线路
        /// </summary>
        private void ChooseRoutes()
        {
            //C# 版本
            //browser.WaitUtil(
            //     wb => !string.IsNullOrWhiteSpace(wb.Invoke(new Func<string>(() => wb.Document.GetElementById("click_url_0").GetAttribute("onclick"))) as string),
            //     wb => wb.Invoke(new Action(() => wb.Document.GetElementById("click_url_0").InvokeMember("click"))),
            //     TimeSpan.FromSeconds(10)
            //    );

            //Js 版本
            browser.ExecuteScript(@"
let timer= setInterval(function(){
    if(!!$('#click_url_0').attr('onclick')){
        $('#click_url_0').click();
        clearInterval(timer);
    }
},1000);");
        }

        /// <summary>
        /// 同意协议
        /// </summary>
        private void AgreeAgreement()
        {
            //browser.Document.GetElementById("btn_OK").InvokeMember("click");

            browser.ExecuteScript("$('#btn_OK').click()");
        }



        //彩种、玩法、期号、号码、下注金额
        private void Bet(LotteryType type, int rule, long periodNo, string betNo, decimal betMoney)
        {
            var dictNames = new Dictionary<LotteryType, int>
            {
                [LotteryType.Pk10] = 19,
                [LotteryType.Cqssc] = 1
            };

            StringBuilder sb = new StringBuilder();
            //选择彩种
            var typeId = dictNames[type];
            sb.AppendLine($@"
                var win=document.getElementById('mainFrame').contentWindow;

                //选择彩种
                var parts = win.location.pathname.split('/');
                var typeId = parts[parts.length - 1];
                if (typeId !={typeId})
                {{
                    parts[parts.length - 1] ={ typeId};
                    win.location.pathname = parts.join('/');
                }}
            ");
        }
    }
}
