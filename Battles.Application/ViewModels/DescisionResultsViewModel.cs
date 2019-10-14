namespace Battles.Application.ViewModels
{
    public class DecisionResultViewModel
    {
        public int HostVotes { get; set; }
        public int OpponentVotes { get; set; }
        public int HostPercent { get; set; }
        public int OpponentPercent { get; set; }
        public int Winner { get; set; }
    }
}