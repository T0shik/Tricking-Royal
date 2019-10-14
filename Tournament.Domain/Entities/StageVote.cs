using System.ComponentModel.DataAnnotations;

namespace Tournament.Domain.Models
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
