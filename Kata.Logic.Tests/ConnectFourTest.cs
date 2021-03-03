using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Kata.Logic.Tests
{
    public class ConnectFourTest
    {
        [Theory]
        [InlineData]
        [InlineData("A_Red")]
        [InlineData("A_Red", "B_Yellow")]
        public static void WhoIsWinner_NonWinningMoves_ReturnsDraw(params string[] piecesPositionList)
        {
            Assert.Equal("Draw", ConnectFour.WhoIsWinner(piecesPositionList.ToList()));
        }

        [Theory]
        [InlineData("Red", "A_Red", "B_Yellow", "A_Red", "B_Yellow", "A_Red", "B_Yellow", "A_Red")]
        public static void WhoIsWinner_SimpleSequenceFirstWins_ReturnsWinner(params string[] testData)
        {
            var expected = testData[0];
            var piecesPositionList = testData.Skip(1).ToList();

            Assert.Equal(expected, ConnectFour.WhoIsWinner(piecesPositionList));
        }
    }
}