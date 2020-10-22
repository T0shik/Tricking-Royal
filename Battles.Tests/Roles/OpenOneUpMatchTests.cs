using Battles.Enums;
using Battles.Models;
using Battles.Roles;
using Battles.Tests._Mocks;
using Xunit;

namespace Battles.Tests.Roles
{
    public class OpenOneUpMatchTests
    {
        private static Match InvalidMatch() => new Match
        {
            Mode = Mode.OneUp,
            MatchUsers =
            {
                UserMockCreator.CreateMatchHost(),
            }
        };
        
        private static Match ValidMatch() => new Match
        {
            Mode = Mode.OneUp,
            MatchUsers =
            {
                UserMockCreator.CreateMatchHost(),
                UserMockCreator.CreateMatchOpponent(),
            }
        };

        [Fact]
        public void CantStartWithNoOpponent()
        {
            var match = new OpenOneUpMatch(InvalidMatch());
            Assert.False(match.CanStart());
        }

        [Fact]
        public void CanStartWithOpponent()
        {
            var match = new OpenOneUpMatch(ValidMatch());
            Assert.False(match.CanStart());
        }

        [Fact]
        public void ActiveWhenStarted()
        {
            var match = new OpenOneUpMatch(ValidMatch());
            Assert.True(match.Start());
            Assert.Equal(Status.Active, match.Status);
        }
        
        [Fact]
        public void HostActionsSetWhenStarted()
        {
            var match = new OpenOneUpMatch(ValidMatch());
            Assert.True(match.Start());
            var host = match.GetHost();
            Assert.Equal(host.UserInfromation.DisplayName, match.Turn);
            Assert.True(host.CanGo);
            Assert.False(host.CanFlag);
            Assert.False(host.CanPass);
        }
        
        [Fact]
        public void OpponentActionsSetWhenStarted()
        {
            var match = new OpenOneUpMatch(ValidMatch());
            Assert.True(match.Start());
            var opponent = match.GetOpponent();
            Assert.False(opponent.CanGo);
            Assert.False(opponent.CanFlag);
            Assert.False(opponent.CanPass);
        }
    }
}