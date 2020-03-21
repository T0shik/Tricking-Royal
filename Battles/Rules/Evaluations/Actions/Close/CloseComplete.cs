using System;
using System.Linq;
using System.Threading.Tasks;
using Battles.Enums;
using Battles.Extensions;
using Battles.Models;
using Battles.Rules.Levels;
using Battles.Rules.Matches.Extensions;

namespace Battles.Rules.Evaluations.Actions.Close
{
    public class CloseComplete : ICloseEvaluation
    {
        private readonly Evaluation _evaluation;

        public CloseComplete(Evaluation evaluation)
        {
            _evaluation = evaluation;
        }
        
        public void Close()
        {
            var host = _evaluation.Match.GetHost();
            var opponent = _evaluation.Match.GetOpponent();

            var winner = ResultsCalculator.Calculate(_evaluation.Decisions).GetWinner();

            if (winner == Winner.Draw)
            {
                host.SetDraw(10).AwardExp(10);
                opponent.SetDraw(10).AwardExp(10);
            }
            else if (winner == Winner.Host)
            {
                host.SetWinnerAndLock(10).AwardExp(12);
                opponent.SetLoserAndLock(10).AwardExp(8);
            }
            else if (winner == Winner.Opponent)
            {
                host.SetLoserAndLock(10).AwardExp(8);
                opponent.SetWinnerAndLock(10).AwardExp(12);
            }

            host.User.Hosting--;
            opponent.User.Joined--;

            _evaluation.Complete = true;

            _evaluation.Match.Status = Status.Complete;
            _evaluation.Match.LastUpdate = DateTime.Now;
            _evaluation.Match.Finished = _evaluation.Match.LastUpdate.GetFinishTime();

            _evaluation.Decisions
                      .Select(x => x.User)
                      .ToList()
                      .ForEach(user =>
                      {
                          user.Reputation += 2;
                          user.AwardExp(2);
                      });
        }
    }
}