using Battles.Application.Services.Matches;
using Microsoft.Extensions.DependencyInjection;

namespace Battles.Api.Workers.MatchUpdater
{
    public static class MatchUpdaterServices
    {
        public static IServiceCollection AddMatchUpdaterWorker(this IServiceCollection @this)
        {
            @this.AddTransient<UpdateMatchQueue>();
            @this.AddTransient<IWorker, MatchUpdater>();
            
            return @this;
        }
    }
}