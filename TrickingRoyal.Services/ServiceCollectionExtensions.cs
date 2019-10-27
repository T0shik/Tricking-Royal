using System;
using TrickingRoyal.Services.Email;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTrickingRoyalServices(
            this IServiceCollection services,
            Action<TrickingRoyalServicesOptions> optionsConfiguration)
        {
            var options = new TrickingRoyalServicesOptions();
            optionsConfiguration(options);

            services.AddSingleton(options.EmailSettings);
            services.AddSingleton<IEmailSender, EmailSender>();

            return services;
        }
    }
}