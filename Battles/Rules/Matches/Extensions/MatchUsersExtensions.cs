using Battles.Models;

namespace Battles.Rules.Matches.Extensions
{
    public static class MatchUsersExtensions
    {
        public static MatchUser SetDraw(this MatchUser @this, int reputation)
        {
            @this.SetLockUser();
            @this.Winner = false;
            @this.User.Draw++;
            @this.User.Reputation += reputation;
            return @this;
        }

        public static MatchUser SetWinnerAndLock(this MatchUser @this, int reputation)
        {
            @this.SetLockUser();
            @this.Winner = true;
            @this.User.Win++;
            @this.User.Reputation += reputation;
            return @this;
        }

        public static MatchUser SetLoserAndLock(this MatchUser @this, int reputation)
        {
            @this.SetLockUser();
            @this.Winner = false;
            @this.User.Loss++;
            @this.User.Reputation += reputation;
            return @this;
        }

        public static MatchUser SetPenalty(
            this MatchUser @this,
            int penalty,
            bool close = false)
        {
            @this.User.Reputation -= penalty;
            if (close)
                @this.SetLockUser();

            return @this;
        }

        public static void SetLockUser(this MatchUser @this) => @this.SetGoFlagUpdatePassLock(false, false, false);

        public static void SetFreeze(this MatchUser @this, bool status) => @this.Freeze = status;

        public static MatchUser SetGoFlagUpdatePassLock(
            this MatchUser @this,
            bool go,
            bool flag,
            bool update,
            bool pass = false,
            bool lockIn = false,
            bool ready = false)
        {
            @this.CanGo = go;
            @this.CanPass = pass;
            @this.CanFlag = flag;
            @this.CanUpdate = update;
            @this.CanLockIn = lockIn;
            @this.Ready = ready;

            return @this;
        }
    }
}