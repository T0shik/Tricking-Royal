using System;
using System.Collections.Generic;
using Battles.Enums;

namespace Battles.Models
{
    public class Evaluation
    {
        public int Id { get; set; }

        public bool Complete { get; set; }

        public int MatchId { get; set; }
        public Match Match { get; set; }

        public List<Decision> Decisions { get; set; }

        public string Reason { get; set; }
        public EvaluationT EvaluationType { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expiry { get; set; }
    }
}
