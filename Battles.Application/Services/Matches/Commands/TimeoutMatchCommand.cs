using TrickingRoyal.Database;
using Battles.Domain.Models;
using Battles.Rules.Matches.Extensions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Battles.Enums;
using Battles.Models;
using Battles.Rules.Levels;
using Microsoft.AspNetCore.Authorization;

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
                        host.SetLoser(-5);
                        opponent.SetWinner(5).AwardExp(5);
                    }
                    else if (!opponent.Ready)
                    {
                        opponent.SetLoser(-5);
                        host.SetWinner(5).AwardExp(5);
                    }
                }
                else
                {
                    host.SetDraw(-5);
                    opponent.SetDraw(0).AwardExp(2);
                }

                _ctx.Matches.Remove(match);
            }
            else
            {
                if (match.IsTurn(MatchRole.Host))
                {
                    host.SetLoser(-5);
                    opponent.SetWinner(5)
                        .AwardExp(2 + match.Round);
                }
                else if (match.IsTurn(MatchRole.Host))
                {
                    host.SetWinner(5)
                        .AwardExp(2 + match.Round);
                    opponent.SetLoser(-5);
                }

                host.SetGoFlagUpdatePassLock(false, false, false);
                opponent.SetGoFlagUpdatePassLock(false, false, false);
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