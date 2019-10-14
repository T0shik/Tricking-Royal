namespace Battles.Configuration
{
    public class ReputationAwards
    {
        public int OneUpWinner { get; set; }
        public int OneUpLoser { get; set; }
        public int OneUpPerRound { get; set; }

        public int ThreeRoundPassWinner { get; set; }
        public int ThreeRoundPassLoser { get; set; }
        public int ThreeRoundPassPerRound { get; set; }

        public int CopyCatWinner { get; set; }
        public int CopyCatLoser { get; set; }
        public int CopyCatPerRound { get; set; }

        public int Flag { get; set; }
        public int CorrectVote { get; set; }
    }
}
