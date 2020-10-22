using System;
using System.Collections.Generic;
using System.Linq;
using Battles.Enums;
using Battles.Models;

namespace Battles.Roles
{
    public abstract class MatchBaseRole
    {
        private readonly Match _match;

        public MatchBaseRole(Match match)
        {
            _match = match;
        }

        public Match Unpack() => _match;

        public int Id
        {
            get => _match.Id;
            set => _match.Id = value;
        }

        public DateTime Created
        {
            get => _match.Created;
            set => _match.Created = value;
        }

        public DateTime LastUpdate
        {
            get => _match.LastUpdate;
            set => _match.LastUpdate = value;
        }

        public int TurnDays
        {
            get => _match.TurnDays;
            set => _match.TurnDays = value;
        }

        public TurnType TurnType
        {
            get => _match.TurnType;
            set => _match.TurnType = value;
        }

        public Status Status
        {
            get => _match.Status;
            set => _match.Status = value;
        }

        public Mode Mode
        {
            get => _match.Mode;
            set => _match.Mode = value;
        }

        public Surface Surface
        {
            get => _match.Surface;
            set => _match.Surface = value;
        }

        public bool Updating
        {
            get => _match.Updating;
            set => _match.Updating = value;
        }

        public string Finished
        {
            get => _match.Finished;
            set => _match.Finished = value;
        }

        public int Round
        {
            get => _match.Round;
            set => _match.Round = value;
        }

        public string Chain
        {
            get => _match.Chain;
            set => _match.Chain = value;
        }

        public string Turn
        {
            get => _match.Turn;
            set => _match.Turn = value;
        }


        public ICollection<Video> Videos => _match.Videos;
        public ICollection<MatchUser> MatchUsers => _match.MatchUsers;

        public ICollection<Like> Likes => _match.Likes;

        public ICollection<Comment> Comments => _match.Comments;

        public bool IsHost(string userId) =>
            MatchUsers.Any(x => x.UserId == userId && x.Role == MatchRole.Host);

        public bool IsOpponent(string userId) =>
            MatchUsers.Any(x => x.UserId == userId && x.Role == MatchRole.Opponent);

        public bool IsParticipant(string userId) =>
            MatchUsers.Any(x => x.UserId == userId && x.Role == MatchRole.Participant);
    }
}