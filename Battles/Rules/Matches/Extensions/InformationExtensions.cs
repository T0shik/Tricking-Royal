using System.Collections.Generic;
using System.Linq;
using Battles.Enums;
using Battles.Models;

namespace Battles.Rules.Matches.Extensions
{
    public static class InformationExtensions
    {
        public static bool IsTurn(this Match @this, MatchRole role) =>
            @this.MatchUsers.FirstOrDefault(x => x.Role == role).CanGo;

        public static MatchUser GetUser(this Match @this, string userId) =>
            @this.MatchUsers.FirstOrDefault(x => x.UserId == userId);

        public static MatchUser GetHost(this Match @this) =>
            @this.MatchUsers.FirstOrDefault(x => x.Role == MatchRole.Host);

        public static MatchUser GetOpponent(this Match @this) =>
            @this.MatchUsers.FirstOrDefault(x => x.Role == MatchRole.Opponent);

        public static IEnumerable<string> GetUserIds(this Match @this) =>
            @this.MatchUsers.Select(x => x.UserId);
        
        public static IEnumerable<string> GetOtherUserIds(this Match @this, string userId) =>
            @this.GetOtherUsers(userId).Select(x => x.UserId);

        public static IEnumerable<MatchUser> GetOtherUsers(this Match @this, string userId) =>
            @this.MatchUsers.Where(x => x.UserId != userId);

        public static MatchUser GetTurnUser(this Match @this) =>
            @this.MatchUsers.FirstOrDefault(x => x.CanGo);

        public static string GetTurnName(this Match @this) =>
            @this.GetTurnUser().User.DisplayName;

        public static string GetWinName(this Match @this) =>
            //@this.MatchUsers
            //    .FirstOrDefault(x => x.Winner)
            //    .User.DisplayName;
            "";

        public static bool UserInRole(this Match @this, string userId, MatchRole role) =>
            @this.MatchUsers.Any(x => x.UserId == userId && x.Role == role);

        public static bool InRole(this MatchUser @this, MatchRole role) =>
            @this.Role == role;

        public static bool IsParticipating(this Match @this, string userId) =>
            @this.MatchUsers.Any(x => x.UserId == userId);

        public static bool NotInvited(this Match @this, UserInformation user) =>
            @this.Status == Status.Invite && !@this.UserInRole(user.Id, MatchRole.Opponent);
    }
}