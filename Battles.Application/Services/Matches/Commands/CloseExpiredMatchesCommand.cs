using TrickingRoyal.Database;
using Battles.Rules.Matches.Extensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Enums;
using Battles.Models;
using Battles.Rules.Levels;
using Microsoft.EntityFrameworkCore;

namespace Battles.Application.Services.Matches.Commands
{
    public class CloseExpiredMatchesCommand : IRequest<Unit> {    }

    public class CloseExpiredMatchesCommandHandler : IRequestHandler<CloseExpiredMatchesCommand, Unit>
    {
        private readonly AppDbContext _ctx;

        public CloseExpiredMatchesCommandHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Unit> Handle(CloseExpiredMatchesCommand request, CancellationToken cancellationToken)
        {
            foreach (var match in GetExpiredMatches())
                TimeoutMatch(match);

            await _ctx.SaveChangesAsync(cancellationToken);

            return new Unit();
        }

        private void TimeoutMatch(Match match)
        {
            var host = match.GetHost();
            var opponent = match.GetOpponent();

            if (match.Round == 1)
            {
                if (match.Mode == Mode.ThreeRoundPass && match.TurnType == TurnType.Blitz)
                {
                    if (!host.Ready && !opponent.Ready)
                    {
                        host.SetDraw(-5);
                        opponent.SetDraw(-5);
                    }
                    else if (!host.Ready)
                    {
                        host.SetLoserAndLock(-5);
                        opponent.SetWinnerAndLock(5).AwardExp(5);
                    }
                    else if (!opponent.Ready)
                    {
                        opponent.SetLoserAndLock(-5);
                        host.SetWinnerAndLock(5).AwardExp(5);
                    }
                }
                else
                {
                    host.SetDraw(-5);
                    opponent.SetDraw(2).AwardExp(2);
                }

                _ctx.Matches.Remove(match);
            }
            else
            {
                if (match.IsTurn(MatchRole.Host))
                {
                    host.SetLoserAndLock(-5);
                    opponent.SetWinnerAndLock(5)
                            .AwardExp(2 + match.Round);
                }
                else if (match.IsTurn(MatchRole.Opponent))
                {
                    host.SetWinnerAndLock(5)
                        .AwardExp(2 + match.Round);
                    opponent.SetLoserAndLock(-5);
                }

                match.Status = Status.Complete;
                match.LastUpdate = DateTime.Now;
                match.Finished = DateTime.Now.ToString("dddd, dd MMMM yyyy, HH:mm");
            }

            host.User.Hosting--;
            opponent.User.Joined--;
        }
        
        private IEnumerable<Match> GetExpiredMatches() =>
            _ctx.Matches
                .Include(x => x.MatchUsers)
                .ThenInclude(x => x.User)
                .Where(x => x.Status == Status.Active)
                .Where(x => EF.Functions.DateDiffHour(x.LastUpdate, DateTime.Now) >= x.TurnDays * 24)
                .ToList();
    }
}