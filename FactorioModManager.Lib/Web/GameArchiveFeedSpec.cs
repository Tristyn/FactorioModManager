namespace FactorioModManager.Lib.Web
{
    class GameArchiveFeedSpec
    {
        public bool AllowExperimental { get; }
        public bool OnlyHeadlessServer { get; }

        public GameArchiveFeedSpec(bool allowExperimental, bool onlyHeadlessServer)
        {
            AllowExperimental = allowExperimental;
            OnlyHeadlessServer = onlyHeadlessServer;
        }
    }
}
