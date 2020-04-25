using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tournaments.Entities
{
    public class Tournament
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Prize { get; set; }

        public ICollection<Participant> Participants { get; set; }
        public ICollection<StageMatch> StageMatches { get; set; }
        public MatchConfiguration Configuration { get; set; }
    }
}
