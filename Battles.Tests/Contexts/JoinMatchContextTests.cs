using Battles.Enums;
using Battles.Interfaces;
using Battles.Models;
using Battles.Rules.Matches;
using Battles.Tests._Mocks;
using Moq;
using Xunit;
using Match = Battles.Models.Match;

namespace Battles.Tests.Contexts
{
    public class JoinMatchContextTests
    {
        private readonly Mock<IOpponentChecker> _opponentCheckerMock;
        private readonly MatchUser _host;
        private readonly UserInformation _opponent;
        private readonly JoinMatchContext _context;

        public JoinMatchContextTests()
        {
            _opponentCheckerMock = new Mock<IOpponentChecker>();
            _context = new JoinMatchContext(_opponentCheckerMock.Object, new StartMatchContextMock());
            _host = UserMockCreator.CreateMatchHost();
            _opponent = UserMockCreator.CreateOpponent();
        }

        private class StartMatchContextMock : StartMatchContext
        {
            public override StartMatchContext Setup(Match match)
            {
                return this;
            }

            public override bool Start()
            {
                return true;
            }
        }

        private Match CreateMatch() => new Match
        {
            Id = 1,
            Mode = Mode.OneUp,
            TurnType = TurnType.Classic,
            MatchUsers =
            {
                _host
            }
        };

        [Fact]
        public void UserCanJoinMatch_WithRoleOpponent()
        {
            var match = CreateMatch();

            _context.Setup(match).AddOpponent(_opponent);

            Assert.Contains(match.MatchUsers, x => x.UserId == _opponent.Id
                                                   && x.Role == MatchRole.Opponent);
        }

        [Fact]
        public void ThrowsCantJoinMatchException_WhenJoinLimitReached()
        {
            _opponent.Joined = 1;
            Assert.Throws<CantJoinMatchException>(() => _context.Setup(CreateMatch()).AddOpponent(_opponent));
        }

        [Fact]
        public void ThrowsCantJoinMatchException_WhenMatchNotOpenOrInvitation()
        {
            var match = CreateMatch();

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
                Assert.Throws<CantJoinMatchException>(() => _context.Setup(match).AddOpponent(_opponent));
            }
        }

        [Fact]
        public void ThrowsCantJoinMatchException_WhenTryingToJoinYourOwnMatch()
        {
            var match = CreateMatch();

            Assert.Throws<CantJoinMatchException>(() => _context.Setup(match).AddOpponent(_host.User));
        }

        [Fact]
        public void ThrowsCantJoinMatchException_WhenAlreadyBattlingOpponent()
        {
            var match = CreateMatch();
            _opponentCheckerMock
                .Setup(x => x.AreOpponents(_host.UserId, _opponent.Id))
                .Returns(true);

            Assert.Throws<CantJoinMatchException>(() => _context.Setup(match).AddOpponent(_opponent));
        }

        [Fact]
        public void UserJoinCountIncremented_WhenJoinedMatch()
        {
            Assert.True(_opponent.Joined == 0);
            var match = CreateMatch();

            _context.Setup(match).AddOpponent(_opponent);
            Assert.Equal(1, _opponent.Joined);
        }
    }
}