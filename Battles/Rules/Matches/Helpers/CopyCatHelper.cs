using Battles.Enums;
using Battles.Models;
using Battles.Rules.Levels;
using Battles.Rules.Matches.Extensions;

namespace Battles.Rules.Matches.Helpers
{
    public class CopyCatHelper
    {
        public static bool CompleteCopyCat(Match match)
        {
            if (match.Round < 4) return false;

            var host = match.GetHost();
            var opponent = match.GetOpponent();

            if (match.Round == 4)
            {
                if (!(host.Points - opponent.Points == 2 ||
                    opponent.Points - host.Points == 1))
                    return false;
            }

            if (host.Points > opponent.Points)
            {
                host.SetWinnerAndLock(12).AwardExp(12);
                opponent.SetLoserAndLock(6).AwardExp(6);
;
            }
            else if (host.Points < opponent.Points)
            {
                opponent.SetWinnerAndLock(12).AwardExp(12);
                host.SetLoserAndLock(6).AwardExp(6);
            }
            else
            {
                opponent.SetDraw(8).AwardExp(10);
                host.SetDraw(8).AwardExp(10);
            }

            if (match.IsTurn(MatchRole.Host))
                match.Round--;

            match.Status = Status.Complete;

            host.User.Hosting--;
            opponent.User.Joined--;

            return true;
        }
    }
}
