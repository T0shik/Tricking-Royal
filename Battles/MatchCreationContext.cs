using System;
using Battles.Enums;
using Battles.Models;
using Battles.Rules.Matches;

namespace Battles
{
    public class MatchCreationContext: IContext
    {
        public MatchCreationContext()
        { }
        
        public Match CreateMatch(MatchSettings settings)
        {
            if (settings.Host == null)
            {
                throw new UserNotFoundException("Host profile not found.");
            }

            if (settings.Host.Hosting >= settings.Host.HostingLimit)
            {
                throw new HostingLimitReachedException("Hosting limit reached.");
            }

            //todo create tests for invitations
            var match = new Match
            {
                Mode = settings.Mode,
                TurnType = settings.TurnType,
                TurnDays = settings.TurnTime,
                Surface = settings.Surface,
                Created = DateTime.Now,
            };

            settings.Host.Hosting++;

            match.MatchUsers.Add(new MatchUser
            {
                UserId = settings.Host.Id,
                User = settings.Host,
                Role = MatchRole.Host,
            });

            return match;
        }
        
        public class MatchSettings
        {
            public string UserId { get; set; }
            public Mode Mode { get; set; }
            public TurnType TurnType { get; set; }
            public Surface Surface { get; set; }
            public int TurnTime { get; set; }
            public UserInformation Host { get; set; }
            public bool IsInvite { get; set; }
        }
    }
}