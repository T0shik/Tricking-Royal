using Battles.Shared;

namespace Battles.Tests._Mocks
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