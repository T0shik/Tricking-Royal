using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace IdentityServer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            DatabaseSeed.EnsureSeed(host.Services);

            host.Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseIISIntegration()
                .UseStartup<Startup>();
    }
}
