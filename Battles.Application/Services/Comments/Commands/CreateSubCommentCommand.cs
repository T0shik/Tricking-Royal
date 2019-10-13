using TrickingRoyal.Database;
using Battles.Application.ViewModels;
using Battles.Domain.Models;
using Battles.Rules.Matches;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class CreateSubCommentCommand : IRequest<CommentViewModel>
    {
        public string UserId { get; set; }
        [Required] public int CommentId { get; set; }
        [Required] public string Message { get; set; }
        [Required] public string TaggedUser { get; set; }
    }

    public class CreateSubCommentCommandHandler : IRequestHandler<CreateSubCommentCommand, CommentViewModel>
    {
        private readonly AppDbContext _ctx;
        private readonly INotificationQueue _notification;

        public CreateSubCommentCommandHandler(
            AppDbContext ctx,
            INotificationQueue notification)
        {
            _ctx = ctx;
            _notification = notification;
        }

        public async Task<CommentViewModel> Handle(CreateSubCommentCommand command, CancellationToken cancellationToken)
        {
            if (IsNullOrEmpty(command.Message))
            {
                throw new Exception("Comment needs a message.");
            }

            var comment = await _ctx.Comments
                .Include(x => x.Match)
                .ThenInclude(x => x.MatchUsers)
                .FirstAsync(x => x.Id == command.CommentId, cancellationToken: cancellationToken);

            if (comment == null)
            {
                throw new Exception("Main comment not found.");
            }

            var user = await _ctx.UserInformation
                .FirstAsync(x => x.Id == command.UserId, cancellationToken: cancellationToken);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            var subComment = new SubComment
            {
                Picture = user.Picture,
                DisplayName = user.DisplayName,
                Message = command.Message,
                TaggedUser = command.TaggedUser
            };

            comment.SubComments.Add(subComment);

            await _ctx.SaveChangesAsync(cancellationToken);

            if (!IsNullOrEmpty(command.TaggedUser))
            {
                var taggedUser = await _ctx.UserInformation
                    .FirstAsync(x => x.DisplayName == command.TaggedUser, cancellationToken: cancellationToken);

                _notification.QueueNotification(
                    $"{user.DisplayName} replied to your comment.",
                    new[] {comment.MatchId.ToString(), comment.Id.ToString(), subComment.Id.ToString()}.DefaultJoin(),
                    NotificationMessageType.SubComment,
                    new[] {taggedUser.Id});
            }

            _notification.QueueNotification(
                $"{user.DisplayName} commented on your battle.",
                new[] {comment.MatchId.ToString(), comment.Id.ToString(), subComment.Id.ToString()}.DefaultJoin(),
                NotificationMessageType.SubComment,
                comment.Match.GetOtherUserIds(user.Id));

            return CommentViewModel.SubCommentProjection.Compile().Invoke(subComment);
        }
    }
}