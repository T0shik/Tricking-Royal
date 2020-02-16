using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Battles.Application.Extensions;
using Battles.Extensions;
using Battles.Models;
using Newtonsoft.Json;

// ReSharper disable MemberCanBePrivate.Global
namespace Battles.Application.ViewModels.Matches
{
    public class MatchViewModel : BaseMatchViewModel<MatchUserViewModel>
    {
        public int Id { get; set; }
        public int Round { get; set; }
        public int TurnType { get; set; }
        public string Turn { get; set; }
        public string Finished { get; set; }
        public IEnumerable<MatchCommentsViewModel> Comments { get; set; }
        public bool Updating { get; set; }
        public bool CanGo { get; set; }
        public bool CanFlag { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanPass { get; set; }
        public bool CanLockIn { get; set; }
        public bool CanLike { get; set; }
        public bool CanClose { get; set; }
        public string TurnTime { get; set; }

        [JsonIgnore]
        public static readonly Expression<Func<Match, MatchViewModel>> ProjectionForAnon =
            match => new MatchViewModel
            {
                Id = match.Id,
                Key = $"{match.Id}-{match.LastUpdate.GetKeyTime()}",
                Participants =
                    match.MatchUsers.AsQueryable().OrderBy(x => x.Role).Select(MatchUserViewModel.Projection),

                Finished = match.Finished,
                TurnTime = $"{match.TurnDays} Days",
                TimeLeft = match.LastUpdate.Add(new TimeSpan(match.TurnDays, 0, 0, 0))
                                .Subtract(DateTime.Now).ConvertTimeSpan("Left"),

                Turn = match.Turn,
                Round = match.Round,
                Status = (int) match.Status,
                Mode = (int) match.Mode,
                Surface = (int) match.Surface,
                TurnType = (int) match.TurnType,
                Chain = match.Chain.DefaultSplit(),

                Videos = match.Videos.AsQueryable().Select(VideoViewModel.Projection)
            };
        
        public static MatchViewModel GetMatch(Match match, string userId)
        {
            return new MatchViewModel
            {
                Id = match.Id,
                Key = $"{match.Id}-{match.LastUpdate.GetKeyTime()}",
                Participants = match.MatchUsers.AsQueryable()
                                    .Select(MatchUserViewModel.Projection),

                Finished = match.Finished,
                TurnTime = $"{match.TurnDays} Days",
                TimeLeft = match.LastUpdate.Add(new TimeSpan(match.TurnDays, 0, 0, 0))
                                .Subtract(DateTime.Now).ConvertTimeSpan("Left"),

                Turn = match.Turn,
                Round = match.Round,
                Status = (int) match.Status,
                Mode = (int) match.Mode,
                Surface = (int) match.Surface,
                TurnType = (int) match.TurnType,
                Chain = match.Chain.DefaultSplit(),
                Updating = match.Updating,

                Videos = match.Videos.Select(VideoViewModel.ProjectionFunction),

                CanGo = !match.Updating && (match.MatchUsers.FirstOrDefault(x => x.UserId == userId && !x.Freeze)?.CanGo ?? false),
                CanFlag = !match.Updating && (match.MatchUsers.FirstOrDefault(x => x.UserId == userId && !x.Freeze)?.CanFlag ?? false),
                CanUpdate = !match.Updating && (match.MatchUsers.FirstOrDefault(x => x.UserId == userId && !x.Freeze)?.CanUpdate ?? false),
                CanPass = !match.Updating && (match.MatchUsers.FirstOrDefault(x => x.UserId == userId && !x.Freeze)?.CanPass ?? false),
                CanLockIn = !match.Updating && (match.MatchUsers.FirstOrDefault(x => x.UserId == userId && !x.Freeze)?.CanLockIn ?? false),

                Likes = match.Likes.Count,
                CanLike = match.Likes.All(x => x.UserId != userId),
            };
        }

        public static MatchViewModel GetMatchWithComments(Match match, string userId)
        {
            return new MatchViewModel
            {
                Id = match.Id,
                Key = $"{match.Id}-{match.LastUpdate.GetKeyTime()}",
                Participants = match.MatchUsers.AsQueryable().Select(MatchUserViewModel.Projection),

                Finished = match.Finished,
                TurnTime = $"{match.TurnDays} Days",
                TimeLeft = match.LastUpdate.Add(new TimeSpan(match.TurnDays, 0, 0, 0))
                                .Subtract(DateTime.Now).ConvertTimeSpan("Left"),

                Turn = match.Turn,
                Round = match.Round,
                Status = (int) match.Status,
                Mode = (int) match.Mode,
                Surface = (int) match.Surface,
                TurnType = (int) match.TurnType,
                Chain = match.Chain.DefaultSplit(),
                Updating = match.Updating,

                Videos = match.Videos.Select(VideoViewModel.ProjectionFunction),

                CanGo = !match.Updating && (match.MatchUsers.FirstOrDefault(x => x.UserId == userId && !x.Freeze)?.CanGo ?? false),
                CanFlag = !match.Updating && (match.MatchUsers.FirstOrDefault(x => x.UserId == userId && !x.Freeze)?.CanFlag ?? false),
                CanUpdate = !match.Updating && (match.MatchUsers.FirstOrDefault(x => x.UserId == userId && !x.Freeze)?.CanUpdate ?? false),
                CanPass = !match.Updating && (match.MatchUsers.FirstOrDefault(x => x.UserId == userId && !x.Freeze)?.CanPass ?? false),
                CanLockIn = !match.Updating && (match.MatchUsers.FirstOrDefault(x => x.UserId == userId && !x.Freeze)?.CanLockIn ?? false),

                Likes = match.Likes.Count,
                CanLike = match.Likes.All(x => x.UserId != userId),
                Comments = match.Comments.Select(x => new MatchCommentsViewModel
                {
                    MainComment = CommentViewModel.CommentProjection.Compile().Invoke(x),
                    SubComments = x.SubComments.Select(y => CommentViewModel.SubCommentProjection.Compile().Invoke(y))
                })
            };
        }
    }
}