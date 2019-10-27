using IdentityServer.Configuration;
using IdentityServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using TrickingRoyal.Database;

namespace IdentityServer
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;

        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<OAuth>(_config.GetSection(nameof(OAuth)));
            var routing = _config.GetSection("OAuth").Get<OAuth>().Routing;

            services.Configure<CookiePolicyOptions>(options =>
            {
                //                options.CheckConsentNeeded = context => _env.IsDevelopment()
                //                    ? !context.Request.Path.StartsWithSegments("/Server")
                //                      && !context.Request.Path.StartsWithSegments("/connect")
                //                      && !context.Request.PathBase.Value.Contains("api")
                //                    : !context.Request.Path.StartsWithSegments("/connect")
                //                      && !context.Request.Host.Value.Contains("api");
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            if (_env.IsProduction())
            {
                services.AddDataProtection()
                        .SetApplicationName("IdentityServer")
                        .PersistKeysToFileSystem(new DirectoryInfo(_config["MachineKeys"]));
            }

            services.AddIdentity<ApplicationUser, IdentityRole>(IdentitySetupAction())
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            var connectionString = _config.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                                                    options.UseSqlServer(connectionString));

            AddIdentityServerConfiguration(services, connectionString);

            var facebook = _config.GetSection(nameof(Facebook)).Get<Facebook>();
            services.AddAuthentication()
                    .AddFacebook(options =>
                    {
                        options.AppId = facebook.Id;
                        options.AppSecret = facebook.Secret;
                        options.Events.OnRemoteFailure = (context) =>
                        {
                            context.Response.Redirect("/account/login");
                            context.HandleResponse();
                            return System.Threading.Tasks.Task.FromResult(0);
                        };
                    });

            SetupCors(services, routing.Client);

            var emailSettings = _config.GetSection(nameof(MailKitOptions)).Get<MailKitOptions>();
            services.AddMailKit(optionBuilder => { optionBuilder.UseMailKit(emailSettings); });

            services.AddHealthChecks();
            services.AddMvc();
        }

        public void Configure(
            IApplicationBuilder app,
            ILoggerFactory loggerFactory)
        {
            app.UseCors(_env.IsDevelopment() ? "AllowAll" : "AllowClients");
            app.UseHealthChecks("/healthcheck");

            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage()
                   .UseDatabaseErrorPage();
            }
            else
            {
                loggerFactory.AddFile("Logs/server-{Date}.txt");
                app.Use((context, next) =>
                {
                    context.Request.Scheme = "https";
                    return next();
                });

                app.UseExceptionHandler("/Shared/Error");
            }

            app.UseCookiePolicy()
               .UseStaticFiles()
               .UseIdentityServer()
               .UseMvcWithDefaultRoute();
        }

#region Helpers

        private void AddIdentityServerConfiguration(
            IServiceCollection services,
            string connectionString)
        {
            var identityServiceBuilder = services.AddIdentityServer(options =>
            {
                options.Authentication.CookieSlidingExpiration = true;
                options.Authentication.CookieLifetime = TimeSpan.FromDays(30);
            });

            if (!_env.IsDevelopment())
            {
                identityServiceBuilder.AddDeveloperSigningCredential();
            }
            else
            {
                var appSecrets = _config.GetSection("AppSecrets").Get<AppSecrets>();
                var fileName = Path.Combine(_env.ContentRootPath, "token.pfx");
                if (!File.Exists(fileName))
                {
                    throw new FileNotFoundException("Signing Certificate is missing.");
                }

                identityServiceBuilder.AddSigningCredential(new X509Certificate2(fileName, appSecrets.CertPassword));
            }

            var migrationsAssembly = typeof(AppDbContext).GetTypeInfo().Assembly.GetName().Name;
            identityServiceBuilder.AddConfigurationStore(options =>
                                  {
                                      options.ConfigureDbContext = builder =>
                                          builder.UseSqlServer(connectionString,
                                                               sql => sql.MigrationsAssembly(migrationsAssembly));
                                  })
                                  .AddOperationalStore(options =>
                                  {
                                      options.ConfigureDbContext = builder =>
                                          builder.UseSqlServer(connectionString,
                                                               sql => sql.MigrationsAssembly(migrationsAssembly));

                                      options.EnableTokenCleanup = true;
                                      options.TokenCleanupInterval = 3600 * 24;
                                  })
                                  .AddAspNetIdentity<ApplicationUser>()
                                  .AddProfileService<IdentityProfileService>();
        }

        private void SetupCors(IServiceCollection services, string clientUrl)
        {
            if (_env.IsDevelopment())
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll",
                                      p => p.AllowAnyOrigin()
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowCredentials());
                });
            }
            else
            {
                services.AddHttpsRedirection(options => { options.HttpsPort = 443; });

                services.AddCors(options =>
                {
                    options.AddPolicy("AllowClients",
                                      p => p.WithOrigins(clientUrl, _config["HealthChecker"])
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowCredentials());
                });
            }
        }

        private Action<IdentityOptions> IdentitySetupAction()
        {
            if (_env.IsProduction())
            {
                return options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedEmail = true;
                };
            }

            return options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            };
        }

#endregion
    }
}