using System.Collections.Generic;
using Battles.Enums;
using Battles.Models;
using Battles.Rules.Evaluations;
using Xunit;

namespace Battles.Tests.EvaluationTests
{
    public class ResultsCalculatorTests
    {
        private readonly List<Decision> _decisions = new List<Decision>
        {
            new Decision {Vote = 0, Weight = 1},
            new Decision {Vote = 0, Weight = 1},
            new Decision {Vote = 1, Weight = 2},
            new Decision {Vote = 1, Weight = 2},
        };

        [Fact]
        public void CountsNumberOfVotesForHost()
        {
            var resultsCalculator = ResultsCalculator.Calculate(_decisions);
            Assert.Equal(2, resultsCalculator.GetHostVotes());
        }

        [Fact]
        public void CountsNumberOfVotesForOpponent()
        {
            var resultsCalculator = ResultsCalculator.Calculate(_decisions);
            Assert.Equal(2, resultsCalculator.GetOpponentVotes());
        }

        [Fact]
        public void CountsWinningPercentageForHost()
        {
            var resultsCalculator = ResultsCalculator.Calculate(_decisions);
            Assert.Equal(33, resultsCalculator.GetHostPercent());
        }

        [Fact]
        public void CountsWinningPercentageForOpponent()
        {
            var resultsCalculator = ResultsCalculator.Calculate(_decisions);
            Assert.Equal(67, resultsCalculator.GetOpponentPercent());
        }

        [Fact]
        public void RecognizesHostAsWinner()
        {
            var decisions = new List<Decision>
            {
                new Decision {Vote = 0, Weight = 2},
                new Decision {Vote = 1, Weight = 1},
            };

            var resultsCalculator = ResultsCalculator.Calculate(decisions);
            Assert.Equal(Winner.Host, resultsCalculator.GetWinner());
        }

        [Fact]
        public void RecognizesOpponentAsWinner()
        {
            var decisions = new List<Decision>
            {
                new Decision {Vote = 0, Weight = 1},
                new Decision {Vote = 1, Weight = 2},
            };

            var resultsCalculator = ResultsCalculator.Calculate(decisions);
            Assert.Equal(Winner.Opponent, resultsCalculator.GetWinner());
        }

        [Fact]
        public void RecognizesDraw()
        {
            var decisions = new List<Decision>
            {
                new Decision {Vote = 0, Weight = 1},
                new Decision {Vote = 1, Weight = 1},
            };

            var resultsCalculator = ResultsCalculator.Calculate(decisions);
            Assert.Equal(Winner.Draw, resultsCalculator.GetWinner());
        }
    }
}