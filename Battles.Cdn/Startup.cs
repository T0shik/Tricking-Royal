using System.IO.Compression;
using Battles.Cdn.FileServices;
using Battles.Shared;
using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrickingRoyal.Database;

namespace Battles.Cdn
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _env;
        private readonly OAuth _oAuth;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _config = configuration;
            _env = env;
            _oAuth = _config.GetSection("OAuth").Get<OAuth>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config.GetSection("FilePaths").Get<FilePaths>());

            var connectionString = _config.GetConnectionString("DefaultConnection");
            services.AddTrickingRoyalDatabase(connectionString);

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddAuthentication("Bearer")
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = _oAuth.Routing.Server;
                        options.RequireHttpsMetadata = _env.IsProduction();

                        options.ApiName = _oAuth.Cdn.Name;
                        options.ApiSecret = _oAuth.Cdn.ResourceSecret.ToSha256();
                    });

            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("Weekly", new CacheProfile {Duration = 60 * 60 * 24 * 7});
                options.CacheProfiles.Add("Monthly", new CacheProfile {Duration = 60 * 60 * 24 * 7 * 4});
                options.CacheProfiles.Add("Quarterly", new CacheProfile {Duration = 60 * 60 * 24 * 7 * 4 * 3});
                options.CacheProfiles.Add("Monthly6", new CacheProfile {Duration = 60 * 60 * 24 * 7 * 4 * 6});
            });

            services.AddSingleton<FileStreams>()
                    .AddSingleton<ImageManager>()
                    .AddSingleton<VideoManager>();

            services.AddHealthChecks();
            SetupCors(services);
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseCors(_env.IsDevelopment() ? "AllowAll" : "AllowClients");
            app.UseHealthChecks("/healthcheck");

            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors("AllowAll");
            }
            else
            {
                loggerFactory.AddFile("Logs/server-{Date}.txt");
            }

            app.UseAuthentication();

            app.UseResponseCompression();

            app.UseMvc();
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
    }
}