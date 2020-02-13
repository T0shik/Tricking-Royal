using System.Linq;
using Battles.Enums;
using Battles.Models;

namespace Battles.Rules.Matches.Extensions
{
    public static class OpportunityExtensions
    {
        public static bool CanGo(this Match @this, string userId) =>
            @this.MatchUsers.Any(x => x.UserId == userId && x.CanGo);

        public static bool CanUpdate(this Match @this, string userId) =>
            @this.MatchUsers.Any(x => x.UserId == userId && x.CanUpdate);

        public static bool CanFlag(this Match @this, string userId) =>
            @this.MatchUsers.Any(x => x.UserId == userId && x.CanFlag);

        public static bool CanPass(this Match @this, string userId) =>
            @this.MatchUsers.Any(x => x.UserId == userId && x.CanPass);

        public static bool UserCanLockIn(this Match @this, string userId) =>
            @this.MatchUsers.Any(x => x.UserId == userId && x.CanLockIn);

        public static bool CanClose(this Match @this, string userId) =>
            (@this.Status == Status.Open && @this.UserInRole(userId, MatchRole.Host))
            || (@this.Status == Status.Invite && @this.IsParticipating(userId));

        public static bool CanLike(this Match @this, string userId) =>
           !@this.Likes.Any(x => x.UserId == userId);

        //public static bool CanJoin(this Match @this, User user) =>
        //    !(@this.AreOpponents(user)
        //        && @this.UserInRole(user.Id, MatchRole.Host)
        //        && @this.NotInvited(user));
    }
}
