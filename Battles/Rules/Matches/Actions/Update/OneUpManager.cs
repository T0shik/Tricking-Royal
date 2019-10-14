using System;
using Battles.Configuration;
using Battles.Enums;
using Battles.Helpers;
using Battles.Models;
using Battles.Rules.Matches.Extensions;

namespace Battles.Rules.Matches.Actions.Update
{
    public class OneUpManager : IMatchManager
    {
        private readonly Match _match;
        private readonly Routing _routing;

        public OneUpManager(
            Routing routing,
            Match match)
        {
            _match = match;
            _routing = routing;
        }

        public void UpdateMatch(UpdateSettings command)
        {
            var host = _match.GetHost();
            var opponent = _match.GetOpponent();

            if (_match.IsTurn(MatchRole.Host))
            {
                host.SetGoFlagUpdatePassLock(false, false, true);
                opponent.SetGoFlagUpdatePassLock(true, true, false);
            }
            else
            {
                host.SetGoFlagUpdatePassLock(true, true, false);
                opponent.SetGoFlagUpdatePassLock(false, false, true);
                _match.Round++;
            }

            var user = _match.GetUser(command.UserId);
            _match.Videos.Add(new Video
            {
                VideoIndex = _match.Videos.Count,
                UserIndex = user.Index,
                UserId = user.UserId,
                VideoPath = CdnUrlHelper.CreateVideoUrl(_routing.Cdn, _match.Id.ToString(), command.Video),
                ThumbPath = CdnUrlHelper.CreateThumbUrl(_routing.Cdn, _match.Id.ToString(), command.Thumb)
            });

            _match.Chain += $" {command.Move.Trim()} ->";
            _match.Turn = _match.GetTurnName();
            _match.LastUpdate = DateTime.Now;
        }
    }
}