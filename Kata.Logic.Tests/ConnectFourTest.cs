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
        [InlineData("A_Red", "A_Yellow", "A_Red", "B_Yellow", "A_Red", "B_Yellow", "A_Red")]
        public static void WhoIsWinner_NonWinningMoves_ReturnsDraw(params string[] piecesPositionList)
        {
            Assert.Equal("Draw", ConnectFour.WhoIsWinner(piecesPositionList.ToList()));
        }

        [Theory]
        [InlineData("Red", "A_Red", "B_Yellow", "A_Red", "B_Yellow", "A_Red", "B_Yellow", "A_Red")]
        [InlineData("Yellow", "A_Red", "A_Yellow", "B_Red", "A_Yellow", "B_Red", "A_Yellow", "B_Red", "A_Yellow")]
        [InlineData("Yellow", "G_Red", "G_Yellow", "B_Red", "G_Yellow", "B_Red", "G_Yellow", "B_Red", "G_Yellow")]

        // Codewars FirstTest
        [InlineData("Yellow", "A_Red", "B_Yellow", "A_Red", "B_Yellow", "A_Red", "B_Yellow", "G_Red", "B_Yellow")]
        public static void WhoIsWinner_WinByColumn_ReturnsWinner(params string[] testData)
        {
            var expected = testData[0];
            var piecesPositionList = testData.Skip(1).ToList();

            Assert.Equal(expected, ConnectFour.WhoIsWinner(piecesPositionList));
        }

        [Theory]
        [InlineData("Red", "A_Red", "A_Yellow", "B_Red", "B_Yellow", "C_Red", "C_Yellow", "D_Red")]
        public static void WhoIsWinner_WinByRow_ReturnsWinner(params string[] testData)
        {
            var expected = testData[0];
            var piecesPositionList = testData.Skip(1).ToList();

            Assert.Equal(expected, ConnectFour.WhoIsWinner(piecesPositionList));
        }
    }
}