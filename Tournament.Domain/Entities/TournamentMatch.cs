using System;
using System.ComponentModel.DataAnnotations;
using Battles.Enums;
using Battles.Models;

namespace Tournament.Domain.Models
{
    public class TournamentMatch
    {
        [Key]
        public int Id { get; set; }

        public int HostId { get; set; }
        public UserInformation Host { get; set; }

        public int OpponentId { get; set; }
        public UserInformation Opponent { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastUpdate { get; set; } = DateTime.Now;
        public string Finished { get; set; } = "";

        public int TurnHours { get; set; } = 0;
        public int TurnDays { get; set; } = 0;
        public int Round { get; set; } = 1;
        public string Chain { get; set; } = "";
        
        public TurnType TurnType { get; set; } = TurnType.Blitz;
        public Status Status { get; set; } = Status.Open;
        public Mode Mode { get; set; } = Mode.OneUp;
        public Surface Surface { get; set; } = Surface.Any;

        public string FinalPath { get; set; } = "";
        public string PreviousPath { get; set; } = "";

        public string FinalPathHost { get; set; } = "";
        public string FinalPathOpponent { get; set; } = "";

        public string MainThumb { get; set; } = "";
        public string SecondThumb { get; set; } = "";

        public bool HostWon { get; set; } = false;
    }
}
