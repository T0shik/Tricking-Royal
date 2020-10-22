using System.Linq;
using Battles.Enums;
using Battles.Models;

namespace Battles.Roles
{
    public abstract class HostedMatch : MatchBaseRole
    {
        protected HostedMatch(Match match) : base(match)
        {
        }

        public Host GetHost() => new Host(MatchUsers.First(x => x.Role == MatchRole.Host));
        public string GetHostId() => GetHost().UserId;
        public Host GetOpponent() => new Host(MatchUsers.First(x => x.Role == MatchRole.Opponent));
        public string GetOpponentId() => GetOpponent().UserId;
    }
}