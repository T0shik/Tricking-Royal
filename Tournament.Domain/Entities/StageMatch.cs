using System.Collections.Generic;

namespace Tournament.Domain.Models
{
    public class StageMatch
    {
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }

        public int TournamentMatchId { get; set; }
        public TournamentMatch TournamentMatch { get; set; }

        public ICollection<StageDecision> Decisions { get; set; }

        public int Stage { get; set; }
    }
}
