using System;
using System.Linq.Expressions;
using Battles.Enums;
using Battles.Models;

namespace Battles.Application.ViewModels.Matches
{
    public class MatchUserViewModel
    {
        public string DisplayName { get; set; }
        public string Picture { get; set; }
        public string Skill { get; set; }
        public int Level { get; set; }
        public int Index { get; set; }
        public int Role { get; set; }
        public int Points { get; set; }
        public bool Winner { get; set; }

        public static readonly Expression<Func<MatchUser, MatchUserViewModel>> Projection =
            matchUser => new MatchUserViewModel
            {
                DisplayName = matchUser.User.DisplayName,
                Picture = matchUser.User.Picture,
                Skill = matchUser.User.Skill.ToString().ToLower(),
                Level = matchUser.User.Level,
                Index = matchUser.Index,
                Role = (int) matchUser.Role,
                Points = matchUser.Points,
                Winner = matchUser.Winner,
            };
    }
}