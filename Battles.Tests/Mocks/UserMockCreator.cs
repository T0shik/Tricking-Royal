using Battles.Domain.Models;
using Battles.Models;

namespace Battles.Tests.Mocks
{
    public static class UserMockCreator
    {
        public static UserInformation CreateUser() =>
            new UserInformation
            {
                Id = "1",
                DisplayName = "user",
                HostingLimit = 1,
                JoinedLimit = 1
            };

        public static UserInformation CreateHost() =>
            new UserInformation
            {
                Id = "2",
                DisplayName = "host",
                HostingLimit = 1,
                JoinedLimit = 1
            };

        public static UserInformation CreateOpponent() =>
            new UserInformation
            {
                Id = "3",
                DisplayName = "opponent",
                HostingLimit = 1,
                JoinedLimit = 1
            };
    }
}
