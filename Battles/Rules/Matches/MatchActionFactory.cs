using Battles.Configuration;
using Battles.Enums;
using Battles.Models;
//using Battles.Rules.Matches.Actions.ReUpload;
using Battles.Rules.Matches.Actions.Update;

namespace Battles.Rules.Matches
{
    public class MatchActionFactory
    {
        private readonly Routing _routing;

        public MatchActionFactory(Routing routing)
        {
            _routing = routing;
        }

        public IMatchManager CreateMatchManager(Match match)
        {
            if (match == null)
                throw new System.Exception("Match Not Found");

            switch (match.Mode)
            {
                case Mode.OneUp:
                    return new OneUpManager(_routing, match);
                case Mode.CopyCat:
                    return new CopyCatManager(_routing, match);
                case Mode.ThreeRoundPass:
                    return new ThreeRoundPassManager(_routing, match);
                default:
                    throw new MatchException("Failed to create a manager");
            }
        }

//        public IReUploadManager CreateReUploadManager(Match match)
//        {
//            if (match == null)
//                throw new System.Exception("Match Not Found");
//
//            switch (match.Mode)
//            {
//                case Mode.OneUp:
//                    return new OneUpReUploadManager(_routing, match);
//                case Mode.CopyCat:
//                    return new CopyCatReUploadManager(_routing, match);
//                case Mode.ThreeRoundPass:
//                    return new ThreeRoundPassReUploadManager(_routing, match);
//                default:
//                    throw new MatchException("Failed to create a manager");
//            }
//        }
    }
}