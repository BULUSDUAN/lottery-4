using System;
using System.Collections.Generic;

namespace Colin.Lottery.Common.AutoBetSites
{
    public class SiteStateCache
    {
        private static IDictionary<string, ISiteBet> SiteContainer
            = new Dictionary<string, ISiteBet>();



        public static void AddSite(string key, ISiteBet site)
        {
            if (string.IsNullOrWhiteSpace(key) || site == null)
            {
                throw new ArgumentNullException(nameof(key), "KEY 或 VALUE 不能为空. ");
            }

            if (SiteContainer.ContainsKey(key))
            {
                SiteContainer[key] = site;
            }
            else
            {
                SiteContainer.Add(key, site);
            }
        }

        public static ISiteBet TryGet(string key)
        {
            if (SiteContainer.TryGetValue(key, out var site))
            {
                return site;
            }

            return null;
        }

        public static void TryRemove(string key)
        {

        }
    }
}
