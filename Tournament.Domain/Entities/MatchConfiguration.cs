using Battles.Enums;

namespace Tournaments.Entities
{
    public class MatchConfiguration
    {
        public int Id { get; set; }
        public Mode Mode { get; set; }
        public TurnType TurnType { get; set; }
        public Surface Surface { get; set; }
        public int Time { get; set; }
        public int Format { get; set; }

        public bool JudgeComments { get; set; }
        public bool UserComments { get; set; }

        public bool JudgeReview { get; set; }
    }
}
