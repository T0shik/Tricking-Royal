using Battles.Models;
using Tournaments.Enumerations;

namespace Tournaments.Entities
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
