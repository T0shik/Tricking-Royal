using System.Linq;
using Battles.Domain.Models;
using Battles.Models;
using Microsoft.EntityFrameworkCore;

namespace Battles.Application.Extensions
{
    public static class DbSetExtensions
    {
        public static Evaluation CanVote(this DbSet<Evaluation> @this, int EvalId, string UserId) =>
            @this
                .Include(e => e.Match)
                    .ThenInclude(x => x.MatchUsers)
                .Include(e => e.Decisions)
                .Where(x => !x.Match.MatchUsers.Select(y => y.UserId).Contains(UserId))
                .Where(x => !x.Decisions.Select(y => y.UserId).Contains(UserId))
                .FirstOrDefault(x => x.Id == EvalId);
    }
}
