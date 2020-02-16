using System.Collections.Generic;
using System.Linq;
using Battles.Enums;
using Battles.Interfaces;
using Microsoft.EntityFrameworkCore;
using TrickingRoyal.Database;

namespace Battles.Application.SubServices
{
    public class OpponentChecker : IOpponentChecker
    {
        private readonly AppDbContext _ctx;
        private List<string> _opponents;

        public OpponentChecker(AppDbContext ctx)
        {
            _ctx = ctx;
            _opponents = new List<string>();
        }

        public void LoadOpponents(string userId)
        {
            var activeMatchUsers = _ctx.Matches
                .Include(x => x.MatchUsers)
                .Where(x => x.Status == Status.Active)
                .Where(x => x.MatchUsers.Any(y => y.UserId == userId && y.Role == MatchRole.Host))
                .Select(x => x.MatchUsers)
                .ToList();
            
            if (activeMatchUsers.Count == 0)
                return;

            _opponents = activeMatchUsers
                .Select(x => x.Select(y => y.UserId).Aggregate((c, n) => $"{c}{n}"))
                .ToList();
        }

        public bool AreOpponents(string host, string opponent)
        {
            return _opponents.Any(x => x.Contains(host) && x.Contains(opponent));
        }
    }
}