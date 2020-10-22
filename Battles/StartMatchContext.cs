using Battles.Enums;
using Battles.Models;
using Battles.Roles;

namespace Battles
{
    public class StartMatchContext  : IContext
    {
        private OpenMatch _match;

        public virtual StartMatchContext Setup(Match match)
        {
            _match = match.Mode switch
            {
                Mode.OneUp => new OpenMatch(match),
            };
            
            return this;
        }

        public virtual bool Start()
        {
            return _match.CanStart() && _match.Start();
        }
    }
}