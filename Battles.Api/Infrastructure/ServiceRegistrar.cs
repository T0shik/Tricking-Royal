using System.Linq;
using System.Reflection;
using Battles.Rules.Evaluations;
using Battles.Rules.Matches;
using Microsoft.Extensions.DependencyInjection;

namespace Battles.Api.Infrastructure
{
    public static class ServiceRegistrar
    {
        public static IServiceCollection AddBattlesServices(this IServiceCollection @this)
        {
            @this.AddTransient<MatchActionFactory>();

            var attributeType = typeof(ServiceAttribute);
            var assembly = attributeType.Assembly;
            var definedTypes = assembly.DefinedTypes;

            var services = definedTypes
                .Where(t => t.GetTypeInfo()
                                .GetCustomAttribute<ServiceAttribute>() != null);

            foreach (var service in services)
                @this.AddTransient(service);

            return @this;
        }
    }
}