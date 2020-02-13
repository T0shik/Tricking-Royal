using System;
using TrickingRoyal.Database;
using Battles.Rules.Matches.Actions.Join;
using Battles.Rules.Matches.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
    public class JoinMatchCommand : IRequest<BaseResponse>
    {
        public int MatchId { get; set; }
        public string UserId { get; set; }
    }

    public class JoinMatchCommandHandler : IRequestHandler<JoinMatchCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;
        private readonly MatchDoorman _matchDoorman;
        private readonly INotificationQueue _notification;
        private readonly ITranslator _translator;

        public JoinMatchCommandHandler(
            AppDbContext ctx,
            MatchDoorman matchDoorman,
            INotificationQueue notification,
            ITranslator translator)
        {
            _ctx = ctx;
            _matchDoorman = matchDoorman;
            _notification = notification;
            _translator = translator;
        }

        public async Task<BaseResponse> Handle(JoinMatchCommand request, CancellationToken cancellationToken)
        {
            var match = _ctx.Matches
                            .Include(x => x.MatchUsers)
                            .ThenInclude(x => x.User)
                            .FirstOrDefault(x => x.Id == request.MatchId);

            if (match == null)
                return BaseResponse.Fail(await _translator.GetTranslation("Match", "NotFound"));

            var currentUser = _ctx.UserInformation.FirstOrDefault(x => x.Id == request.UserId);

            if (currentUser == null)
                return BaseResponse.Fail(await _translator.GetTranslation("User", "NotFound"));

            try
            {
                _matchDoorman.AddOpponent(match, currentUser);
            }
            catch (Exception e)
            {
                return BaseResponse.Fail(e.Message);
            }

            await _ctx.SaveChangesAsync(cancellationToken);

            var notificationMessage =
                await _translator.GetTranslation("Notification", "JoinedMatch", currentUser.DisplayName);

            _notification.QueueNotification(notificationMessage,
                                            new[] {match.Id.ToString()}.DefaultJoin(),
                                            NotificationMessageType.MatchActive,
                                            new[] {match.GetHost().UserId});

            return BaseResponse.Ok(await _translator.GetTranslation("Match", "Joined"));
        }
    }
}