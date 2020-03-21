using System;
using System.Linq;
using Battles.Application.Extensions;
using Battles.Extensions;
using Battles.Models;

// ReSharper disable MemberCanBePrivate.Global
namespace Battles.Application.ViewModels.Matches
{
    public class OpenMatchViewModel : BaseMatchViewModel<OpenMatchUserViewModel>
    {
        public int Id { get; set; }
        public int Round { get; set; }
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

                Status = (int) match.Status,
                Mode = (int) match.Mode,
                Surface = (int) match.Surface,
                TurnType = (int) match.TurnType,
                Chain = match.Chain.DefaultSplit(),
                CanClose = match.MatchUsers.First().UserId == userId
            };
        }
    }
}