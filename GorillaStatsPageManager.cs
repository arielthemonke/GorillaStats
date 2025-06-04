using System.Collections.Generic;

namespace GorillaStats
{
    public static class GorillaStatsPageManager
    {
        public static List<IGorillaStatsPage> GorillaStatsPages = new List<IGorillaStatsPage>();

        public static void RegisterPage(IGorillaStatsPage page)
        {
            if (GorillaStatsPages.Contains(page)) return;
            GorillaStatsPages.Add(page);
        }
    }
}