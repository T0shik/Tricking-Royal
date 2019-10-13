using System;
using Battles.Domain.Models;
using Battles.Enums;
using Battles.Models;

namespace Battles.Rules.Matches.Actions.Create
{
    public static class MatchCreator
    {
        public static Match CreateMatch(MatchSettings settings)
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
    }
}