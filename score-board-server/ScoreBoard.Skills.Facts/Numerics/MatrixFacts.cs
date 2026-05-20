using FluentAssertions;
using ScoreBoard.Skills.Numerics;
using Xunit;

namespace ScoreBoard.Skills.Facts.Numerics
{
    public class MatrixFacts
    {
        [Fact]
        public void TwoByTwoDeterminantFact()
        {
            var a = new SquareMatrix(1, 2, 3, 4);
            a.Determinant.Should().Be(-2);

            var b = new SquareMatrix(3, 4, 5, 6);
            b.Determinant.Should().Be(-2);

            var c = new SquareMatrix(1, 1, 1, 1);
            c.Determinant.Should().Be(0);

            var d = new SquareMatrix(12, 15, 17, 21);
            d.Determinant.Should().Be(12 * 21 - 15 * 17);
        }

        [Fact]
        public void ThreeByThreeDeterminantFact()
        {
            var a = new SquareMatrix(1, 2, 3, 4, 5, 6, 7, 8, 9);
            a.Determinant.Should().Be(0);

            var π = new SquareMatrix(3, 1, 4, 1, 5, 9, 2, 6, 5);

            // Verified against http://www.wolframalpha.com/input/?i=determinant+%7B%7B3%2C1%2C4%7D%2C%7B1%2C5%2C9%7D%2C%7B2%2C6%2C5%7D%7D
            π.Determinant.Should().Be(-90);
        }

        [Fact]
        public void FourByFourDeterminantFact()
        {
            var a = new SquareMatrix( 1,  2,  3,  4, 5,  6,  7,  8, 9, 10, 11, 12, 13, 14, 15, 16);
            a.Determinant.Should().Be(0);

            var π = new SquareMatrix(3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 3);

            // Verified against http://www.wolframalpha.com/input/?i=determinant+%7B+%7B3%2C1%2C4%2C1%7D%2C+%7B5%2C9%2C2%2C6%7D%2C+%7B5%2C3%2C5%2C8%7D%2C+%7B9%2C7%2C9%2C3%7D%7D
            π.Determinant.Should().Be(98);
        }

        [Fact]
        public void EightByEightDeterminantFact()
        {
            var a = new SquareMatrix( 1,   2,  3,  4,  5,  6,  7,  8,
                                      9,  10, 11, 12, 13, 14, 15, 16,
                                      17, 18, 19, 20, 21, 22, 23, 24, 
                                      25, 26, 27, 28, 29, 30, 31, 32,
                                      33, 34, 35, 36, 37, 38, 39, 40,
                                      41, 42, 32, 44, 45, 46, 47, 48,
                                      49, 50, 51, 52, 53, 54, 55, 56,
                                      57, 58, 59, 60, 61, 62, 63, 64);
            a.Determinant.Should().Be(0);

            var π = new SquareMatrix(3, 1, 4, 1, 5, 9, 2, 6, 
                                     5, 3, 5, 8, 9, 7, 9, 3,
                                     2, 3, 8, 4, 6, 2, 6, 4, 
                                     3, 3, 8, 3, 2, 7, 9, 5,
                                     0, 2, 8, 8, 4, 1, 9, 7,
                                     1, 6, 9, 3, 9, 9, 3, 7,
                                     5, 1, 0, 5, 8, 2, 0, 9, 
                                     7, 4, 9, 4, 4, 5, 9, 2);

            // Verified against http://www.wolframalpha.com/input/?i=det+%7B%7B3%2C1%2C4%2C1%2C5%2C9%2C2%2C6%7D%2C%7B5%2C3%2C5%2C8%2C9%2C7%2C9%2C3%7D%2C%7B2%2C3%2C8%2C4%2C6%2C2%2C6%2C4%7D%2C%7B3%2C3%2C8%2C3%2C2%2C7%2C9%2C5%7D%2C%7B0%2C2%2C8%2C8%2C4%2C1%2C9%2C7%7D%2C%7B1%2C6%2C9%2C3%2C9%2C9%2C3%2C7%7D%2C%7B5%2C1%2C0%2C5%2C8%2C2%2C0%2C9%7D%2C%7B7%2C4%2C9%2C4%2C4%2C5%2C9%2C2%7D%7D
            π.Determinant.Should().Be(1378143);
        }

        [Fact]
        public void EqualsFact()
        {
            var a = new SquareMatrix(1, 2, 3, 4);
            var b = new SquareMatrix(1, 2, 3, 4);
            a.Should().Be(b);
            a.Should().BeEquivalentTo(b);

            var c = new Matrix(2, 3, 1, 2, 3, 4, 5, 6);
            var d = new Matrix(2, 3, 1, 2, 3, 4, 5, 6);
            c.Should().Be(d);
            c.Should().BeEquivalentTo(d);

            var e = new Matrix(3, 2, 1, 4, 2, 5, 3, 6);
            var f = e.Transpose;
            d.Should().Be(f);
            d.Should().BeEquivalentTo(f);
            d.GetHashCode().Should().Be(f.GetHashCode());

            // Test rounding (thanks to nsp on GitHub for finding this case)
            var g = new SquareMatrix(1, 2.00000000000001, 3, 4);
            var h = new SquareMatrix(1, 2, 3, 4);
            g.Should().Be(h);
            g.Should().BeEquivalentTo(h);
            g.GetHashCode().Should().Be(h.GetHashCode());
        }

        [Fact]
        public void AdjugateFact()
        {
            // From Wikipedia: http://en.wikipedia.org/wiki/Adjugate_matrix
            var a = new SquareMatrix(1, 2, 3, 4);
            var b = new SquareMatrix( 4, -2, -3, 1);
            b.Should().Be(a.Adjugate);

            var c = new SquareMatrix(-3,  2, -5, -1,  0, -2, 3, -4,  1);
            var d = new SquareMatrix(-8, 18, -4, -5, 12, -1, 4, -6, 2);
            d.Should().Be(c.Adjugate);
        }

        [Fact]
        public void InverseFact()
        {
            // see http://www.mathwords.com/i/inverse_of_a_matrix.htm
            var a = new SquareMatrix(4, 3, 3, 2);
            var b = new SquareMatrix(-2, 3, 3, -4);
            var aInverse = a.Inverse;
            b.Should().Be(aInverse);

            var identity2X2 = new IdentityMatrix(2);
            var aaInverse = a * aInverse;
            aaInverse.Should().Be(identity2X2);

            var c = new SquareMatrix(1, 2, 3, 0, 4, 5, 1, 0, 6);
            var cInverse = c.Inverse;
            var d = (1.0 / 22) * new SquareMatrix(24, -12, -2, 5,   3, -5, -4,   2,  4);
            d.Should().Be(cInverse);

            var identity3X3 = new IdentityMatrix(3);
            var ccInverse = c * cInverse;
            identity3X3.Should().Be(ccInverse);
        }
    }
}