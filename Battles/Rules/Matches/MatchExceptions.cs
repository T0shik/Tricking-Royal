using System;

namespace Battles.Rules.Matches
{
    public class MatchException : Exception
    {
        public MatchException(string message)
            : base(message) { }
    }

    //todo move to user exceptions
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException()
            : base("Couldn't find user profile.") { }

        public UserNotFoundException(string message)
            : base(message) { }
    }

    public class HostingLimitReachedException : Exception
    {
        public HostingLimitReachedException(string message)
            : base(message) { }
    }

    public class CantJoinMatchException : Exception
    {
        public CantJoinMatchException(string message)
            : base(message) { }
    }
}
