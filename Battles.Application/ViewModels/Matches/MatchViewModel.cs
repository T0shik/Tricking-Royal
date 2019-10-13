using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Battles.Application.Extensions;
using Battles.Application.ViewModels.Matches;
using Battles.Extensions;
using Battles.Models;
using Newtonsoft.Json;

// ReSharper disable MemberCanBePrivate.Global

namespace Battles.Application.ViewModels.Matches
{
    public class MatchViewModel
    {
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
                    
                Turn =  match.Turn,
                Round = match.Round,
                Status = match.Status.GetString(),
                Mode = match.Mode.GetString(),
                Surface = match.Surface.GetString(),
                TurnType = (int) match.TurnType,
                Chain = match.Chain.DefaultSplit(),

                Videos = match.Videos.AsQueryable().Select(VideoViewModel.Projection)
            };

        public MatchViewModel()
        {
            Participants = new List<MatchUserViewModel>();
        }

        public int Id { get; set; }
        public string Key { get; set; }

        public IEnumerable<MatchUserViewModel> Participants { get; set; }
        public IEnumerable<VideoViewModel> Videos { get; set; }

        public string Mode { get; set; }
        public string Surface { get; set; }
        public int Round { get; set; }
        public string Status { get; set; }
        public string[] Chain { get; set; }
        public int Likes { get; set; }
        public int TurnType { get; set; }
        public string Turn { get; set; }
        public string TimeLeft { get; set; }
        public string Finished { get; set; }
        public IEnumerable<MatchCommentsViewModel> Comments { get; set; }

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

                Turn =  match.Turn,
                Round = match.Round,
                Status = match.Status.GetString(),
                Mode = match.Mode.GetString(),
                Surface = match.Surface.GetString(),
                TurnType = (int) match.TurnType,
                Chain = match.Chain.DefaultSplit(),

                Videos = match.Videos.Select(VideoViewModel.ProjectionFunction),

                CanGo = match.MatchUsers.FirstOrDefault(x => x.UserId == userId)?.CanGo ?? false,
                CanFlag = match.MatchUsers.FirstOrDefault(x => x.UserId == userId)?.CanFlag ?? false,
                CanUpdate = match.MatchUsers.FirstOrDefault(x => x.UserId == userId)?.CanUpdate ?? false,
                CanPass = match.MatchUsers.FirstOrDefault(x => x.UserId == userId)?.CanPass ?? false,
                CanLockIn = match.MatchUsers.FirstOrDefault(x => x.UserId == userId)?.CanLockIn ?? false,

                Likes = match.Likes.Count(),
                CanLike = match.Likes.All(x => x.UserId != userId)
            };
        }

        public static MatchViewModel GetOpenMatch(Match match, string userId)
        {
            return new MatchViewModel
            {
                Id = match.Id,
                Key = $"{match.Id}-{match.LastUpdate.GetKeyTime()}",
                Participants = match.MatchUsers.Select(MatchUserViewModel.Projection.Compile().Invoke),

                Finished = match.Finished,
                TurnTime = $"{match.TurnDays} Days",
                TimeLeft = match.LastUpdate.Add(new TimeSpan(match.TurnDays, 0, 0, 0))
                    .Subtract(DateTime.Now).ConvertTimeSpan("Left"),

                Status = match.Status.GetString(),
                Mode = match.Mode.GetString(),
                Surface = match.Surface.GetString(),
                TurnType = (int) match.TurnType,
                Chain = match.Chain.DefaultSplit(),

                CanClose = match.MatchUsers.First().UserId == userId
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

                Turn =  match.Turn,
                Round = match.Round,
                Status = match.Status.GetString(),
                Mode = match.Mode.GetString(),
                Surface = match.Surface.GetString(),
                TurnType = (int) match.TurnType,
                Chain = match.Chain.DefaultSplit(),

                Videos = match.Videos.Select(VideoViewModel.ProjectionFunction),

                CanGo = match.MatchUsers.FirstOrDefault(x => x.UserId == userId)?.CanGo ?? false,
                CanFlag = match.MatchUsers.FirstOrDefault(x => x.UserId == userId)?.CanFlag ?? false,
                CanUpdate = match.MatchUsers.FirstOrDefault(x => x.UserId == userId)?.CanUpdate ?? false,
                CanPass = match.MatchUsers.FirstOrDefault(x => x.UserId == userId)?.CanPass ?? false,
                CanLockIn = match.MatchUsers.FirstOrDefault(x => x.UserId == userId)?.CanLockIn ?? false,

                Likes = match.Likes.Count(),
                CanLike = match.Likes.All(x => x.UserId != userId),
                Comments = match.Comments.Select(x => new MatchCommentsViewModel
                {
                    MainComment = CommentViewModel.CommentProjection.Compile().Invoke(x),
                    SubComments = x.SubComments
                        .Select(y => CommentViewModel.SubCommentProjection.Compile().Invoke(y))
                })
            };
        }

        #region User Perspective Properties

        public bool CanGo { get; set; }
        public bool CanFlag { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanPass { get; set; }
        public bool CanLockIn { get; set; }
        public bool CanLike { get; set; }
        public bool CanJoin { get; set; }
        public bool CanClose { get; set; }

        #endregion

        #region Open Match Configuration

        public string TurnTime { get; set; }
        public string Invitation { get; set; }

        #endregion
    }
}