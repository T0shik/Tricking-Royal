using Battles.Models;

namespace Battles.Roles
{
    public class OpenOneUpMatch : OpenMatch
    {
        public OpenOneUpMatch(Match match) : base(match)
        {
        }

        public override bool CanStart()
        {
            return base.CanStart();
        }

        public override bool Start()
        {
            base.Start();
            var host = GetHost();
            host.SetActions(true, false, false);
            Turn = host.UserInfromation.DisplayName;

            return true;
        }
    }
}