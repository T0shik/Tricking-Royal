using TrickingRoyal.Database;
using Battles.Rules.Matches.Extensions;
using Battles.Rules.Matches.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Services.Notifications;
using Battles.Application.ViewModels;
using Battles.Enums;
using Battles.Extensions;
using Battles.Models;

namespace Battles.Application.Services.Matches.Commands
{
    public class PassRoundCommand : IRequest<BaseResponse>
    {
        public int MatchId { get; set; }
        public string UserId { get; set; }
    }

    public class PassRoundCommandHandler : IRequestHandler<PassRoundCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;
        private readonly INotificationQueue _notifications;

        public PassRoundCommandHandler(
            AppDbContext ctx,
            INotificationQueue notifications)
        {
            _ctx = ctx;
            _notifications = notifications;
        }

        public async Task<BaseResponse> Handle(PassRoundCommand request, CancellationToken cancellationToken)
        {
            var match = _ctx.Matches
                            .Include(x => x.MatchUsers)
                            .ThenInclude(x => x.User)
                            .FirstOrDefault(x => x.Id == request.MatchId);

            if (match == null)
            {
                return new BaseResponse("Match not found.", false);
            }

            if (!match.CanPass(request.UserId))
            {
                return new BaseResponse("Not allowed to pass.", false);
            }

            var user = match.GetUser(request.UserId);

            match.Videos.Add(new Video
            {
                VideoIndex = match.Videos.Count,
                UserIndex = user.Index,
                UserId = user.UserId,
                Empty = true,
            });

            var host = match.GetHost();
            var opponent = match.GetOpponent();

            host.SetGoFlagUpdatePassLock(host.CanGo, false, false);
            opponent.SetGoFlagUpdatePassLock(opponent.CanGo, false, false);

            match.Round++;

            match.Turn = match.GetTurnName();
            var finished = CopyCatHelper.CompleteCopyCat(match);
            match.LastUpdate = DateTime.Now;

            await _ctx.SaveChangesAsync(cancellationToken);

            if (finished)
            {
                _notifications.QueueNotification(
                                                 $"{user.User.DisplayName} passed round {match.Round}, you won!",
                                                 new[] {match.Id.ToString()}.DefaultJoin(),
                                                 NotificationMessageType.MatchHistory,
                                                 match.GetOtherUserIds(user.UserId));
            }
            else
            {
                _notifications.QueueNotification(
                                                 $"{user.User.DisplayName} passed round {match.Round}.",
                                                 new[] {match.Id.ToString()}.DefaultJoin(),
                                                 NotificationMessageType.MatchActive,
                                                 match.GetOtherUserIds(user.UserId));
            }

            return new BaseResponse("Round passed.", true);
        }
    }
}