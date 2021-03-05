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
            Assert.Equal(expected, AscendingDiagonalCoordinateSystem.Positions(diagonal));
        }
    }
}