using System;
using System.Linq;
using Battles.Configuration;
using Battles.Enums;
using Battles.Helpers;
using Battles.Models;
using Battles.Rules.Matches.Extensions;

namespace Battles.Rules.Matches.Actions.Update
{
    public class ThreeRoundPassManager : IMatchManager
    {
        private readonly Match _match;
        private readonly Routing _routing;

        public ThreeRoundPassManager(
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

            var host = _match.GetHost();
            var opponent = _match.GetOpponent();
            
            if (_match.TurnType == TurnType.Blitz && _match.Videos.Count(x => x.UserId == user.UserId) == 3)
            {
                user.SetGoFlagUpdatePassLock(false, false, true, lockIn: true);
            }
            else if (_match.TurnType == TurnType.Classic)
            {
                if (_match.IsTurn(MatchRole.Host))
                {
                    host.SetGoFlagUpdatePassLock(false, false, true);
                    opponent.SetGoFlagUpdatePassLock(true, true, false);
                }
                else if (_match.IsTurn(MatchRole.Opponent))
                {
                    host.SetGoFlagUpdatePassLock(true, true, false);
                    opponent.SetGoFlagUpdatePassLock(false, false, true);
                    _match.Round++;
                }
                
                _match.Turn = _match.GetTurnName();
            }
            else if (_match.TurnType == TurnType.Alternating)
            {
                if (_match.IsTurn(MatchRole.Host))
                {
                    if (_match.Round == 2)
                    {
                        host.SetGoFlagUpdatePassLock(true, false, true);
                        opponent.SetGoFlagUpdatePassLock(false, true, false);
                        _match.Round++;
                    }
                    else
                    {
                        host.SetGoFlagUpdatePassLock(false, false, true);
                        opponent.SetGoFlagUpdatePassLock(true, true, false);
                    }
                }
                else if (_match.IsTurn(MatchRole.Opponent))
                {
                    if (_match.Round % 2 == 0)
                    {
                        host.SetGoFlagUpdatePassLock(true, true, false);
                        opponent.SetGoFlagUpdatePassLock(false, false, true);
                    }
                    else
                    {
                        host.SetGoFlagUpdatePassLock(false, true, false);
                        opponent.SetGoFlagUpdatePassLock(true, false, true);
                        _match.Round++;
                    }
                }

                _match.Turn = _match.GetTurnName();
            }

            if (_match.Round > 3)
            {
                host.SetLockUser();
                opponent.SetLockUser();
            }
            
            _match.LastUpdate = DateTime.Now;
        }
    }
}