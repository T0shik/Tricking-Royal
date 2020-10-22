using Battles.Enums;
using Battles.Interfaces;
using Battles.Models;
using Battles.Roles;
using Battles.Rules.Matches;

namespace Battles
{
    [Service]
    public class JoinMatchContext : IContext
    {
        private readonly IOpponentChecker _checker;
        private readonly StartMatchContext _startMatchContext;
        private OpenMatch _match;

        public JoinMatchContext(
            IOpponentChecker checker,
            StartMatchContext startMatchContext)
        {
            _checker = checker;
            _startMatchContext = startMatchContext;
        }

        public JoinMatchContext Setup(Match match)
        {
            _match = new OpenMatch(match);
            return this;
        }

        public void AddOpponent(UserInformation user)
        {
            if (_match.Status != Status.Open && _match.Status != Status.Invite)
                throw new CantJoinMatchException("Can't join an active _match.");
            if (user.Joined >= user.JoinedLimit)
                throw new CantJoinMatchException("Join limit reached.");
            if (_match.IsHost(user.Id))
                throw new CantJoinMatchException("Cant join your own _match.");
            _checker.LoadOpponents(user.Id);
            if (_checker.AreOpponents(_match.GetHostId(), user.Id))
                throw new CantJoinMatchException("Already battling this user.");

            //todo check for invitation

            _match.SetOpponent(user);

            _startMatchContext.Setup(_match.Unpack()).Start();
        }
    }
}