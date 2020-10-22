using Battles.Enums;
using Battles.Models;

namespace Battles.Roles
{
    public abstract class MatchUserBaseRole
    {
        private readonly MatchUser _matchUser;

        protected MatchUserBaseRole(MatchUser matchUser)
        {
            _matchUser = matchUser;
        }

        public int MatchId
        {
            get => _matchUser.MatchId;
            set => _matchUser.MatchId = value;
        }

        public Match Match
        {
            get => _matchUser.Match;
            set => _matchUser.Match = value;
        }

        public string UserId
        {
            get => _matchUser.UserId;
            set => _matchUser.UserId = value;
        }

        public UserInformation UserInfromation
        {
            get => _matchUser.User;
            set => _matchUser.User = value;
        }

        public MatchRole Role
        {
            get => _matchUser.Role;
            set => _matchUser.Role = value;
        }

        public int Index
        {
            get => _matchUser.Index;
            set => _matchUser.Index = value;
        }

        public int Points
        {
            get => _matchUser.Points;
            set => _matchUser.Points = value;
        }

        public bool Winner
        {
            get => _matchUser.Winner;
            set => _matchUser.Winner = value;
        }

        public bool Ready
        {
            get => _matchUser.Ready;
            set => _matchUser.Ready = value;
        }

        public bool CanGo
        {
            get => _matchUser.CanGo;
            set => _matchUser.CanGo = value;
        }

        public bool CanUpdate
        {
            get => _matchUser.CanUpdate;
            set => _matchUser.CanUpdate = value;
        }

        public bool CanFlag
        {
            get => _matchUser.CanFlag;
            set => _matchUser.CanFlag = value;
        }

        public bool CanPass
        {
            get => _matchUser.CanPass;
            set => _matchUser.CanPass = value;
        }

        public bool CanLockIn
        {
            get => _matchUser.CanLockIn;
            set => _matchUser.CanLockIn = value;
        }

        public bool Freeze
        {
            get => _matchUser.Freeze;
            set => _matchUser.Freeze = value;
        }
        
        public void SetActions(bool go, bool flag, bool update, bool pass = false, bool lockIn = false, bool ready = false)
        {
            CanGo = go;
            CanPass = pass;
            CanFlag = flag;
            CanUpdate = update;
            CanLockIn = lockIn;
            Ready = ready;
        }
    }
}