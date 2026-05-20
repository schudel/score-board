using FluentAssertions;
using ScoreBoard.Skills.TrueSkill;
using Xunit;

namespace ScoreBoard.Skills.Facts.TrueSkill
{
    public class DrawMarginFacts
    {
        private const double ErrorTolerance = .000001;

        [Fact]
        public void GetDrawMarginFromDrawProbabilityFact()
        {
            double beta = 25.0 / 6.0;
            // The expected values were compared against Ralf Herbrich's implementation in F#
            AssertDrawMargin(0.10, beta, 0.74046637542690541);
            AssertDrawMargin(0.25, beta, 1.87760059883033);
            AssertDrawMargin(0.33, beta, 2.5111010132487492);
        }

        private static void AssertDrawMargin(double drawProbability, double beta, double expected)
        {
            double actual = DrawMargin.GetDrawMarginFromDrawProbability(drawProbability, beta);
            actual.Should().BeApproximately(expected, ErrorTolerance);
        }
    }
}