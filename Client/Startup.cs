using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages.Internal;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VueCliMiddleware;

namespace Battles.Client
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _env;

        public Startup(
            IConfiguration config,
            IHostingEnvironment env)
        {
            _config = config;
            _env = env;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            SetupCors(services);
            
            services.AddHealthChecks();
            
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "vue-client/dist";
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(_env.IsDevelopment() ? "AllowAll" : "AllowClients");
            app.UseHealthChecks("/healthcheck");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                app.UseRewriter(new RewriteOptions()
                    .AddRedirectToWww());
            }

            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "vue-client";
                
                if (env.IsDevelopment())
                {
                    spa.UseVueCli(npmScript: "serve", port: 8081);
                }
            });
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
                                      p => p.WithOrigins(_config["HealthChecker"])
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowCredentials());
                });
            }
        }
    }
}
