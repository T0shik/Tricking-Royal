using Battles.Configuration;

namespace Battles.Tests.Mocks
{
    public static class RoutingMock
    {
        public static Routing Create()
        {
            return new Routing
            {
                Server = "server",
                Api = "api",
                Cdn = "cdn",
                Client = "client"
            };
        }
    }
}