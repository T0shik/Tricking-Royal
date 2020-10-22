using Battles.Models;

namespace Battles.Roles
{
    public abstract class ActiveMatch : HostedMatch
    {
        public ActiveMatch(Match match) : base(match)
        {}
    }
}