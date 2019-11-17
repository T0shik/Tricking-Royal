using System;
using System.Collections.Generic;
using System.Linq;
using Battles.Enums;
using Battles.Models;

namespace Battles.Rules.Evaluations
{
    public class ResultsCalculator
    {
        private readonly int _hostVotes;
        private readonly int _opponentVotes;
        private readonly int _hostPercent;
        private readonly int _opponentPercent;

        private ResultsCalculator(int hostVotes, int opponentVotes, int hostPercent, int opponentPercent)
        {
            _hostVotes = hostVotes;
            _opponentVotes = opponentVotes;
            _hostPercent = hostPercent;
            _opponentPercent = opponentPercent;
        }

        public static ResultsCalculator Calculate(List<Decision> decisions)
        {
            var hostVotes = decisions.Where(x => x.Vote == 0).ToList();
            var opponentVotes = decisions.Where(x => x.Vote == 1).ToList();
            var totalWeight = decisions.Select(x => x.Weight).Sum();

            var hostWeight = hostVotes.Select(x => x.Weight).Sum();
            var opponentWeight = opponentVotes.Select(x => x.Weight).Sum();

            return new ResultsCalculator(
                hostVotes.Count,
                opponentVotes.Count,
                GetPercent(hostWeight, totalWeight),
                GetPercent(opponentWeight, totalWeight)
            );
        }

        private static int GetPercent(int part, int total) =>
            Convert.ToInt32(Math.Round((part * 100) / (double) total, MidpointRounding.AwayFromZero));

        public int GetHostVotes()
        {
            return _hostVotes;
        }

        public int GetOpponentVotes()
        {
            return _opponentVotes;
        }

        public int GetHostPercent()
        {
            return _hostPercent;
        }

        public int GetOpponentPercent()
        {
            return _opponentPercent;
        }

        public Winner GetWinner()
        {
            return _hostPercent > _opponentPercent ? Winner.Host :
                _opponentPercent > _hostPercent ? Winner.Opponent : Winner.Draw;
        }
    }
}