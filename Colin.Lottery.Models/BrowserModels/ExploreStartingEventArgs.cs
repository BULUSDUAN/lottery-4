namespace Colin.Lottery.Models.BrowserModels
{
    public class ExploreStartingEventArgs
    {
        /// <summary>
        /// 爬取目标地址
        /// </summary>
        public string Url { get; }

        public ExploreStartingEventArgs(string url)
        {
            Url = url;
        }
    }
}
