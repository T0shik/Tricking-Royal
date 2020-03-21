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
using Transmogrify;

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
        private readonly ITranslator _translator;

        public PassRoundCommandHandler(
            AppDbContext ctx,
            INotificationQueue notifications,
            ITranslator translator)
        {
            _ctx = ctx;
            _notifications = notifications;
            _translator = translator;
        }

        public async Task<BaseResponse> Handle(PassRoundCommand request, CancellationToken cancellationToken)
        {
            var match = _ctx.Matches
                            .Include(x => x.MatchUsers)
                            .ThenInclude(x => x.User)
                            .FirstOrDefault(x => x.Id == request.MatchId);

            if (match == null)
                return BaseResponse.Fail(await _translator.GetTranslation("Match", "NotFound"));

            if (!match.CanPass(request.UserId))
                return BaseResponse.Fail(await _translator.GetTranslation("Match", "CantPass"));

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
                var notificationMessage =
                    await _translator.GetTranslation("Notification", "PassedAndWon", user.User.DisplayName,
                                                     match.Round.ToString());

                _notifications.QueueNotification(notificationMessage,
                                                 new[] {match.Id.ToString()}.DefaultJoin(),
                                                 NotificationMessageType.MatchHistory,
                                                 match.GetOtherUserIds(user.UserId));
            }
            else
            {
                var notificationMessage =
                    await _translator.GetTranslation("Notification", "Passed", user.User.DisplayName,
                                                     match.Round.ToString());

                _notifications.QueueNotification(notificationMessage,
                                                 new[] {match.Id.ToString()}.DefaultJoin(),
                                                 NotificationMessageType.MatchActive,
                                                 match.GetOtherUserIds(user.UserId));
            }
            
            return BaseResponse.Ok(await _translator.GetTranslation("Match", "Passed"));
        }
    }
}