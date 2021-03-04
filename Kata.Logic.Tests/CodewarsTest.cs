using System.Collections.Generic;
using Xunit;

namespace Kata.Logic.Tests
{
    public class CodewarsTest
    {
#pragma warning disable xUnit1004 // Test methods should not be skipped

        [Fact(Skip = "Not implemented")]
        public void SecondTest()
        {
            // Diagonal von D6 nach G3
            List<string> myList = new List<string>()
            {
                "C_Yellow",
                "E_Red",
                "G_Yellow",
                "B_Red",
                "D_Yellow",
                "B_Red",
                "B_Yellow",
                "G_Red",
                "C_Yellow",
                "C_Red",
                "D_Yellow",
                "F_Red",
                "E_Yellow",
                "A_Red",
                "A_Yellow",
                "G_Red",
                "A_Yellow",
                "F_Red",
                "F_Yellow",
                "D_Red",
                "B_Yellow",
                "E_Red",
                "D_Yellow",
                "A_Red",
                "G_Yellow",
                "D_Red",
                "D_Yellow",
                "C_Red"
            };
            Assert.Equal("Yellow", ConnectFour.WhoIsWinner(myList));
        }

        [Fact(Skip = "Not implemented")]
        public void ThirdTest()
        {
            // First 4 wins - here red has the first four (row) before yellow (diagonal)
            List<string> myList = new List<string>()
            {
                "A_Yellow",
                "B_Red",
                "B_Yellow",
                "C_Red",
                "G_Yellow",
                "C_Red",
                "C_Yellow",
                "D_Red",
                "G_Yellow",
                "D_Red",
                "G_Yellow",
                "D_Red",
                "F_Yellow",
                "E_Red",
                "D_Yellow"
            };
            Assert.Equal("Red", ConnectFour.WhoIsWinner(myList));
        }
    }
}