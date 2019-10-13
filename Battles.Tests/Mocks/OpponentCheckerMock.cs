using Battles.Interfaces;
using Moq;

namespace Battles.Tests.Mocks
{
    public static class OpponentCheckerMock
    {
        public static Mock<IOpponentChecker> Create()
        {
            return new Mock<IOpponentChecker>();
        }

        public static Mock<IOpponentChecker> Create(string host, string opponent, bool result)
        {
            var opponentCheckerMock = new Mock<IOpponentChecker>();

            opponentCheckerMock
                .Setup(x => x.AreOpponents(host, opponent))
                .Returns(result);

            return opponentCheckerMock;
        }
    }
}