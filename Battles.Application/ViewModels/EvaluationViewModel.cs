using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Battles.Application.Extensions;
using Battles.Application.ViewModels.Matches;
using Battles.Enums;
using Battles.Extensions;
using Battles.Models;
// ReSharper disable MemberCanBePrivate.Global

namespace Battles.Application.ViewModels
{
    public class EvaluationViewModel
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public IEnumerable<MatchUserViewModel> Participants { get; set; }
        public IEnumerable<VideoViewModel> Videos { get; set; }

        public string Mode { get; set; }
        public string Surface { get; set; }
        public int Target { get; set; }
        public bool Flag { get; set; }
        public string Reason { get; set; }
        public string[] Chain { get; set; }

        public string TimeLeft { get; set; }

        public bool CanVote { get; set; }


        public static Expression<Func<Evaluation, EvaluationViewModel>> Projection =>
            eval => new EvaluationViewModel
            {
                Id = eval.Id,
                MatchId = eval.MatchId,
                Participants = eval.Match.MatchUsers
                    .AsQueryable()
                    .OrderBy(x => x.Role)
                    .Select(MatchUserViewModel.Projection),

                Mode = eval.Match.Mode.GetString(),
                Surface = eval.Match.Surface.GetString(),

                Flag = eval.EvaluationType == EvaluationT.Flag,
                Reason = eval.Reason,

                Chain = eval.Match.Chain.DefaultSplit(),
                TimeLeft = eval.Expiry.Subtract(DateTime.Now).ConvertTimeSpan("Left"),

                Videos = eval.Match.Videos.AsQueryable().Select(VideoViewModel.Projection),
            };
    }
}
