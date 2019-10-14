using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TrickingRoyal.Database
{
    public static class ServiceSpec
    {
        public static IServiceCollection AddTrickingRoyalDatabase(this IServiceCollection @this,
            string connectionString)
        {
            @this.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            return @this;
        }
    }
}