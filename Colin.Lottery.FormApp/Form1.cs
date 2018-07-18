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

        /// <summary>
        /// 选择彩种
        /// </summary>
        /// <param name="type"></param>
        private void ChooseLotteryType(LotteryType type = LotteryType.Pk10)
        {
            var dictNames = new Dictionary<LotteryType, string>
            {
                [LotteryType.Pk10] = "北京PK10",
                [LotteryType.Cqssc] = "重庆时时彩"
            };

            string name = dictNames[type];

            //导航到主页
            
        }

        private void NavigateHome()
        {
            
        }
    }
}
