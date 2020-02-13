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

        public JoinMatchCommandHandler(
            AppDbContext ctx,
            MatchDoorman matchDoorman,
            INotificationQueue notification)
        {
            _ctx = ctx;
            _matchDoorman = matchDoorman;
            _notification = notification;
        }

        public async Task<BaseResponse> Handle(JoinMatchCommand request, CancellationToken cancellationToken)
        {
            var match = _ctx.Matches
                            .Include(x => x.MatchUsers)
                            .ThenInclude(x => x.User)
                            .FirstOrDefault(x => x.Id == request.MatchId);

            if (match == null)
                return new BaseResponse("Match not found.", false);

            var currentUser = _ctx.UserInformation
                                  .FirstOrDefault(x => x.Id == request.UserId);

            if (currentUser == null)
            {
                return new BaseResponse("User record not found", false);
            }

            try
            {
                _matchDoorman.AddOpponent(match, currentUser);
            }
            catch (Exception e)
            {
                return new BaseResponse(e.Message, false);
            }

            await _ctx.SaveChangesAsync(cancellationToken);

            _notification.QueueNotification(
                                            $"{currentUser.DisplayName} joined your match.",
                                            new[] {match.Id.ToString()}.DefaultJoin(),
                                            NotificationMessageType.MatchActive,
                                            new[] {match.GetHost().UserId});

            return new BaseResponse("Match joined", true);
        }
    }
}