using System.Linq;
using Battles.Models;
using Microsoft.EntityFrameworkCore;

namespace TrickingRoyal.Database.Queries
{
    public static class EvaluationQueries
    {
        public static Evaluation CanVote(
            this DbSet<Evaluation> @this,
            int evalId,
            string userId) =>
            @this.Include(e => e.Match)
                 .ThenInclude(x => x.MatchUsers)
                 .Include(e => e.Decisions)
                 .Where(x => !x.Match.MatchUsers.Select(y => y.UserId).Contains(userId))
                 .Where(x => !x.Decisions.Select(y => y.UserId).Contains(userId))
                 .FirstOrDefault(x => x.Id == evalId);
    }
}