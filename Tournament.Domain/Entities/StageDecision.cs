﻿using System.Collections.Generic;
using Battles.Models;

namespace Tournaments.Entities
{
    public class StageDecision
    {
        public int Id { get; set; }

        public int StageMatchId { get; set; }
        public StageMatch StageMatch { get; set; }

        public string UserId { get; set; }
        public UserInformation User { get; set; }

        public ICollection<StageVote> Votes { get; set; }

        public string Review { get; set; }
    }
}
