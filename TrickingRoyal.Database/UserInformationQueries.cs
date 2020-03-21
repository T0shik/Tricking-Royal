using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TrickingRoyal.Database
{
    public static class UserInformationQueries
    {
        public static Task<string> UserLanguage(
            this AppDbContext @this,
            string userId,
            CancellationToken ct) =>
            @this.UserInformation
                 .Where(x => x.Id == userId)
                 .Select(x => x.Language)
                 .FirstAsync(ct);
    }
}