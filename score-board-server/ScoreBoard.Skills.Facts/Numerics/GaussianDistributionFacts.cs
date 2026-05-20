using System;
using FluentAssertions;
using ScoreBoard.Skills.Numerics;
using Xunit;

namespace ScoreBoard.Skills.Facts.Numerics
{
    public class GaussianDistributionFacts
    {
        private const double ErrorTolerance = 0.000001;

        [Fact]
        public void CumulativeToFact()
        {
            // Verified with WolframAlpha
            // (e.g. http://www.wolframalpha.com/input/?i=CDF%5BNormalDistribution%5B0%2C1%5D%2C+0.5%5D )
            GaussianDistribution.CumulativeTo(0.5).Should().BeApproximately(0.691462, ErrorTolerance);
        }

        [Fact]
        public void AtFact()
        {
            // Verified with WolframAlpha
            // (e.g. http://www.wolframalpha.com/input/?i=PDF%5BNormalDistribution%5B0%2C1%5D%2C+0.5%5D )
            GaussianDistribution.At(0.5).Should().BeApproximately(0.352065, ErrorTolerance);
        }

        [Fact]
        public void MultiplicationFact()
        {
            // I verified this against the formula at http://www.tina-vision.net/tina-knoppix/tina-memo/2003-003.pdf
            var standardNormal = new GaussianDistribution(0, 1);
            var shiftedGaussian = new GaussianDistribution(2, 3);

            var product = standardNormal * shiftedGaussian;

            product.Mean.Should().BeApproximately(0.2, ErrorTolerance);
            product.StandardDeviation.Should().BeApproximately(3.0 / Math.Sqrt(10), ErrorTolerance);

            var m4S5 = new GaussianDistribution(4, 5);
            var m6S7 = new GaussianDistribution(6, 7);

            var product2 = m4S5 * m6S7;
            double Square(double x) => x * x;

            var expectedMean = (4 * Square(7) + 6 * Square(5)) / (Square(5) + Square(7));
            product2.Mean.Should().BeApproximately(expectedMean, ErrorTolerance);

            var expectedSigma = Math.Sqrt(((Square(5) * Square(7)) / (Square(5) + Square(7))));
            product2.StandardDeviation.Should().BeApproximately(expectedSigma, ErrorTolerance);
        }

        [Fact]
        public void DivisionFact()
        {
            // Since the multiplication was worked out by hand, we use the same numbers but work backwards
            var product = new GaussianDistribution(0.2, 3.0 / Math.Sqrt(10));
            var standardNormal = new GaussianDistribution(0, 1);

            var productDividedByStandardNormal = product / standardNormal;
            productDividedByStandardNormal.Mean.Should().BeApproximately(2.0, ErrorTolerance);
            productDividedByStandardNormal.StandardDeviation.Should().BeApproximately(3.0, ErrorTolerance);

            double Square(double x) => x * x;
            var product2 = new GaussianDistribution((4 * Square(7) + 6 * Square(5)) / (Square(5) + Square(7)), Math.Sqrt(((Square(5) * Square(7)) / (Square(5) + Square(7)))));
            var m4S5 = new GaussianDistribution(4,5);
            var product2DividedByM4S5 = product2 / m4S5;
            product2DividedByM4S5.Mean.Should().BeApproximately(6.0, ErrorTolerance);
            product2DividedByM4S5.StandardDeviation.Should().BeApproximately(7.0, ErrorTolerance);
        }

        [Fact]
        public void LogProductNormalizationFact()
        {
            // Verified with Ralf Herbrich's F# implementation
            var standardNormal = new GaussianDistribution(0, 1);
            var lpn = GaussianDistribution.LogProductNormalization(standardNormal, standardNormal);
            lpn.Should().BeApproximately(-1.2655121234846454, ErrorTolerance);

            var m1S2 = new GaussianDistribution(1, 2);
            var m3S4 = new GaussianDistribution(3, 4);
            var lpn2 = GaussianDistribution.LogProductNormalization(m1S2, m3S4);
            lpn2.Should().BeApproximately(-2.5168046699816684, ErrorTolerance);
        }

        [Fact]
        public void LogRatioNormalizationFact()
        {
            // Verified with Ralf Herbrich's F# implementation            
            var m1S2 = new GaussianDistribution(1, 2);
            var m3S4 = new GaussianDistribution(3, 4);
            var lrn = GaussianDistribution.LogRatioNormalization(m1S2, m3S4);
            lrn.Should().BeApproximately(2.6157405972171204, ErrorTolerance);
        }

        [Fact]
        public void AbsoluteDifferenceFact()
        {
            // Verified with Ralf Herbrich's F# implementation            
            var standardNormal = new GaussianDistribution(0, 1);
            var absDiff = GaussianDistribution.AbsoluteDifference(standardNormal, standardNormal);
            absDiff.Should().BeApproximately(0.0, ErrorTolerance);

            var m1S2 = new GaussianDistribution(1, 2);
            var m3S4 = new GaussianDistribution(3, 4);
            var absDiff2 = GaussianDistribution.AbsoluteDifference(m1S2, m3S4);
            absDiff2.Should().BeApproximately(0.4330127018922193, ErrorTolerance);
        }
    }
}