using Colin.Lottery.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Colin.Lottery.Common.AutoBetSites
{
    public class SiteBetBase
    {
        protected RestClientHelper _restHelper;

        public SiteBetBase(string domain)
        {
            _restHelper = new RestClientHelper(domain);
        }

        protected void PrintLog(string logFormat, params object[] args)
        {
            Console.WriteLine(string.Format(logFormat, args));
        }
    }
}
