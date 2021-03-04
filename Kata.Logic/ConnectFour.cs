using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private static int ParsePieceColumn(string piecePosition) => piecePosition[0] - 'A';
    }

    public class Engine
    {
        private const int SameColorPiecesRequiredToWin = 4;
        private readonly Board _board;

        public Engine(Board board) => _board = board;

        public Player CalculateWinner()
        {
            using var columns = _board.GetPiecesByColumn().GetEnumerator();
            var winner = CalculateWinnerByGroupedBoard(columns);

            if (winner == Player.None)
            {
                using var rows = _board.GetPiecesByRow().GetEnumerator();
                winner = CalculateWinnerByGroupedBoard(rows);
            }

            return winner;
        }

        private static Player CalculateWinnerByGroupedBoard(IEnumerator<IEnumerable<Player>> groupedBoard)
        {
            var winner = Player.None;
            while (winner == Player.None && groupedBoard.MoveNext())
            {
                winner = CalculateWinnerByLine(groupedBoard.Current);
            }

            return winner;
        }

        private static Player CalculateWinnerByLine(IEnumerable<Player> piecesOfColumn)
        {
            var winner = Player.None;
            var consecutiveRed = 0;
            var consecutiveYellow = 0;

            foreach (var piece in piecesOfColumn)
            {
                if (piece == Player.Red)
                {
                    consecutiveYellow = 0;
                    ++consecutiveRed;
                }
                else if (piece == Player.Yellow)
                {
                    consecutiveRed = 0;
                    ++consecutiveYellow;
                }

                if (consecutiveRed >= SameColorPiecesRequiredToWin)
                {
                    winner = Player.Red;
                }

                if (consecutiveYellow >= SameColorPiecesRequiredToWin)
                {
                    winner = Player.Yellow;
                }
            }

            return winner;
        }
    }

    public class Board
    {
        private const int Columns = 7;
        private const int Rows = 6;

        private readonly Player[,] _pieces = new Player[Rows, Columns];
        private readonly int[] _piecesPerColumn = new int[Columns];

        public void AddPieceToColumn(int column, Player player)
        {
            var numberOfPiecesInColumn = _piecesPerColumn[column];

            _pieces[numberOfPiecesInColumn, column] = player;
            _piecesPerColumn[column]++;
        }

        public IEnumerable<IEnumerable<Player>> GetPiecesByColumn()
        {
            for (var column = 0; column < Columns; column++)
            {
                yield return Enumerable
                    .Range(0, Rows)
                    .Select(i => _pieces[i, column])
                    .ToList();
            }
        }

        public IEnumerable<IEnumerable<Player>> GetPiecesByRow()
        {
            for (var row = 0; row < Rows; ++row)
            {
                yield return Enumerable
                    .Range(0, Columns)
                    .Select(i => _pieces[row, i])
                    .ToList();
            }
        }
    }

    public enum Player
    {
        None = 0,
        Red,
        Yellow,
    }
}