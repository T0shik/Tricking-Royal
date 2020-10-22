namespace Battles.Tests.Contexts
{
    public class StartMatchContextTests
    {
      
   

        // [Fact]
        // public void InformationAsExpected_WhenModeIsThreeRoundPass_AndTurnTypeIsClassic()
        // {
        //     var settings = MatchMockCreator.CreateSettings(Mode.ThreeRoundPass);
        //     settings.Host = _host;
        //     settings.TurnType = TurnType.Classic;
        //     var match = settings.CreateMatch();
        //     var context = new JoinMatchContext(_opponentCheckerMock.Object);
        //     context.Setup(match).AddOpponent(_opponent);
        //
        //     Assert.Equal(Status.Active, match.Status);
        //     Assert.True(match.GetHost().CanGo);
        //     Assert.False(match.GetOpponent().CanGo);
        //     Assert.Equal(_host.DisplayName, match.Turn);
        // }
        //
        // [Fact]
        // public void InformationAsExpected_WhenModeIsThreeRoundPass_AndTurnTypeIsAlternating()
        // {
        //     var settings = MatchMockCreator.CreateSettings(Mode.ThreeRoundPass);
        //     settings.Host = _host;
        //     settings.TurnType = TurnType.Alternating;
        //     var match = settings.CreateMatch();
        //     var context = new JoinMatchContext(_opponentCheckerMock.Object);
        //     context.Setup(match).AddOpponent(_opponent);
        //
        //     Assert.Equal(Status.Active, match.Status);
        //     Assert.True(match.GetHost().CanGo);
        //     Assert.False(match.GetOpponent().CanGo);
        //     Assert.Equal(_host.DisplayName, match.Turn);
        // }
        //
        // [Fact]
        // public void InformationAsExpected_WhenModeIsThreeRoundPass_AndTurnTypeIsBlitz()
        // {
        //     var settings = MatchMockCreator.CreateSettingsWithHost(Mode.ThreeRoundPass);
        //     settings.TurnType = TurnType.Blitz;
        //     var match = settings.CreateMatch();
        //     var context = new JoinMatchContext(_opponentCheckerMock.Object);
        //     context.Setup(match).AddOpponent(_opponent);
        //
        //     Assert.Equal(Status.Active, match.Status);
        //     Assert.True(match.GetHost().CanGo);
        //     Assert.True(match.GetOpponent().CanGo);
        //     Assert.Equal("", match.Turn);
        // }
        //
        // [Fact]
        // public void InformationAsExpected_WhenModeIsCopyCat()
        // {
        //     var settings = MatchMockCreator.CreateSettings(Mode.CopyCat);
        //     settings.Host = _host;
        //     var match = settings.CreateMatch();
        //     var context = new JoinMatchContext(_opponentCheckerMock.Object);
        //     context.Setup(match).AddOpponent(_opponent);
        //
        //     Assert.Equal(Status.Active, match.Status);
        //     Assert.True(match.GetHost().CanGo);
        //     Assert.False(match.GetOpponent().CanGo);
        //     Assert.Equal(_host.DisplayName, match.Turn);
        // }
    }
}