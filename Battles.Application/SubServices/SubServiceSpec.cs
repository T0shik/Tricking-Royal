using Battles.Application.SubServices.VideoConversion;
using Battles.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Battles.Application.SubServices
{
    public static class SubServiceSpec
    {
        public static IServiceCollection AddSubServices(this IServiceCollection @this)
        {
            @this.AddSingleton<VideoConverter>();
            @this.AddTransient<IOpponentChecker, OpponentChecker>();

            return @this;
        }
    }
}