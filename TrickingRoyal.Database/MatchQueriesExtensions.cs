using System;
using System.Linq;
using System.Threading.Tasks;
using Battles.Models;
using Microsoft.EntityFrameworkCore;

namespace TrickingRoyal.Database
{
    public static class MatchQueriesExtensions
    {
        public static Task<bool> MatchIs(this AppDbContext @this, int matchId, Func<Match, bool> fn)
        {
            return @this.Matches
                .AsNoTracking()
                .Where(x => x.Id == matchId)
                .AnyAsync(x => fn(x));
        }
    }
}