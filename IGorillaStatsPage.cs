namespace GorillaStats
{
    public interface IGorillaStatsPage
    {
        string PageName { get; }
        string GetPageText();
    }
}