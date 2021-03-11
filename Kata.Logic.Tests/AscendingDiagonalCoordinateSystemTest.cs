using Xunit;

namespace Kata.Logic.Tests
{
    public class AscendingDiagonalCoordinateSystemTest
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
            Assert.Equal(expected, new AscendingDiagonalTransformation().MaximumVerticalPosition(diagonal));
        }

        [Theory]
        [InlineData(0, 0, 5)]
        [InlineData(1, 1, 5)]
        [InlineData(2, 0, 3)]
        [InlineData(2, 2, 5)]
        [InlineData(4, 2, 3)]
        [InlineData(5, 0, 0)]
        [InlineData(5, 2, 2)]
        [InlineData(5, 5, 5)]
        [InlineData(6, 5, 5)]
        [InlineData(7, 2, 2)]
        [InlineData(8, 2, 2)]
        [InlineData(9, 2, 2)]
        [InlineData(11, 0, 0)]
        public void GetRow__ReturnsExpected(int diagonal, int position, int expected)
        {
            Assert.Equal(expected, new AscendingDiagonalTransformation().GetCartesianRow(diagonal, position));
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 1, 1)]
        [InlineData(2, 0, 0)]
        [InlineData(2, 2, 2)]
        [InlineData(4, 2, 2)]
        [InlineData(5, 0, 0)]
        [InlineData(5, 2, 2)]
        [InlineData(5, 5, 5)]
        [InlineData(6, 5, 6)]
        [InlineData(7, 2, 4)]
        [InlineData(8, 2, 5)]
        [InlineData(9, 2, 6)]
        [InlineData(11, 0, 6)]
        public void GetColumn__ReturnsExpected(int diagonal, int position, int expected)
        {
            Assert.Equal(expected, new AscendingDiagonalTransformation().GetCartesianColumn(diagonal, position));
        }
    }
}