using System.Collections.Generic;

namespace Kata.Logic
{
    public static class ConnectFour
    {
        public static string WhoIsWinner(List<string> piecesPositionList)
        {
            var board = new Board();
            var engine = new Engine(board);

            foreach (var piecePosition in piecesPositionList)
            {
                var column = ParsePieceColumn(piecePosition);
                var player = ParsePlayer(piecePosition);
                board.AddPieceToColumn(column, player);
            }

            var winner = engine.CalculateWinner();
            return winner == Player.None ? "Draw" : winner.ToString();
        }

        private static Player ParsePlayer(string piecePosition)
        {
            var colorString = piecePosition[2..];
            var piece = colorString == "Red" ? Player.Red : Player.Yellow;
            return piece;
        }

        private static int ParsePieceColumn(string piecePosition)
        {
            var column = piecePosition[0] == 'A' ? 0 : 1;
            return column;
        }
    }

    public class Engine
    {
        private readonly Board _board;

        public Engine(Board board) => _board = board;

        public Player CalculateWinner()
        {
            var consecutiveRed = 0;

            foreach (var piece in _board.GetPiecesByColumn())
            {
                if (piece == Player.Red)
                {
                    ++consecutiveRed;
                }
            }

            if (consecutiveRed >= 4)
            {
                return Player.Red;
            }

            return Player.None;
        }
    }

    public class Board
    {
        private readonly Player[,] _pieces = new Player[6, 7];

        public IEnumerable<Player> GetPiecesByColumn()
        {
            for (var column = 0; column < _pieces.GetUpperBound(1); column++)
            {
                for (var row = 0; row < _pieces.GetUpperBound(0); row++)
                {
                    yield return _pieces[row, column];
                }
            }
        }

        public void AddPieceToColumn(int column, Player player)
        {
            var numberOfPiecesInColumn = 0;
            while (_pieces[numberOfPiecesInColumn, column] != Player.None)
            {
                numberOfPiecesInColumn++;
            }

            _pieces[numberOfPiecesInColumn, column] = player;
        }
    }

    public enum Player
    {
        None = 0,
        Red,
        Yellow,
    }
}