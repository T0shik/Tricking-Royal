using Battles.Application.Jobs;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HangfireJobsServiceRegister
    {
        public static IServiceCollection AddHangfireServices(this IServiceCollection @this)
        {
            @this.AddScoped<IHangfireJobs, HangfireJobs>();

            return @this;
        }
    }
}
