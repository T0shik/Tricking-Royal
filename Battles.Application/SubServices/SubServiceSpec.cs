using Battles.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Battles.Application.SubServices
{
    public static class SubServiceSpec
    {
        public static IServiceCollection AddSubServices(this IServiceCollection @this)
        {
            @this.AddTransient<IOpponentChecker, OpponentChecker>();

            return @this;
        }
    }
}