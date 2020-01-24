using TrickingRoyal.Database;
using Battles.Application.ViewModels;
using Battles.Rules.Matches;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
    public class CreateCommentCommand : IRequest<BaseResponse<CommentViewModel>>
    {
        public int MatchId { get; set; }
        public string UserId { get; set; }
        [Required] public string Message { get; set; }
    }

    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, BaseResponse<CommentViewModel>>
    {
        private readonly AppDbContext _ctx;
        private readonly INotificationQueue _notification;
        private readonly ITranslator _translator;

        public CreateCommentCommandHandler(
            AppDbContext ctx,
            INotificationQueue notification,
            ITranslator translator)
        {
            _ctx = ctx;
            _notification = notification;
            _translator = translator;
        }

        public async Task<BaseResponse<CommentViewModel>> Handle(
            CreateCommentCommand request,
            CancellationToken cancellationToken)
        {
            if (IsNullOrEmpty(request.Message))
            {
                return BaseResponse.Fail<CommentViewModel>(await _translator.GetTranslation("Comment.NeedMessage"));
            }

            var match = await _ctx.Matches.AsNoTracking()
                                  .Include(x => x.MatchUsers)
                                  .FirstAsync(x => x.Id == request.MatchId, cancellationToken);

            if (match == null)
            {
                return BaseResponse.Fail<CommentViewModel>(await _translator.GetTranslation("Match.404"));
            }

            var user = _ctx.UserInformation.AsNoTracking().FirstOrDefault(x => x.Id == request.UserId);

            if (user == null)
            {
                return BaseResponse.Fail<CommentViewModel>(await _translator.GetTranslation("User.404"));
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

            _notification.QueueNotification($"{user.DisplayName} commented on your battle.",
                                            new[] {match.Id.ToString(), comment.Id.ToString()}.DefaultJoin(),
                                            NotificationMessageType.Comment,
                                            match.GetOtherUserIds(user.Id));

            return BaseResponse.Ok(await _translator.GetTranslation("Comment.Created"),
                                   CommentViewModel.CommentProjection.Compile().Invoke(comment));
        }
    }
}