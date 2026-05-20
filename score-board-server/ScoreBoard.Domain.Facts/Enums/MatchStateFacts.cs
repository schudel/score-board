using System.Collections.Generic;
using FluentAssertions;
using ScoreBoard.Domain.Enums;
using Xunit;

namespace ScoreBoard.Domain.Facts.Enums
{
    public class MatchStateFacts
    {
        [Fact]
        public void GetAllMatchStatesFact()
        {
            IList<MatchState> allMatchStates = MatchState.GetAllMatchStates;
            allMatchStates.Should().HaveCount(5);
        }

        [Theory]
        [InlineData(MatchStateEnum.Aborted, "Aborted")]
        [InlineData(MatchStateEnum.Done, "Done")]
        [InlineData(MatchStateEnum.Interrupted, "Interrupted")]
        [InlineData(MatchStateEnum.Playing, "Playing")]
        [InlineData(MatchStateEnum.Scheduled, "Scheduled")]
        public void GetMatchStateByEnumFact(MatchStateEnum matchStateEnum, string name)
        {
            MatchState matchState = MatchState.GetMatchState(matchStateEnum);
            matchState.Name.Should().Be(name);
        }

        [Fact]
        public void GetMatchStateWithNullFact()
        {
            MatchState matchState = MatchState.GetMatchState(null);
            matchState.Should().BeNull();
        }

        [Theory]
        [InlineData("Aborted", MatchStateEnum.Aborted)]
        [InlineData("Done", MatchStateEnum.Done)]
        [InlineData("Interrupted", MatchStateEnum.Interrupted)]
        [InlineData("Playing", MatchStateEnum.Playing)]
        [InlineData("Scheduled", MatchStateEnum.Scheduled)]
        public void GetMatchStateByStringFact(string name, MatchStateEnum matchStateEnum)
        {
            MatchState matchState = MatchState.GetMatchState(name);
            matchState.MatchStateEnum.Should().Be(matchStateEnum);
        }

        [Fact]
        public void GetMatchStateWithEmptyStringFact()
        {
            MatchState matchState = MatchState.GetMatchState("");
            matchState.Should().BeNull();
        }
    }
}
