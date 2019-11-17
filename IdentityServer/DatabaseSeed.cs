using TrickingRoyal.Database;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Battles.Shared;

namespace IdentityServer
{
    public static class DatabaseSeed
    {
        public static void EnsureSeed(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
                var persistent = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                var configuration = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                persistent.Database.Migrate();
                configuration.Database.Migrate();

                var settings = config.GetSection("OAuth").Get<OAuth>();

                if (!configuration.Clients.Any())
                {
                    foreach (var client in Config.GetClients(settings))
                    {
                        configuration.Clients.Add(client.ToEntity());
                    }

                    configuration.SaveChanges();
                }

                if (!configuration.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        configuration.IdentityResources.Add(resource.ToEntity());
                    }

                    configuration.SaveChanges();
                }

                if (!configuration.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApiResources(settings))
                    {
                        configuration.ApiResources.Add(resource.ToEntity());
                    }

                    configuration.SaveChanges();
                }
            }
        }
    }
}