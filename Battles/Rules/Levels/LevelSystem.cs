using Battles.Domain.Models;
using Battles.Models;

namespace Battles.Rules.Levels
{
    public static class LevelSystem
    {
        public static void AwardExp(UserInformation user, int experience)
        {
            user.Experience += experience;

            var expNeeded = ExpNeededForLevelUp(user.Level);

            while (user.Experience >= expNeeded)
            {
                user.Experience -= expNeeded;
                user.Level++;
                user.LevelUpPoints++;
                expNeeded = ExpNeededForLevelUp(user.Level);
            }
        }

        public static int ExpNeededForLevelUp(int level)
        {
            return c_baseLevelExp + c_midBaseLevelExp * (level - 1);
        }

        private const int c_baseLevelExp = 5;
        private const int c_midBaseLevelExp = 4;
    }
}