using System;
using System.Linq;
using System.Threading.Tasks;
using Battles.Models;
using Microsoft.EntityFrameworkCore;

namespace TrickingRoyal.Database
{
    public static class MatchUserQueriesExtensions
    {
        public static Task<bool> CanGo(this AppDbContext @this, int matchId, string userId)
        {
            return @this.CanDo(matchId, userId, user => user.CanGo);
        }

        public static Task<bool> CanUpdate(this AppDbContext @this, int matchId, string userId)
        {
            return @this.CanDo(matchId, userId, user => user.CanUpdate);
        }

        private static Task<bool> CanDo(this AppDbContext @this, int matchId, string userId, Func<MatchUser, bool> fn)
        {
            return @this.MatchUser
                .AsNoTracking()
                .Where(x => x.MatchId == matchId && x.UserId == userId)
                .Select(x => fn(x))
                .FirstOrDefaultAsync();
        }
    }
}