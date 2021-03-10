using System.Collections.Generic;
using System.Linq;

namespace Kata.Logic
{
    public static class ConnectFour
    {
        public static string WhoIsWinner(List<string> piecesPositionList)
        {
            var board = new Board();
            var engine = new Engine();

            var winner = Player.None;
            using var piecePositionEnumerator = piecesPositionList.GetEnumerator();
            while(winner == Player.None && piecePositionEnumerator.MoveNext())
            {
                var piecePosition = piecePositionEnumerator.Current;
                var column = ParsePieceColumn(piecePosition);
                var player = ParsePlayer(piecePosition);
                board.AddPieceToColumn(column, player);
                winner = engine.CalculateWinner(board);
            }

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
        private List<IWinnerCalculator> WinnerCalculators = new();

        public Engine()
        {
            WinnerCalculators.Add(new ColumnWinnerCalculator());
            WinnerCalculators.Add(new RowWinnerCalculator());
            WinnerCalculators.Add(new DiagonalWinnerCalculator(new AscendingDiagonalCoordinateSystem()));
            WinnerCalculators.Add(new DiagonalWinnerCalculator(new DescendingDiagonalCoordinateSystem()));
        }

        public Player CalculateWinner(Board board)
        {
            var winner = Player.None;
            using var winnerCalculator = WinnerCalculators.GetEnumerator();

            while (winner == Player.None && winnerCalculator.MoveNext())
            {
                winner = winnerCalculator.Current.CalculateWinner(board);
            }

            return winner;
        }
    }

    internal interface IWinnerCalculator
    {
        Player CalculateWinner(Board board);
    }

    public class DiagonalWinnerCalculator : AbstractWinnerCalculator
    {
        private readonly DiagonalCoordinateSystem _coordinateSystem;

        public DiagonalWinnerCalculator(DiagonalCoordinateSystem coordinateSystem) =>
            _coordinateSystem = coordinateSystem;

        protected override IEnumerable<IEnumerable<Player>> GetGroupedPieces(Board board)
        {
            for (var diagonal = 0; diagonal < DiagonalCoordinateSystem.Diagonals; diagonal++)
            {
                var positions = _coordinateSystem.Positions(diagonal);
                var result = new List<Player>();
                for (var position = 0; position < positions; position++)
                {
                    var row = _coordinateSystem.GetRow(diagonal, position);
                    var column = _coordinateSystem.GetColumn(diagonal, position);
                    result.Add(board.GetPieceAt(row, column));
                }

                yield return result;
            }
        }
    }

    public abstract class DiagonalCoordinateSystem
    {
        public const int Diagonals = 12;
        protected const int LongestDiagonalIndex = Diagonals / 2;

        public virtual int Positions(int diagonal) =>
            diagonal < LongestDiagonalIndex ? diagonal + 1 : Diagonals - diagonal;

        public abstract int GetRow(int diagonal, int position);
        public abstract int GetColumn(int diagonal, int position);
    }

    public class AscendingDiagonalCoordinateSystem : DiagonalCoordinateSystem
    {
        public override int GetRow(int diagonal, int position)
        {
            if (diagonal < LongestDiagonalIndex)
            {
                return LongestDiagonalIndex - 1 - diagonal + position;
            }

            return position;
        }

        public override int GetColumn(int diagonal, int position)
        {
            if (diagonal < LongestDiagonalIndex)
            {
                return position;
            }

            var positions = Positions(diagonal);
            return Board.Columns - (positions - position);
        }
    }

    public class DescendingDiagonalCoordinateSystem : DiagonalCoordinateSystem
    {
        public override int GetRow(int diagonal, int position)
        {
            if (diagonal <= LongestDiagonalIndex)
            {
                return position;
            }

            return diagonal - LongestDiagonalIndex + position;
        }

        public override int GetColumn(int diagonal, int position)
        {
            if (diagonal <= LongestDiagonalIndex)
            {
                return diagonal - position;
            }

            return Board.Columns - 1 - position;
        }
    }

    public class RowWinnerCalculator : AbstractWinnerCalculator
    {
        protected override IEnumerable<IEnumerable<Player>> GetGroupedPieces(Board board)
        {
            for (var row = 0; row < Board.Rows; ++row)
            {
                yield return Enumerable
                    .Range(0, Board.Columns)
                    .Select(i => board.GetPieceAt(row, i))
                    .ToList();
            }
        }
    }

    public class ColumnWinnerCalculator : AbstractWinnerCalculator
    {
        protected override IEnumerable<IEnumerable<Player>> GetGroupedPieces(Board board)
        {
            for (var column = 0; column < Board.Columns; column++)
            {
                yield return Enumerable
                    .Range(0, Board.Rows)
                    .Select(i => board.GetPieceAt(i, column))
                    .ToList();
            }
        }
    }

    public abstract class AbstractWinnerCalculator : IWinnerCalculator
    {
        private const int SameColorPiecesRequiredToWin = 4;

        public Player CalculateWinner(Board board)
        {
            using var columns = GetGroupedPieces(board).GetEnumerator();
            return CalculateWinnerByGroupedBoard(columns);
        }

        protected abstract IEnumerable<IEnumerable<Player>> GetGroupedPieces(Board board);

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
                else if (piece == Player.None)
                {
                    consecutiveRed = 0;
                    consecutiveYellow = 0;
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
        public const int Columns = 7;
        public const int Rows = 6;

        private readonly Player[,] _pieces = new Player[Rows, Columns];
        private readonly int[] _piecesPerColumn = new int[Columns];

        public void AddPieceToColumn(int column, Player player)
        {
            var numberOfPiecesInColumn = _piecesPerColumn[column];

            _pieces[numberOfPiecesInColumn, column] = player;
            _piecesPerColumn[column]++;
        }

        public Player GetPieceAt(int row, int column) => _pieces[row, column];
    }

    public enum Player
    {
        None = 0,
        Red,
        Yellow,
    }
}