﻿using System.Net.Http.Headers;
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
using Battles.Api.Notifications;
using Battles.Api.Settings;
using Battles.Application.SubServices;
using Microsoft.Extensions.Logging;
using TrickingRoyal.Database;
using Routing = Battles.Configuration.Routing;

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
            _oAuth = _config.GetSection(nameof(OAuth)).Get<OAuth>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<OneSignal>(_config.GetSection(nameof(OneSignal)));
            services.Configure<AppSettings>(_config.GetSection(nameof(AppSettings)));

            var email = _config.GetSection(nameof(EmailSettings)).Get<EmailSettings>();

            services.AddSingleton(_oAuth.Routing);
            services.AddSingleton(email);

            var connectionString = _config.GetConnectionString("DefaultConnection");
            services.AddTrickingRoyalDatabase(connectionString)
                .AddHangfireServices()
                .AddHangfire(options =>
                    options.UseSqlServerStorage(connectionString));

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
                .AddNotificationServices()
                .AddSubServices()
                .AddMediatR(typeof(GetUserQuery).GetTypeInfo().Assembly)
                .AddHttpClient("default",
                    config =>
                    {
                        config.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
                    });

            services.AddMvc();
        }    

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            if (_env.IsProduction())
            {
                loggerFactory.AddFile("Logs/api-{Date}.log");
            }

            app.UseHangfireServer();

            RecurringJob.AddOrUpdate<IHangfireJobs>(
                jobs => jobs.CloseExpiredMatches(), Cron.Hourly);

            RecurringJob.AddOrUpdate<IHangfireJobs>(
                jobs => jobs.CloseExpiredEvaluations(), Cron.Hourly);

            app.UseAuthentication()
                .UseCors(_env.IsDevelopment() ? "AllowAll" : "AllowClient")
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
                    options.AddPolicy("AllowClient",
                        p => p.WithOrigins(_oAuth.Routing.Client)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials());
                });
            }
        }
    }
}