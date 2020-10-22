using System;
using Battles.Enums;
using Battles.Models;

namespace Battles.Roles
{
    public class OpenMatch : HostedMatch
    {
        public OpenMatch(Match match) : base(match)
        {
        }

        public void SetOpponent(UserInformation user)
        {
            MatchUsers.Add(new MatchUser
            {
                Index = MatchUsers.Count,
                UserId = user.Id,
                User = user,
                Role = MatchRole.Opponent
            });
            user.Joined++;
        }

        public virtual bool CanStart() => false;

        public virtual bool Start()
        {
            Status = Status.Active;
            LastUpdate = DateTime.UtcNow;

            return false;
        }
    }
}