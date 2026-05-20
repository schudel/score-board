using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ScoreBoard.Skills.Elo;
using Xunit;

namespace ScoreBoard.Skills.Facts.Elo
{
    public class DuellingEloFacts
    {
        private const double ErrorTolerance = 0.1;

        [Fact]
        public void TwoOnTwoDuellingFact()
        {
            var calculator = new DuellingEloCalculator(new GaussianEloCalculator());

            var player1 = new Player(1);
            var player2 = new Player(2);

            var gameInfo = GameInfo.DefaultGameInfo;

            var team1 = new Team()
                .AddPlayer(player1, gameInfo.DefaultRating)
                .AddPlayer(player2, gameInfo.DefaultRating);

            var player3 = new Player(3);
            var player4 = new Player(4);

            var team2 = new Team()
                        .AddPlayer(player3, gameInfo.DefaultRating)
                        .AddPlayer(player4, gameInfo.DefaultRating);

            var teams = Teams.Concat(team1, team2);
            IEnumerable<IDictionary<Player, Rating>> teamEnumerable = teams as IDictionary<Player, Rating>[] ?? teams.ToArray();
            var newRatingsWinLose = calculator.CalculateNewRatings(gameInfo, teamEnumerable, 1, 2);

            // TODO: Verify?
            AssertRating(37, newRatingsWinLose[player1]);
            AssertRating(37, newRatingsWinLose[player2]);
            AssertRating(13, newRatingsWinLose[player3]);
            AssertRating(13, newRatingsWinLose[player4]);

            var quality = calculator.CalculateMatchQuality(gameInfo, teamEnumerable);

            quality.Should().BeApproximately(1.0, 0.001);
        }

        private static void AssertRating(double expected, Rating actual)
        {
            actual.Mean.Should().BeApproximately(expected, ErrorTolerance);
        }
    }
}
