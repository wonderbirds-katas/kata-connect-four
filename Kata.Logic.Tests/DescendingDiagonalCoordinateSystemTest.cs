using Xunit;

namespace Kata.Logic.Tests
{
    public class DescendingDiagonalCoordinateSystemTest
    {
        [Theory]
        [InlineData(0, 1)]
        [InlineData(2, 3)]
        [InlineData(5, 6)]
        [InlineData(6, 6)]
        [InlineData(10, 2)]
        [InlineData(11, 1)]
        public void Positions_GivenDiagonal_ReturnsExpected(int diagonal, int expected)
        {
            Assert.Equal(expected, new DescendingDiagonalCoordinateSystem().Positions(diagonal));
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 1, 1)]
        [InlineData(5, 5, 5)]
        [InlineData(6, 5, 5)]
        [InlineData(11, 0, 5)]
        public void GetRow__ReturnsExpected(int diagonal, int position, int expected)
        {
            Assert.Equal(expected, new DescendingDiagonalCoordinateSystem().GetRow(diagonal, position));
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 1, 0)]
        [InlineData(3, 1, 2)]
        [InlineData(5, 5, 0)]
        [InlineData(6, 0, 6)]
        [InlineData(6, 5, 1)]
        [InlineData(7, 0, 6)]
        [InlineData(8, 2, 4)]
        [InlineData(10, 0, 6)]
        public void GetColumn__ReturnsExpected(int diagonal, int position, int expected)
        {
            Assert.Equal(expected, new DescendingDiagonalCoordinateSystem().GetColumn(diagonal, position));
        }
    }
}