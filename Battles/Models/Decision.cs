namespace Battles.Models
{
    public class Decision
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public UserInformation User { get; set; }

        public int EvaluationId { get; set; }
        public Evaluation Evaluation { get; set; }

        public int Vote { get; set; }
        public int Weight { get; set; }
    }
}
