using System;
namespace Colin.Lottery.Models
{
    public class ExploreStartingEventArgs
    {
        /// <summary>
        /// 爬取目标地址
        /// </summary>
        public string Url { get; set; }

        public ExploreStartingEventArgs(string url)
        {
            Url = url;
        }
    }
}
