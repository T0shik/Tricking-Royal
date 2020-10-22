using Battles.Enums;
using Battles.Models;

namespace Battles.Tests._Mocks
{
    public static class MatchMockCreator
    {
        public static MatchCreationContext.MatchSettings CreateSettingsWithHost(Mode mode = Mode.OneUp, TurnType type = TurnType.Classic)
        {
            var settings = CreateSettings(mode, type);
            settings.Host = UserMockCreator.CreateHost();
            return settings;
        }

        public static MatchCreationContext.MatchSettings CreateSettings(Mode mode = Mode.OneUp, TurnType type = TurnType.Classic) =>
            new MatchCreationContext.MatchSettings
            {
                Mode = mode,
                TurnType = type,
                TurnTime = 1,
            };

        public static Match CreateMatch(this MatchCreationContext.MatchSettings settings) =>
            new MatchCreationContext().CreateMatch(settings);
    }
}