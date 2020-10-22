using System;
using Battles.Enums;
using Battles.Models;

namespace Battles.Tests._Mocks
{
    public static class UserMockCreator
    {
        public static UserInformation CreateUser() =>
            new UserInformation
            {
                Id = Guid.NewGuid().ToString(),
                DisplayName = "user",
                HostingLimit = 1,
                JoinedLimit = 1
            };

        public static MatchUser CreateMatchHost()
        {
            var host = CreateHost();
            return new MatchUser
            {
                UserId = host.Id,
                User = host,
                Role = MatchRole.Host,
            };
        }

        public static MatchUser CreateMatchOpponent()
        {
            var opponent = CreateOpponent();
            return new MatchUser
            {
                UserId = opponent.Id,
                User = opponent,
                Role = MatchRole.Opponent,
            };
        }

        public static UserInformation CreateHost() =>
            new UserInformation
            {
                Id = Guid.NewGuid().ToString(),
                DisplayName = "host",
                HostingLimit = 1,
                JoinedLimit = 1
            };

        public static UserInformation CreateOpponent() =>
            new UserInformation
            {
                Id = Guid.NewGuid().ToString(),
                DisplayName = "opponent",
                HostingLimit = 1,
                JoinedLimit = 1
            };
    }
}