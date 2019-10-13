using Battles.Domain.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using Battles.Models;

namespace Battles.Application.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public bool HasReplies { get; set; }
        public string Message { get; set; }
        public string DisplayName { get; set; }
        public string Picture { get; set; }
        public string TaggedUser { get; set; }

        public static readonly Expression<Func<Comment, CommentViewModel>> CommentProjection =
            comment => new CommentViewModel()
            {
                MatchId = comment.MatchId,
                Id = comment.Id,
                HasReplies = comment.SubComments.AsQueryable().Any(),
                DisplayName = comment.DisplayName,
                Message = comment.Message,
                Picture = comment.Picture
            };

        public static readonly Expression<Func<SubComment, CommentViewModel>> SubCommentProjection =
            comment => new CommentViewModel()
            {
                Id = comment.Id,
                HasReplies = false,
                DisplayName = comment.DisplayName,
                Message = comment.Message,
                Picture = comment.Picture,
                TaggedUser = comment.TaggedUser
            };
    }
}