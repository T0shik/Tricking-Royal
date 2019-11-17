using Battles.Rules.Matches.Extensions;
using Battles.Rules.Matches.Helpers;
using System;
using Battles.Enums;
using Battles.Helpers;
using Battles.Models;
using Battles.Shared;
using static System.String;

namespace Battles.Rules.Matches.Actions.Update
{
    public class CopyCatManager : IMatchManager
    {
        private readonly Routing _routing;
        private readonly Match _match;

        public CopyCatManager(
            Routing routing,
            Match match)
        {
            _routing = routing;
            _match = match;
        }

        public void UpdateMatch(UpdateSettings command)
        {
            var user = _match.GetUser(command.UserId);
            _match.Videos.Add(new Video
            {
                VideoIndex = _match.Videos.Count,
                UserIndex = user.Index,
                UserId = user.UserId,
                VideoPath = CdnUrlHelper.CreateVideoUrl(_routing.Cdn, _match.Id.ToString(), command.Video),
                ThumbPath = CdnUrlHelper.CreateThumbUrl(_routing.Cdn, _match.Id.ToString(), command.Thumb)
            });
            
            var tempRound = _match.Round;

            var host = _match.GetHost();
            var opponent = _match.GetOpponent();

            if (_match.Round % 2 == 1)
            {
                if (_match.IsTurn(MatchRole.Host))
                {
                    host.Points++;
                    host.SetGoFlagUpdatePassLock(false, false, true);
                    opponent.SetGoFlagUpdatePassLock(true, true, false, pass: true);
                }
                else
                {
                    opponent.Points++;
                    host.SetGoFlagUpdatePassLock(false, true, false);
                    opponent.SetGoFlagUpdatePassLock(true, false, true);
                    _match.Round++;
                }
            }
            else if (_match.IsTurn(MatchRole.Host))
            {
                host.Points++;
                host.SetGoFlagUpdatePassLock(true, false, true);
                opponent.SetGoFlagUpdatePassLock(false, true, false);
                _match.Round++;
            }
            else
            {
                host.SetGoFlagUpdatePassLock(true, true, false, pass: true);
                opponent.SetGoFlagUpdatePassLock(false, false, true);
                opponent.Points++;
            }

            if (tempRound == _match.Round)
            {
                if (IsNullOrEmpty(_match.Chain))
                    _match.Chain = command.Move.Trim();
                else
                    _match.Chain += $"|{command.Move.Trim()}";
            }

            CopyCatHelper.CompleteCopyCat(_match);

            _match.Turn = _match.GetTurnName();
            _match.LastUpdate = DateTime.Now;
        }
    }
}