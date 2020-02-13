using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Battles.Application.Extensions;
using Battles.Extensions;
using Battles.Models;
using Newtonsoft.Json;

// ReSharper disable MemberCanBePrivate.Global
namespace Battles.Application.ViewModels.Matches
{
    public class OpenMatchViewModel : BaseMatchViewModel<OpenMatchUserViewModel>
    {
        public int Id { get; set; }
        public int Round { get; set; }
        public string Status { get; set; }
        public int TurnType { get; set; }
        public bool CanJoin { get; set; }
        public bool CanClose { get; set; }
        public string TurnTime { get; set; }
        public string Invitation { get; set; }

        public static OpenMatchViewModel GetOpenMatch(Match match, string userId)
        {
            return new OpenMatchViewModel
            {
                Id = match.Id,
                Key = $"{match.Id}-{match.LastUpdate.GetKeyTime()}",
                Participants = match.MatchUsers.Select(OpenMatchUserViewModel.Projection.Compile().Invoke),

                TurnTime = $"{match.TurnDays} Days",
                TimeLeft = match.LastUpdate.Add(new TimeSpan(match.TurnDays, 0, 0, 0)).
                                 Subtract(DateTime.Now).
                                 ConvertTimeSpan("Left"),

                Status = match.Status.GetString(),
                Mode = match.Mode.GetString(),
                Surface = (int) match.Surface,
                TurnType = (int) match.TurnType,
                Chain = match.Chain.DefaultSplit(),
                CanClose = match.MatchUsers.First().UserId == userId
            };
        }
    }
}