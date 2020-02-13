using System;
using System.Collections.Generic;
using Battles.Enums;

namespace Battles.Models
{
    public class Match
    {
        public int Id { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastUpdate { get; set; } = DateTime.Now;
        public int TurnDays { get; set; }
        public TurnType TurnType { get; set; } = TurnType.Blitz;
        public Status Status { get; set; } = Status.Open;
        public Mode Mode { get; set; } = Mode.OneUp;
        public Surface Surface { get; set; } = Surface.Any;

        public bool Updating { get; set; }

        public string Finished { get; set; } = "";
        public int Round { get; set; } = 1;
        public string Chain { get; set; } = "";

        public string Turn { get; set; } = "";

        public ICollection<Video> Videos { get; } = new List<Video>();
        public ICollection<MatchUser> MatchUsers { get; } = new List<MatchUser>();
        public ICollection<Like> Likes { get; } = new List<Like>();
        public ICollection<Comment> Comments { get; } = new List<Comment>();
    }
}