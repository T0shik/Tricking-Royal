using System;
using Battles.Enums;
using Battles.Models;
using Battles.Rules.Evaluations.Actions.Close;
using Battles.Shared;

//using Battles.Rules.Matches.Actions.ReUpload;

namespace Battles.Rules.Evaluations
{
    public class EvaluationActionFactory
    {
        private readonly Routing _routing;

        public EvaluationActionFactory(Routing routing)
        {
            _routing = routing;
        }

        public static ICloseEvaluation CreateClose(Evaluation evaluation)
        {
            if (evaluation == null)
            {
                throw new Exception("Evaluation Not Found");
            }

            switch (evaluation.EvaluationType)
            {
                case EvaluationT.Complete:
                    return new CloseComplete(evaluation);
                case EvaluationT.Flag:
                    return new CloseFlag(evaluation);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}