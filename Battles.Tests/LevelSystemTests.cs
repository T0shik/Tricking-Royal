using Battles.Models;
using Battles.Rules.Levels;
using Xunit;

namespace Battles.Tests
{
    public class LevelSystemTests
    {
        private readonly UserInformation _user = new UserInformation();

        [Fact]
        public void User_StartsWithExpectedInformation()
        {
            Assert.Equal(1, _user.Level);
            Assert.Equal(0, _user.Experience);
            Assert.Equal(0, _user.LevelUpPoints);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(0, 2)]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        public void AwardsExperienceToUser(int startingExp, int awardedExp)
        {
            _user.Experience = startingExp;
            LevelSystem.AwardExp(_user, awardedExp);
            Assert.Equal(startingExp + awardedExp, _user.Experience);
            Assert.Equal(1, _user.Level);
        }

        [Theory]
        [InlineData(5, 1)]
        [InlineData(9, 2)]
        [InlineData(13, 3)]
        [InlineData(17, 4)]
        [InlineData(21, 5)]
        [InlineData(25, 6)]
        [InlineData(29, 7)]
        public void UserNeedsExp_ToLevelUpFromLevel(int exp, int level)
        {
            _user.Level = level;
            var expectedPoints = _user.LevelUpPoints + 1;
            LevelSystem.AwardExp(_user, exp);
            Assert.Equal(0, _user.Experience);
            Assert.Equal(level + 1, _user.Level);
            Assert.Equal(expectedPoints, _user.LevelUpPoints);
        }
        
        [Theory]
        [InlineData(5 + 9, 1, 2)]
        [InlineData(5 + 9 + 13, 1,  3)]
        [InlineData(5 + 9 + 13 + 17, 1,  4)]
        public void UserCanLevelUpMultipleTimesPerAward(int exp, int level, int levelIncrease)
        {
            _user.Level = level;
            var expectedPoints = _user.LevelUpPoints + levelIncrease;
            LevelSystem.AwardExp(_user, exp);
            Assert.Equal(0, _user.Experience);
            Assert.Equal(level + levelIncrease, _user.Level);
            Assert.Equal(expectedPoints, _user.LevelUpPoints);
        }
        
        
        [Theory]
        [InlineData(5, 1, 1)]
        [InlineData(5, 1, 2)]
        [InlineData(5, 1, 3)]
        public void UserHadExpLeftOverAfterLevelUp(int exp, int level, int lefOverExp)
        {
            _user.Level = level;
            var expectedPoints = _user.LevelUpPoints + 1;
            LevelSystem.AwardExp(_user, exp + lefOverExp);
            Assert.Equal(lefOverExp, _user.Experience);
            Assert.Equal(level + 1, _user.Level);
            Assert.Equal(expectedPoints, _user.LevelUpPoints);
        }
    }
}