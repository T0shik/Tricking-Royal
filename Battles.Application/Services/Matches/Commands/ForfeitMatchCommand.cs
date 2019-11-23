using TrickingRoyal.Database;
using Battles.Rules.Matches.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Extensions;
using Battles.Application.Services.Notifications;
using Battles.Application.ViewModels;
using Battles.Enums;
using Battles.Extensions;
using Battles.Rules.Levels;
using static System.String;

namespace Battles.Application.Services.Matches.Commands
{
    public class ForfeitMatchCommand : IRequest<BaseResponse>
    {
        public int MatchId { get; set; }
        public string UserId { get; set; }
    }

    public class ForfeitMatchCommandHandler : IRequestHandler<ForfeitMatchCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;
        private readonly INotificationQueue _notification;

        public ForfeitMatchCommandHandler(
            AppDbContext ctx,
            INotificationQueue notification)
        {
            _ctx = ctx;
            _notification = notification;
        }

        //todo move to core level
        public async Task<BaseResponse> Handle(ForfeitMatchCommand request, CancellationToken cancellationToken)
        {
            var match = await _ctx.Matches
                                  .Include(x => x.MatchUsers)
                                  .ThenInclude(x => x.User)
                                  .FirstOrDefaultAsync(x => x.Id == request.MatchId, cancellationToken: cancellationToken);

            if (!match.CanGo(request.UserId))
            {
                return new BaseResponse("Not allowed to forfeit match.", false);
            }

            var host = match.GetHost();
            var opponent = match.GetOpponent();
            var notificationExtension = "";

            if (match.Round == 1 || (match.Mode == Mode.ThreeRoundPass && match.TurnType == TurnType.Blitz))
            {
                _ctx.Matches.Remove(match);
                notificationExtension = "Match deleted.";
            }
            else
            {
                if (match.IsTurn(MatchRole.Host))
                {
                    host.SetLoser(match.Round + 5)
                        .AwardExp(4 + match.Round);

                    opponent.SetWinner(match.Round + 5)
                            .AwardExp(7 + match.Round);
                }
                else if (match.IsTurn(MatchRole.Opponent))
                {
                    host.SetWinner(match.Round + 5)
                        .AwardExp(7 + match.Round);
                    opponent.SetLoser(match.Round + 5)
                            .AwardExp(4 + match.Round);
                }

                match.Status = Status.Complete;
                match.LastUpdate = DateTime.Now;
                match.Finished = DateTime.Now.GetFinishTime();
            }

            host.User.Hosting--;
            opponent.User.Joined--;

            await _ctx.SaveChangesAsync(cancellationToken);

            var notificationType = IsNullOrEmpty(notificationExtension)
                                       ? NotificationMessageType.MatchHistory
                                       : NotificationMessageType.Empty;

            var user = match.GetUser(request.UserId);
            _notification.QueueNotification(
                                            $"{user.User.DisplayName} forfeited the match. {notificationExtension}",
                                            new[] {match.Id.ToString()}.DefaultJoin(),
                                            notificationType,
                                            match.GetOtherUserIds(request.UserId));

            return new BaseResponse("Match forfeited.", true);
        }
    }
}