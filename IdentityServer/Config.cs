using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityServer.Configuration;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources(OAuth settings)
        {
            return new List<ApiResource>
            {
                new ApiResource(settings.Api.Name, "Information In/Out Point")
                {
                    ApiSecrets =
                    {
                        new Secret(settings.Api.ResourceSecret.Sha256())
                    }
                },
                new ApiResource(settings.Cdn.Name, "Content Distributing Network")
                {
                    ApiSecrets =
                    {
                        new Secret(settings.Cdn.ResourceSecret.Sha256())
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients(OAuth settings)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "vue",
                    ClientName = "Vue Client App",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,

                    RedirectUris =
                    {
                        $"{settings.Routing.Client}/static/callback.html",
                        $"{settings.Routing.Client}/static/silent.html",
                    },
                    PostLogoutRedirectUris = {$"{settings.Routing.Client}"},
                    AllowedCorsOrigins = {$"{settings.Routing.Client}"},

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        settings.Api.Name,
                        settings.Cdn.Name,
                    },
                    AllowOfflineAccess = true,
                    IdentityTokenLifetime = 3600,
                    AccessTokenLifetime = 3600 * 24,
                },
                // new Client
                // {
                //     ClientId = "ionic-TrickingR-app-H",
                //     ClientName = "Ionic App Client",
                //     AllowedGrantTypes = GrantTypes.Implicit,
                //
                //     AllowAccessTokensViaBrowser = true,
                //     RequireConsent = false,
                //
                //     RedirectUris = {"http://localhost:8100"},
                //     PostLogoutRedirectUris = {"http://localhost:8100"},
                //     AllowedCorsOrigins = {"http://localhost:8100"},
                //
                //     AllowedScopes =
                //     {
                //         IdentityServerConstants.StandardScopes.OpenId,
                //         IdentityServerConstants.StandardScopes.Profile,
                //         settings.Api.Name,
                //         settings.Cdn.Name,
                //     },
                //
                //     AllowOfflineAccess = true
                // }
            };
        }
    }
}