namespace Battles.Models
{
    public class Like
    {
        public int MatchId { get; set; }
        public Match Match { get; set; }

        public string UserId { get; set; }
    }
}
