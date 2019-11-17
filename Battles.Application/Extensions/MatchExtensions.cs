using System.Linq;
using Battles.Models;
using static System.String;

namespace Battles.Application.Extensions
{
    public static class MatchExtensions
    {
        public static IQueryable<Match> OrderByDate(this IQueryable<Match> @this) =>
            @this.OrderByDescending(m => m.LastUpdate.Date)
                .ThenByDescending(m => m.LastUpdate.TimeOfDay);

        public static IQueryable<Match> FilterByUser(this IQueryable<Match> @this, string displayName) =>
            IsNullOrEmpty(displayName)
                ? @this
                : @this.Where(x => x.MatchUsers
                    .Select(y => y.User.DisplayName)
                    .Contains(displayName));
    }
}