using System.Collections.Generic;
using Battles.Models;

namespace Battles.Application.ViewModels.Matches
{
    public abstract class BaseMatchViewModel<TUser> where TUser : MatchUserViewModel
    {
        public BaseMatchViewModel()
        {
            Participants = new List<TUser>();
            Videos = new List<VideoViewModel>();
        }
        
        public string Key { get; set; }
        public string Mode { get; set; }
        public string Surface { get; set; }

        public string[] Chain { get; set; }
        public string TimeLeft { get; set; }
        public int Likes { get; set; }
        public IEnumerable<TUser> Participants { get; set; }
        public IEnumerable<VideoViewModel> Videos { get; set; }
    }
}