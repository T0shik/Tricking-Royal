using Battles.Domain.Models;
using Battles.Models;

namespace Battles.Rules.Levels
{
    public static class UserInformationExtensions
    {
        public static UserInformation AwardExp(this UserInformation @this, int exp)
        {
            LevelSystem.AwardExp(@this, exp);
            return @this;
        }

        public static MatchUser AwardExp(this MatchUser @this, int exp)
        {
            LevelSystem.AwardExp(@this.User, exp);
            return @this;
        }
    }
}