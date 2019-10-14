using Battles.Enums;
using Battles.Rules.Matches;
using Battles.Tests.Mocks;
using Xunit;

namespace Battles.Tests.Matches
{
    public class MatchCreationTests
    {
        [Theory]
        [InlineData(Mode.OneUp, TurnType.Classic)]
        [InlineData(Mode.ThreeRoundPass, TurnType.Classic)]
        [InlineData(Mode.ThreeRoundPass, TurnType.Alternating)]
        [InlineData(Mode.ThreeRoundPass, TurnType.Blitz)]
        [InlineData(Mode.CopyCat, TurnType.Classic)]
        [InlineData(Mode.CopyCat, TurnType.Alternating)]
        public void CreatesOneUp_WhenModeSetToOneUp(Mode mode, TurnType type)
        {
            var match = MatchMockCreator.CreateSettingsWithHost(mode, type)
                .CreateMatch();

            Assert.Equal(mode, match.Mode);
        }

        [Theory]
        [InlineData(Surface.Any)]
        [InlineData(Surface.Concrete)]
        [InlineData(Surface.Grass)]
        [InlineData(Surface.Trampoline)]
        [InlineData(Surface.Tumbling)]
        [InlineData(Surface.SprungFloor)]
        public void CreatesMatch_WithSelectedSurface(Surface surface)
        {
            var settings = MatchMockCreator.CreateSettingsWithHost();
            settings.Surface = surface;
            var match = settings.CreateMatch();
            Assert.Equal(surface, match.Surface);
        }

        [Fact]
        public void Match_HasOnlyHostUserWithId_WhenCreated()
        {
            var match = MatchMockCreator.CreateSettingsWithHost()
                .CreateMatch();

            Assert.True(match.MatchUsers.Count == 1);
            Assert.Contains(match.MatchUsers, x =>
                x.Role == MatchRole.Host && !string.IsNullOrEmpty(x.UserId));
        }

        [Fact]
        public void IncrementsHostingCount_ForHost_WhenMatchCreated()
        {
            var host = UserMockCreator.CreateHost();
            var settings = MatchMockCreator.CreateSettings();
            settings.Host = host;
            Assert.Equal(0, host.Hosting);
            settings.CreateMatch();

            Assert.Equal(1, host.Hosting);
        }

        [Fact]
        public void ThrowsUserNotFoundExceptionWhen_HostIsNull()
        {
            Assert.Throws<UserNotFoundException>(() => MatchMockCreator.CreateSettings().CreateMatch());
        }

        [Fact]
        public void ThrowsHostingLimitReachedException_WhenHostingLimitReached()
        {
            var settings = MatchMockCreator.CreateSettingsWithHost();
            settings.Host.Hosting = 1;
            Assert.Equal(settings.Host.Hosting, settings.Host.HostingLimit);
            Assert.Throws<HostingLimitReachedException>(() => settings.CreateMatch());
        }
    }
}