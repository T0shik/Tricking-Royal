using Battles.Enums;
using Battles.Models;

namespace Battles.Rules.Matches.Actions.Create
{
    public class MatchSettings
    {
        public string UserId { get; set; }
        public Mode Mode { get; set; }
        public TurnType TurnType { get; set; }
        public Surface Surface { get; set; }
        public int TurnTime { get; set; }
        public UserInformation Host { get; set; }
        public bool IsInvite { get; set; }
    }
}