using System.IO.Compression;
using Battles.Cdn.FileServices;
using Battles.Cdn.Settings;
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

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _config = configuration;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var oAuth = _config.GetSection("OAuth").Get<OAuth>();

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
                    options.Authority = oAuth.Routing.Server;
                    options.RequireHttpsMetadata = _env.IsProduction();

                    options.ApiName = oAuth.Cdn.Name;
                    options.ApiSecret = oAuth.Cdn.ResourceSecret.ToSha256();
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
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
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
    }
}