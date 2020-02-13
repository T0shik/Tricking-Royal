using System;
using System.Linq.Expressions;
using Battles.Models;

// ReSharper disable MemberCanBePrivate.Global

namespace Battles.Application.ViewModels
{
    public class UserViewModel
    {
        public string DisplayName { get; set; }
        public string Skill { get; set; }
        public int SkillNorm { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Gym { get; set; }
        public string Information { get; set; }
        public string Instagram { get; set; }
        public string Youtube { get; set; }
        public string Facebook { get; set; }
        public string Picture { get; set; }
        public int Win { get; set; }
        public int Loss { get; set; }
        public int Draw { get; set; }
        public int Reputation { get; set; }
        public int Style { get; set; }
        public int Flags { get; set; }
        public int Hosting { get; set; }
        public int HostingLimit { get; set; }
        public int Joined { get; set; }
        public int JoinedLimit { get; set; }
        public int VotingPower { get; set; }
        public bool Activated { get; set; }
        public int Experience { get; set; }
        public int ExperienceNeed { get; set; }
        public int Level { get; set; }
        public int LevelUpPoints { get; set; }


        public static readonly Expression<Func<UserInformation, UserViewModel>> Projection =
            user => new UserViewModel()
            {
                DisplayName = user.DisplayName,
                Skill = user.Skill.ToString(),
                SkillNorm = (int) user.Skill,
                City = user.City,
                Country = user.Country,
                Gym = user.Gym,
                Information = user.Information,
                Instagram = user.Instagram,
                Facebook = user.Facebook,
                Youtube = user.Youtube,
                Picture = user.Picture,
                Win = user.Win,
                Loss = user.Loss,
                Draw = user.Draw,
                Reputation = user.Reputation,
                Style = user.Style,
                Flags = user.Flags,
                Hosting = user.Hosting,
                Joined = user.Joined,
                HostingLimit = user.HostingLimit,
                JoinedLimit = user.JoinedLimit,
                VotingPower = user.VotingPower,
                Activated = user.Activated,
                Experience = user.Experience,
                Level = user.Level,
                LevelUpPoints = user.LevelUpPoints,
            };
    }
}