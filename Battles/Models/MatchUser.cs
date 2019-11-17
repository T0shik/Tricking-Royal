using Battles.Enums;

namespace Battles.Models
{
    public class MatchUser
    {
        public int MatchId { get; set; }
        public Match Match { get; set; }
        public string UserId { get; set; }
        public UserInformation User { get; set; }
        public MatchRole Role { get; set; }
        public int Index { get; set; }
        public int Points { get; set; }
        public bool Winner { get; set; }
        public bool Ready { get; set; }
        public bool CanGo { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanFlag { get; set; }
        public bool CanPass { get; set; }
        public bool CanLockIn { get; set; }
    }
}
