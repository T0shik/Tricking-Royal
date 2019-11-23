using System.Net.Http.Headers;
using Battles.Application.Jobs;
using Battles.Application.Services.Users.Queries;
using Battles.Configuration;
using Hangfire;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Models;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Battles.Api.Infrastructure;
using Battles.Api.Workers;
using Battles.Api.Workers.Notifications;
using Battles.Api.Workers.Notifications.Settings;
using Battles.Application.SubServices;
using Battles.Shared;
using Microsoft.Extensions.Logging;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using TrickingRoyal.Database;

namespace Battles.Api
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;
        private readonly OAuth _oAuth;

        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
            _oAuth = _config.GetSection("OAuth").Get<OAuth>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<OneSignal>(_config.GetSection("OneSignal"));
            services.Configure<AppSettings>(_config.GetSection("AppSettings"));

            services.AddSingleton(_oAuth);
            services.AddSingleton(_oAuth.Routing);

            var connectionString = _config.GetConnectionString("DefaultConnection");
            services.AddTrickingRoyalDatabase(connectionString)
                    .AddHangfireServices()
                    .AddHangfire(options => options.UseSqlServerStorage(connectionString));

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = _oAuth.Routing.Server;
                        options.RequireHttpsMetadata = _env.IsProduction();

                        options.ApiName = _oAuth.Api.Name;
                        options.ApiSecret = _oAuth.Api.ResourceSecret.Sha256();

                        options.TokenRetriever = httpRequest =>
                        {
                            var fromHeader = TokenRetrieval.FromAuthorizationHeader();
                            var fromQuery = TokenRetrieval.FromQueryString();
                            return fromHeader(httpRequest) ?? fromQuery(httpRequest);
                        };
                    });

            SetupCors(services);

            services.AddBattlesServices()
                    .AddSubServices()
                    .AddWorkers()
                    .AddMediatR(typeof(GetUserQuery).GetTypeInfo().Assembly)
                    .AddHttpClient("default",
                                   config =>
                                   {
                                       config.DefaultRequestHeaders.Accept
                                             .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                   });


            var emailSettings = _config.GetSection(nameof(MailKitOptions)).Get<MailKitOptions>();
            services.AddMailKit(optionBuilder => { optionBuilder.UseMailKit(emailSettings); });

            services.AddHealthChecks();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseCors(_env.IsDevelopment() ? "AllowAll" : "AllowClients");
            app.UseHealthChecks("/healthcheck");

            if (_env.IsProduction())
            {
                loggerFactory.AddFile("Logs/api-{Date}.log");
            }

            app.UseHangfireServer();
            SetupHangfireJobs();

            app.UseAuthentication()
               .UseMvc();
        }

        private void SetupCors(IServiceCollection services)
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
                services.AddCors(options =>
                {
                    options.AddPolicy("AllowClients",
                                      p => p.WithOrigins(_oAuth.Routing.Client, _config["HealthChecker"])
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowCredentials());
                });
            }
        }

        private static void SetupHangfireJobs()
        {
            RecurringJob.AddOrUpdate<IHangfireJobs>(jobs => jobs.CloseExpiredMatches(), Cron.Hourly);
            RecurringJob.AddOrUpdate<IHangfireJobs>(jobs => jobs.CloseExpiredEvaluations(), Cron.Hourly);
            RecurringJob.AddOrUpdate<IHangfireJobs>(jobs => jobs.MatchReminder(), Cron.Daily);
        }
    }
}