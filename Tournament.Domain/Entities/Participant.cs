using Battles.Models;
using Tournament.Domain.Enums;

namespace Tournament.Domain.Models
{
    public class Participant
    {
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }

        public int UserId { get; set; }
        public UserInformation User { get; set; }

        public TournamentRole Role { get; set; }
    }
}
