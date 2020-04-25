using System.ComponentModel.DataAnnotations;

namespace Tournaments.Entities
{
    public class StageVote
    {
        [Key]
        public int Id { get; set; }

        public int Round { get; set; }

        public bool Host { get; set; }
        public bool Draw { get; set; }
        public bool Opponent { get; set; }

        public string Comment { get; set; }
    }
}
