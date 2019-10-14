using Battles.Domain.Models;
using Battles.Rules.Matches.Extensions;
using System;
using Battles.Attributes;
using Battles.Enums;
using Battles.Interfaces;
using Battles.Models;

namespace Battles.Rules.Matches.Actions.Join
{
    [Service]
    public class MatchDoorman
    {
        private readonly IOpponentChecker _checker;

        public MatchDoorman(IOpponentChecker checker)
        {
            _checker = checker;
        }

        public void AddOpponent(Match match, UserInformation user)
        {
            if (match.Status != Status.Open && match.Status != Status.Invite)
                throw new CantJoinMatchException("Can't join an active match.");
            if (user.Joined >= user.JoinedLimit)
                throw new CantJoinMatchException("Join limit reached.");
            if (match.UserInRole(user.Id, MatchRole.Host))
                throw new CantJoinMatchException("Cant join your own match.");
            _checker.LoadOpponents(user.Id);
            if (_checker.AreOpponents(match.GetHost().UserId, user.Id))
                throw new CantJoinMatchException("Already battling this user.");

            //todo check for invitation

            match.MatchUsers.Add(new MatchUser
            {
                Index =  match.MatchUsers.Count,
                UserId = user.Id,
                User = user,
                Role = MatchRole.Opponent
            });

            match.Status = Status.Active;
            user.Joined++;
            match.GetHost().SetGoFlagUpdatePassLock(true, false, false);

            if (match.Mode == Mode.ThreeRoundPass && match.TurnType == TurnType.Blitz)
            {
                match.GetOpponent().SetGoFlagUpdatePassLock(true, false, false);
            }
            else
            {
                match.Turn = match.GetTurnName();
            }

            match.LastUpdate = DateTime.Now;
        }
    }
}