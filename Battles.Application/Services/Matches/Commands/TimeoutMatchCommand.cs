using TrickingRoyal.Database;
using Battles.Rules.Matches.Extensions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Battles.Enums;
using Battles.Models;
using Battles.Rules.Levels;

namespace Battles.Application.Services.Matches.Commands
{
    public class TimeoutMatchCommand : IRequest<Unit>
    {
        public TimeoutMatchCommand(Match match)
        {
            Match = match;
        }

        public Match Match { get; }
    }

    public class TimeoutMatchCommandHandler : IRequestHandler<TimeoutMatchCommand, Unit>
    {
        private readonly AppDbContext _ctx;

        public TimeoutMatchCommandHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Unit> Handle(TimeoutMatchCommand request, CancellationToken cancellationToken)
        {
            var match = request.Match;

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

            await _ctx.SaveChangesAsync(cancellationToken);

            return new Unit();
        }
    }
}