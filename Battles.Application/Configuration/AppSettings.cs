namespace Battles.Application.Configuration
{
    public class AppSettings
    {
        public int MatchListSize { get; set; }
        public int EvalExpiry { get; set; }
        public int FlagExpiry { get; set; }
        public string MatchPath { get; set; }
        public string UserPath { get; set; }
    }
}