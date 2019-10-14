using TrickingRoyal.Database;
using Battles.Application.ViewModels;
using Battles.Rules.Matches;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Interfaces;
using Battles.Enums;
using Battles.Extensions;
using Battles.Models;
using Battles.Rules.Matches.Extensions;
using static System.String;

namespace Battles.Application.Services.Comments.Commands
{
    public class CreateCommentCommand : IRequest<CommentViewModel>
    {
        public int MatchId { get; set; }
        public string UserId { get; set; }
        [Required] public string Message { get; set; }
    }

    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, CommentViewModel>
    {
        private readonly AppDbContext _ctx;
        private readonly INotificationQueue _notification;

        public CreateCommentCommandHandler(
            AppDbContext ctx,
            INotificationQueue notification)
        {
            _ctx = ctx;
            _notification = notification;
        }

        public async Task<CommentViewModel> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            if (IsNullOrEmpty(request.Message))
            {
                throw new Exception("Comment Needs a Message");
            }

            var match = await _ctx.Matches
                .AsNoTracking()
                .Include(x => x.MatchUsers)
                .FirstAsync(x => x.Id == request.MatchId, cancellationToken: cancellationToken);

            if (match == null)
            {
                throw new MatchException("Match not found.");
            }

            var user = _ctx.UserInformation
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == request.UserId);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            var comment = new Comment
            {
                MatchId = match.Id,
                Picture = user.Picture,
                DisplayName = user.DisplayName,
                Message = request.Message
            };

            _ctx.Comments.Add(comment);
            await _ctx.SaveChangesAsync(cancellationToken);

            _notification.QueueNotification(
                $"{user.DisplayName} commented on your battle.",
                new[] {match.Id.ToString(), comment.Id.ToString()}.DefaultJoin(),
                NotificationMessageType.Comment,
                match.GetOtherUserIds(user.Id)
            );

            return CommentViewModel.CommentProjection.Compile().Invoke(comment);
        }
    }
}