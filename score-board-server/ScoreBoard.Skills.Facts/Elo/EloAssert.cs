using FluentAssertions;
using ScoreBoard.Skills.Elo;

namespace ScoreBoard.Skills.Facts.Elo
{
    internal static class EloAssert
    {
        private const double ErrorTolerance = 0.1;

        public static void AssertChessRating(TwoPlayerEloCalculator calculator,
                                             double player1BeforeRating,
                                             double player2BeforeRating,
                                             PairwiseComparison player1Result,
                                             double player1AfterRating,
                                             double player2AfterRating)
        {
            var player1 = new Player(1);
            var player2 = new Player(2);

            var teams = Teams.Concat(
                new Team(player1, new EloRating(player1BeforeRating)),
                new Team(player2, new EloRating(player2BeforeRating)));

            var chessGameInfo = new GameInfo(1200, 0, 200, 0, 0);

            var result = calculator.CalculateNewRatings(chessGameInfo, teams,
                (player1Result == PairwiseComparison.Win) ? new[] { 1, 2 } :
                (player1Result == PairwiseComparison.Lose) ? new[] { 2, 1 } :
                new[] { 1, 1 });


            player1AfterRating.Should().BeApproximately(result[player1].Mean, ErrorTolerance);
            player2AfterRating.Should().BeApproximately(result[player2].Mean, ErrorTolerance);
        }
    }
}
