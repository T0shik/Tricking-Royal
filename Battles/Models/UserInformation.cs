using System.Collections.Generic;
using Battles.Enums;

namespace Battles.Models
{
    public class UserInformation
    {
        public UserInformation()
        {
            VotingPower = 1;
            Level = 1;
            UserMatches = new List<MatchUser>();
            Decisions = new List<Decision>();
            Notifications = new List<NotificationMessage>();
            NotificationConfigurations = new List<NotificationConfiguration>();
        }

        public string Id { get; set; }
        public bool Activated { get; set; }

        public string DisplayName { get; set; }
        public Skill Skill { get; set; }
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
        public int Experience { get; set; }
        public int Level { get; set; }
        public int LevelUpPoints { get; set; }
        public ICollection<MatchUser> UserMatches { get; }
        public ICollection<Decision> Decisions { get; }
        public ICollection<NotificationMessage> Notifications { get; }
        public ICollection<NotificationConfiguration> NotificationConfigurations { get; }
    }
}