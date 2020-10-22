namespace Battles.Models
{
    public class SubComment
    {
        public int Id { get; set; }
        public int CommentId { get; set; }
        public Comment Comment { get; set; }

        public string TaggedUser { get; set; }
        public string Message { get; set; }
        public string Picture { get; set; }
        public string DisplayName { get; set; }
    }
}