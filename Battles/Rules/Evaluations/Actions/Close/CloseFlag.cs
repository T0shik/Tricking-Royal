using System.Linq;
using Battles.Enums;
using Battles.Models;
using Battles.Rules.Matches.Extensions;

namespace Battles.Rules.Evaluations.Actions.Close
{
    public class CloseFlag : ICloseEvaluation
    {
        private readonly Evaluation _evaluation;

        public CloseFlag(Evaluation evaluation)
        {
            _evaluation = evaluation;
        }

        public void Close()
        {
            var calculator = ResultsCalculator.Calculate(_evaluation.Decisions);

            if (calculator.GetForgiveVotes() > calculator.GetPunishVotes())
            {
                // todo: forgive
                _evaluation.Match.Status = Status.Active;
                foreach (var user in _evaluation.Match.MatchUsers)
                {
                    user.SetFreeze(false);
                }
            }
            else if (calculator.GetForgiveVotes() <= calculator.GetPunishVotes())
            {
                //todo: punish
            }

            _evaluation.Complete = true;

            _evaluation.Decisions
                       //.Where(x => x.Vote == VoteResults.Winner())
                       .Select(x => x.User)
                       .ToList()
                       .ForEach(x => { x.Reputation += 2; });
        }
    }
}