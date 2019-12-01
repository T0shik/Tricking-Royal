using System.Collections.Generic;

namespace Battles.Application.ViewModels.Matches
{
    public class BaseMatchViewModel
    {
        public string Key { get; set; }
        public string Mode { get; set; }
        public string Surface { get; set; }
        
        public string[] Chain { get; set; }
        public string TimeLeft { get; set; }
        public int Likes { get; set; }
        public IEnumerable<MatchUserViewModel> Participants { get; set; }
        public IEnumerable<VideoViewModel> Videos { get; set; }
    }
}