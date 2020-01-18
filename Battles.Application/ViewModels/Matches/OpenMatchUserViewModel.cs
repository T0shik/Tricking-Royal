using System;
using System.Linq.Expressions;
using Battles.Models;

namespace Battles.Application.ViewModels.Matches
{
    public class OpenMatchUserViewModel : MatchUserViewModel
    {
        public int Reputation { get; set; }
        public int Win { get; set; }
        public int Loss { get; set; }
        public int Draw { get; set; }

        public static readonly Expression<Func<MatchUser, OpenMatchUserViewModel>> Projection =
            matchUser => new OpenMatchUserViewModel
            {
                DisplayName = matchUser.User.DisplayName,
                Picture = matchUser.User.Picture,
                Skill = matchUser.User.Skill.ToString(),
                Level = matchUser.User.Level,
                Index = matchUser.Index,
                Role = (int) matchUser.Role,
                Points = matchUser.Points,
                Winner = matchUser.Winner,
                Reputation = matchUser.User.Reputation,
                Win = matchUser.User.Win,
                Loss = matchUser.User.Loss,
                Draw = matchUser.User.Draw,
            };
    }
}