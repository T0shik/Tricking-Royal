namespace Battles.Models
{
    public class Video
    {
        public int Id { get; set; }
        
        public int MatchId { get; set; }
        public Match Match { get; set; }
        
        public string UserId { get; set; }
        public UserInformation User { get; set; }
        
        public int VideoIndex { get; set; }
        public int UserIndex { get; set; }
        public string VideoPath { get; set; }
        public string ThumbPath { get; set; }
        public bool Empty { get; set; }
    }
}