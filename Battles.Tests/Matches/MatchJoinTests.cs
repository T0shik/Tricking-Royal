using Battles.Domain.Models;
using Battles.Enums;
using Battles.Interfaces;
using Battles.Models;
using Battles.Rules.Matches;
using Battles.Rules.Matches.Actions.Join;
using Battles.Rules.Matches.Extensions;
using Battles.Tests.Mocks;
using Moq;
using Xunit;

namespace Battles.Tests.Matches
{
    public class MatchJoinTests
    {
        private readonly Mock<IOpponentChecker> _opponentCheckerMock;
        private readonly UserInformation _host;
        private readonly UserInformation _opponent;

        public MatchJoinTests()
        {
            _opponentCheckerMock = OpponentCheckerMock.Create();
            _host = UserMockCreator.CreateHost();
            _opponent = UserMockCreator.CreateOpponent();
        }

        [Fact]
        public void UserCanJoinMatch_WithRoleOpponent()
        {
            var settings = MatchMockCreator.CreateSettings();
            settings.Host = _host;
            var match = settings.CreateMatch();
            var manager = new MatchDoorman(_opponentCheckerMock.Object);

            manager.AddOpponent(match, _opponent);
            Assert.Contains(match.MatchUsers, x => x.UserId == _opponent.Id
                                                   && x.Role == MatchRole.Opponent);
        }

        [Fact]
        public void ThrowsCantJoinMatchException_WhenJoinLimitReached()
        {
            var settings = MatchMockCreator.CreateSettings();
            settings.Host = _host;
            var match = settings.CreateMatch();
            var manager = new MatchDoorman(_opponentCheckerMock.Object);

            _opponent.Joined = 1;
            Assert.Throws<CantJoinMatchException>(() => { manager.AddOpponent(match, _opponent); });
        }

        [Fact]
        public void ThrowsCantJoinMatchException_WhenMatchNotOpenOrInvitation()
        {
            var settings = MatchMockCreator.CreateSettingsWithHost();
            var match = settings.CreateMatch();

            var failMatchStates = new[]
            {
                Status.Active,
                Status.Closed,
                Status.Complete,
                Status.Pending,
            };

            foreach (var status in failMatchStates)
            {
                match.Status = status;
                var doorman = new MatchDoorman(_opponentCheckerMock.Object);
                Assert.Throws<CantJoinMatchException>(() => doorman.AddOpponent(match, _opponent));
            }
        }

        [Fact]
        public void ThrowsCantJoinMatchException_WhenTryingToJoinYourOwnMatch()
        {
            var settings = MatchMockCreator.CreateSettings();
            settings.Host = _host;
            var match = settings.CreateMatch();
            var manager = new MatchDoorman(_opponentCheckerMock.Object);

            Assert.Throws<CantJoinMatchException>(() => { manager.AddOpponent(match, _host); });
        }

        [Fact]
        public void ThrowsCantJoinMatchException_WhenAlreadyBattlingOpponent()
        {
            var settings = MatchMockCreator.CreateSettings();
            settings.Host = _host;
            var match = settings.CreateMatch();
            var checker = OpponentCheckerMock.Create(_host.Id, _opponent.Id, true);
            var manager = new MatchDoorman(checker.Object);

            Assert.Throws<CantJoinMatchException>(() => { manager.AddOpponent(match, _opponent); });
        }

        [Fact]
        public void UserJoinCountIncremented_WhenJoinedMatch()
        {
            Assert.True(_opponent.Joined == 0);

            var settings = MatchMockCreator.CreateSettingsWithHost();
            var match = settings.CreateMatch();
            var manager = new MatchDoorman(_opponentCheckerMock.Object);

            manager.AddOpponent(match, _opponent);
            Assert.Equal(1, _opponent.Joined);
        }

        [Fact]
        public void InformationAsExpected_WhenModeIsOneUp()
        {
            var settings = MatchMockCreator.CreateSettings(Mode.OneUp);
            settings.Host = _host;
            var match = settings.CreateMatch();
            var manager = new MatchDoorman(_opponentCheckerMock.Object);
            manager.AddOpponent(match, _opponent);

            Assert.Equal(Status.Active, match.Status);
            Assert.True(match.GetHost().CanGo);
            Assert.False(match.GetOpponent().CanGo);
            Assert.Equal(_host.DisplayName, match.Turn);
        }

        [Fact]
        public void InformationAsExpected_WhenModeIsThreeRoundPass_AndTurnTypeIsClassic()
        {
            var settings = MatchMockCreator.CreateSettings(Mode.ThreeRoundPass);
            settings.Host = _host;
            settings.TurnType = TurnType.Classic;
            var match = settings.CreateMatch();
            var manager = new MatchDoorman(_opponentCheckerMock.Object);
            manager.AddOpponent(match, _opponent);

            Assert.Equal(Status.Active, match.Status);
            Assert.True(match.GetHost().CanGo);
            Assert.False(match.GetOpponent().CanGo);
            Assert.Equal(_host.DisplayName, match.Turn);
        }

        [Fact]
        public void InformationAsExpected_WhenModeIsThreeRoundPass_AndTurnTypeIsAlternating()
        {
            var settings = MatchMockCreator.CreateSettings(Mode.ThreeRoundPass);
            settings.Host = _host;
            settings.TurnType = TurnType.Alternating;
            var match = settings.CreateMatch();
            var manager = new MatchDoorman(_opponentCheckerMock.Object);
            manager.AddOpponent(match, _opponent);

            Assert.Equal(Status.Active, match.Status);
            Assert.True(match.GetHost().CanGo);
            Assert.False(match.GetOpponent().CanGo);
            Assert.Equal(_host.DisplayName, match.Turn);
        }

        [Fact]
        public void InformationAsExpected_WhenModeIsThreeRoundPass_AndTurnTypeIsBlitz()
        {
            var settings = MatchMockCreator.CreateSettingsWithHost(Mode.ThreeRoundPass);
            settings.TurnType = TurnType.Blitz;
            var match = settings.CreateMatch();
            var manager = new MatchDoorman(_opponentCheckerMock.Object);
            manager.AddOpponent(match, _opponent);

            Assert.Equal(Status.Active, match.Status);
            Assert.True(match.GetHost().CanGo);
            Assert.True(match.GetOpponent().CanGo);
            Assert.Equal("", match.Turn);
        }

        [Fact]
        public void InformationAsExpected_WhenModeIsCopyCat()
        {
            var settings = MatchMockCreator.CreateSettings(Mode.CopyCat);
            settings.Host = _host;
            var match = settings.CreateMatch();
            var manager = new MatchDoorman(_opponentCheckerMock.Object);
            manager.AddOpponent(match, _opponent);

            Assert.Equal(Status.Active, match.Status);
            Assert.True(match.GetHost().CanGo);
            Assert.False(match.GetOpponent().CanGo);
            Assert.Equal(_host.DisplayName, match.Turn);
        }
    }
}