﻿using System;
using Battles.Enums;
using Battles.Models;
using Battles.Rules.Matches.Actions.Update;
using Battles.Shared;

//using Battles.Rules.Matches.Actions.ReUpload;

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
            {
                throw new Exception("Match Not Found");
            }

            switch (match.Mode)
            {
                case Mode.OneUp:
                    return new OneUpManager(_routing, match);
                case Mode.CopyCat:
                    return new CopyCatManager(_routing, match);
                case Mode.ThreeRoundPass:
                    return new ThreeRoundPassManager(_routing, match);
                case Mode.Trick:
                    throw new MatchException("Failed to create a manager");
                default:
                    throw new MatchException("Failed to create a manager");
            }
        }
    }
}