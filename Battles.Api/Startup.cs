﻿using System.IO;
using System.Net.Http.Headers;
using Battles.Application.Services.Users.Queries;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Battles.Api.Infrastructure;
using Battles.Api.Workers;
using Battles.Api.Workers.MatchUpdater;
using Battles.Api.Workers.Notifications.Settings;
using Battles.Application.Configuration;
using Battles.Application.Services.Evaluations.Commands;
using Battles.Application.Services.Matches.Commands;
using Battles.Application.SubServices;
using Battles.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using Transmogrify.DependencyInjection.Json;
using TrickingRoyal.Database;

namespace Battles.Api
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly OAuth _oAuth;

        public Startup(IWebHostEnvironment env, IConfiguration config)
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
            services.AddSingleton(_config.GetSection("FilePaths").Get<FilePaths>());

            var connectionString = _config.GetConnectionString("DefaultConnection");
            services.AddTrickingRoyalDatabase(connectionString)
                    .AddHangfire(options => options.UseSqlServerStorage(connectionString));

            services.AddAuthentication("Bearer")
                    .AddJwtBearer("Bearer", config =>
                    {
                        config.Authority = _oAuth.Routing.Server;
                        config.RequireHttpsMetadata = _env.IsProduction();
                        config.Audience = _oAuth.Api.Name;
                        config.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];

                                var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken)
                                    && path.StartsWithSegments("/hub"))
                                {
                                    context.Token = accessToken;
                                }

                                return Task.CompletedTask;
                            }
                        };
                    });

            SetupCors(services);

            services.AddHttpContextAccessor();
            services.AddTransmogrify(config =>
            {
                config.DefaultLanguage = "en";
                config.LanguagePath = Path.Combine(_env.ContentRootPath, "Languages");
                config.AddResolver(typeof(DefaultLanguageResolver));
            });

            services.AddBattlesServices()
                    .AddSubServices()
                    .AddWorkers()
                    .AddHttpClient("default",
                                   config =>
                                   {
                                       config.DefaultRequestHeaders.Accept
                                             .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                   });

            services.AddMediatR(typeof(GetUserQuery).GetTypeInfo().Assembly)
                    .AddScoped(typeof(IPipelineBehavior<,>), typeof(AppendUserIdPipelineBehaviour<,>));

            var emailSettings = _config.GetSection(nameof(MailKitOptions)).Get<MailKitOptions>();
            services.AddMailKit(optionBuilder => { optionBuilder.UseMailKit(emailSettings); });

            services.AddHealthChecks();
            services.AddControllers();

            services.AddSingleton<IUserIdProvider, UserIdProvider>();
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseCors("AllowClients");

            if (_env.IsProduction())
            {
                loggerFactory.AddFile("Logs/api-{Date}.log");
            }

            app.UseHangfireServer();
            SetupHangfireJobs();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(builder =>
            {
                builder.MapHealthChecks("/healthcheck");
                builder.MapDefaultControllerRoute();
                builder.MapHub<MatchUpdaterHub>("/hub/match-updater");
            });
        }

        private void SetupCors(IServiceCollection services)
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

        private static void SetupHangfireJobs()
        {
            RecurringJob.AddOrUpdate<CloseExpiredMatchesCommandHandler>(handler => handler.Handle(new CloseExpiredMatchesCommand(), CancellationToken.None), Cron.Hourly);
            RecurringJob.AddOrUpdate<CloseEvaluationsCommandHandler>(handler => handler.Handle(new CloseEvaluationsCommand(), CancellationToken.None), Cron.Minutely);
            RecurringJob.AddOrUpdate<SendMatchRemindersCommandHandler>(handler => handler.Handle(new SendMatchRemindersCommand(), CancellationToken.None), Cron.Daily);
        }
    }
}