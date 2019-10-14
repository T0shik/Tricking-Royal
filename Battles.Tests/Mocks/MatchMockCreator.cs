using Battles.Domain.Models;
using Battles.Enums;
using Battles.Models;
using Battles.Rules.Matches.Actions.Create;

namespace Battles.Tests.Mocks
{
    public static class MatchMockCreator
    {
        public static MatchSettings CreateSettingsWithHost(Mode mode = Mode.OneUp, TurnType type = TurnType.Classic)
        {
            var settings = CreateSettings(mode, type);
            settings.Host = UserMockCreator.CreateHost();
            return settings;
        }

        public static MatchSettings CreateSettings(Mode mode = Mode.OneUp, TurnType type = TurnType.Classic) =>
            new MatchSettings
            {
                Mode = mode,
                TurnType = type,
                TurnTime = 1,
            };

        public static Match CreateMatch(this MatchSettings settings) =>
            MatchCreator.CreateMatch(settings);
    }
}