using TrickingRoyal.Database;
using Battles.Application.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Battles.Application.Services.Notifications;
using Battles.Enums;
using Battles.Extensions;
using Battles.Models;
using Battles.Rules.Matches.Extensions;
using Transmogrify;
using static System.String;

namespace Battles.Application.Services.Comments.Commands
{
    public class CreateSubCommentCommand : BaseRequest, IRequest<Response<CommentViewModel>>
    {
        [Required] public int CommentId { get; set; }
        [Required] public string Message { get; set; }
        [Required] public string TaggedUser { get; set; }
    }

    public class
        CreateSubCommentCommandHandler : IRequestHandler<CreateSubCommentCommand, Response<CommentViewModel>>
    {
        private readonly AppDbContext _ctx;
        private readonly INotificationQueue _notification;
        private readonly Library _library;

        public CreateSubCommentCommandHandler(
            AppDbContext ctx,
            INotificationQueue notification,
            Library library)
        {
            _ctx = ctx;
            _notification = notification;
            _library = library;
        }

        public async Task<Response<CommentViewModel>> Handle(
            CreateSubCommentCommand command,
            CancellationToken cancellationToken)
        {
            var translationContext = await _library.GetContext();

            if (IsNullOrEmpty(command.Message))
            {
                return Response.Fail<CommentViewModel>(translationContext.Read("Comment", "NeedMessage"));
            }

            var comment = await _ctx.Comments
                                    .Include(x => x.Match)
                                    .ThenInclude(x => x.MatchUsers)
                                    .FirstAsync(x => x.Id == command.CommentId, cancellationToken: cancellationToken);

            if (comment == null)
            {
                return Response.Fail<CommentViewModel>(translationContext.Read("Comment", "NotFound"));
            }

            var user = await _ctx.UserInformation
                                 .FirstAsync(x => x.Id == command.UserId, cancellationToken: cancellationToken);

            if (user == null)
            {
                return Response.Fail<CommentViewModel>(translationContext.Read("User", "NotFound"));
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
                                                translationContext.Read("Notification", "CommentReply",
                                                                        user.DisplayName),
                                                new[]
                                                {
                                                    comment.MatchId.ToString(), comment.Id.ToString(),
                                                    subComment.Id.ToString()
                                                }.DefaultJoin(),
                                                NotificationMessageType.SubComment,
                                                new[] {taggedUser.Id});
            }

            _notification.QueueNotification(
                                            translationContext.Read("Notification", "CommentCreated", user.DisplayName),
                                            new[]
                                            {
                                                comment.MatchId.ToString(), comment.Id.ToString(),
                                                subComment.Id.ToString()
                                            }.DefaultJoin(),
                                            NotificationMessageType.SubComment,
                                            comment.Match.GetOtherUserIds(user.Id));


            return Response.Ok(translationContext.Read("Comment", "Created"),
                                   CommentViewModel.CommentProjection.Compile().Invoke(comment));
        }
    }
}