using TrickingRoyal.Database;
using Battles.Application.ViewModels;
using Battles.Domain.Models;
using Battles.Rules.Matches;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Services.Notifications;
using Battles.Enums;
using Battles.Extensions;
using Battles.Rules.Matches.Extensions;
using Transmogrify;
using static System.String;

namespace Battles.Application.Services.Comments.Commands
{
    public class CreateSubCommentCommand : IRequest<BaseResponse<CommentViewModel>>
    {
        public string UserId { get; set; }
        [Required] public int CommentId { get; set; }
        [Required] public string Message { get; set; }
        [Required] public string TaggedUser { get; set; }
    }

    public class CreateSubCommentCommandHandler : IRequestHandler<CreateSubCommentCommand, BaseResponse<CommentViewModel>>
    {
        private readonly AppDbContext _ctx;
        private readonly INotificationQueue _notification;
        private readonly ITranslator _translator;

        public CreateSubCommentCommandHandler(
            AppDbContext ctx,
            INotificationQueue notification,
            ITranslator translator)
        {
            _ctx = ctx;
            _notification = notification;
            _translator = translator;
        }

        public async Task<BaseResponse<CommentViewModel>> Handle(CreateSubCommentCommand command, CancellationToken cancellationToken)
        {
            if (IsNullOrEmpty(command.Message))
            {
                return BaseResponse.Fail<CommentViewModel>(await _translator.GetTranslation("Comment.NeedMessage"));
            }

            var comment = await _ctx.Comments
                                    .Include(x => x.Match)
                                    .ThenInclude(x => x.MatchUsers)
                                    .FirstAsync(x => x.Id == command.CommentId, cancellationToken: cancellationToken);

            if (comment == null)
            {
                return BaseResponse.Fail<CommentViewModel>(await _translator.GetTranslation("Comment.Main404"));
            }

            var user = await _ctx.UserInformation
                                 .FirstAsync(x => x.Id == command.UserId, cancellationToken: cancellationToken);

            if (user == null)
            {
                return BaseResponse.Fail<CommentViewModel>(await _translator.GetTranslation("User.404"));
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
                var taggedUser =
                    await _ctx.UserInformation.FirstAsync(x => x.DisplayName == command.TaggedUser, cancellationToken);

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

            
            return BaseResponse.Ok(await _translator.GetTranslation("Comment.Created"),
                                   CommentViewModel.CommentProjection.Compile().Invoke(comment));
        }
    }
}