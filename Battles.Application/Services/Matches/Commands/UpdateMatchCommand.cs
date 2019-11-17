using System;
using TrickingRoyal.Database;
using Battles.Rules.Matches;
using Battles.Rules.Matches.Actions.Update;
using Battles.Rules.Matches.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Interfaces;
using Battles.Application.ViewModels;
using Battles.Enums;
using Battles.Extensions;
using Battles.Models;

namespace Battles.Application.Services.Matches.Commands
{
    public class UpdateMatchCommand : UpdateSettings, IRequest<BaseResponse>
    {
    }

    public class UpdateMatchHandler : IRequestHandler<UpdateMatchCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;
        private readonly MatchActionFactory _managerFactory;
        private readonly INotificationQueue _notifications;

        public UpdateMatchHandler(
            AppDbContext ctx,
            MatchActionFactory managerFactory,
            INotificationQueue notifications)
        {
            _ctx = ctx;
            _managerFactory = managerFactory;
            _notifications = notifications;
        }

        public async Task<BaseResponse> Handle(UpdateMatchCommand request, CancellationToken cancellationToken)
        {
            var match = _ctx.Matches
                            .Include(x => x.MatchUsers)
                            .ThenInclude(x => x.User)
                            .Include(x => x.Videos)
                            .FirstOrDefault(x => x.Id == request.MatchId);

            if (match == null)
            {
                return new BaseResponse("Match not found.", false);
            }

            if (!match.CanGo(request.UserId))
            {
                return new BaseResponse("Not allowed to go.", false);
            }

            try
            {
                _managerFactory
                    .CreateMatchManager(match)
                    .UpdateMatch(request);
            }
            catch (Exception e)
            {
                return new BaseResponse($"Failed to update match: {e.Message}", false);
            }

            if (match.Mode == Mode.ThreeRoundPass
                && (match.TurnType == TurnType.Classic || match.TurnType == TurnType.Alternating)
                && match.Round > 3)
            {
                match.Status = Status.Pending;
                match.GetHost().SetLockUser();
                match.GetOpponent().SetLockUser();

                _ctx.Evaluations.Add(new Evaluation()
                {
                    MatchId = match.Id,
                    Match = match,
                    Expiry = DateTime.Now.AddDays(3),
                });

                await _ctx.SaveChangesAsync(cancellationToken);

                _notifications.QueueNotification(
                                                 "Your match is now in the tribunal for voting.",
                                                 new[] {match.Id.ToString()}.DefaultJoin(),
                                                 NotificationMessageType.TribunalHistory,
                                                 match.GetUserIds());
            }
            else
            {
                await _ctx.SaveChangesAsync(cancellationToken);

                var currentUser = match.GetUser(request.UserId);

                _notifications.QueueNotification(
                                                 $"{currentUser.User.DisplayName} updated a match you are in.",
                                                 new[] {match.Id.ToString()}.DefaultJoin(),
                                                 NotificationMessageType.MatchActive,
                                                 match.GetOtherUserIds(request.UserId));
            }

            return new BaseResponse("Match updated.", true);
        }
    }
}