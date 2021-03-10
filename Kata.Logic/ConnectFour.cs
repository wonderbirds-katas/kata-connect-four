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
            //WinnerCalculators.Add(new ColumnWinnerCalculator());
            WinnerCalculators.Add(new WinnerCalculator(new RotatedCartesianCoordinateSystem())); // Win by Column
            WinnerCalculators.Add(new WinnerCalculator(new CartesianCoordinateSystem())); // Win by Row
            WinnerCalculators.Add(new WinnerCalculator(new AscendingDiagonalCoordinateSystem()));
            WinnerCalculators.Add(new WinnerCalculator(new DescendingDiagonalCoordinateSystem()));
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

    public class WinnerCalculator : AbstractWinnerCalculator
    {
        private readonly ICoordinateSystem _coordinateSystem;

        public WinnerCalculator(ICoordinateSystem coordinateSystem) =>
            _coordinateSystem = coordinateSystem;

        protected override IEnumerable<IEnumerable<Player>> GetGroupedPieces(Board board)
        {
            for (var diagonal = 0; diagonal < _coordinateSystem.HorizontalPositions; diagonal++)
            {
                var positions = _coordinateSystem.MaximumVerticalPosition(diagonal);
                var result = new List<Player>();
                for (var position = 0; position < positions; position++)
                {
                    var row = _coordinateSystem.GetCartesianRow(diagonal, position);
                    var column = _coordinateSystem.GetCartesianColumn(diagonal, position);
                    result.Add(board.GetPieceAt(row, column));
                }

                yield return result;
            }
        }
    }

    public interface ICoordinateSystem
    {
        int HorizontalPositions { get; }
        int VerticalPositions { get; }

        int MaximumVerticalPosition(int horizontalPosition);
        int GetCartesianRow(int horizontalPosition, int verticalPosition);
        int GetCartesianColumn(int horizontalPosition, int verticalPosition);
    }

    public abstract class DiagonalCoordinateSystem : ICoordinateSystem
    {
        public int HorizontalPositions => 12;
        public int VerticalPositions => HorizontalPositions / 2 - 1;

        public virtual int MaximumVerticalPosition(int horizontalPosition) =>
            horizontalPosition <= VerticalPositions ? horizontalPosition + 1 : HorizontalPositions - horizontalPosition;

        public abstract int GetCartesianRow(int horizontalPosition, int verticalPosition);
        public abstract int GetCartesianColumn(int horizontalPosition, int verticalPosition);
    }

    public class AscendingDiagonalCoordinateSystem : DiagonalCoordinateSystem
    {
        public override int GetCartesianRow(int horizontalPosition, int verticalPosition)
        {
            if (horizontalPosition <= VerticalPositions)
            {
                return VerticalPositions - horizontalPosition + verticalPosition;
            }

            return verticalPosition;
        }

        public override int GetCartesianColumn(int horizontalPosition, int verticalPosition)
        {
            if (horizontalPosition <= VerticalPositions)
            {
                return verticalPosition;
            }

            var positions = MaximumVerticalPosition(horizontalPosition);
            return Board.Columns - (positions - verticalPosition);
        }
    }

    public class DescendingDiagonalCoordinateSystem : DiagonalCoordinateSystem
    {
        public override int GetCartesianRow(int horizontalPosition, int verticalPosition)
        {
            if (horizontalPosition <= VerticalPositions + 1)
            {
                return verticalPosition;
            }

            return horizontalPosition - VerticalPositions - 1 + verticalPosition;
        }

        public override int GetCartesianColumn(int horizontalPosition, int verticalPosition)
        {
            if (horizontalPosition <= VerticalPositions + 1)
            {
                return horizontalPosition - verticalPosition;
            }

            return Board.Columns - 1 - verticalPosition;
        }
    }

    public class CartesianCoordinateSystem : ICoordinateSystem
    {
        public int HorizontalPositions => Board.Columns - 1;
        public int VerticalPositions => Board.Rows - 1;

        public int MaximumVerticalPosition(int horizontalPosition) => VerticalPositions;

        public int GetCartesianRow(int horizontalPosition, int verticalPosition) => horizontalPosition;

        public int GetCartesianColumn(int horizontalPosition, int verticalPosition) => verticalPosition;
    }

    public class RotatedCartesianCoordinateSystem : ICoordinateSystem
    {
        public int HorizontalPositions => Board.Rows - 1;
        public int VerticalPositions => Board.Columns - 1;

        public int MaximumVerticalPosition(int horizontalPosition) => HorizontalPositions;

        public int GetCartesianRow(int horizontalPosition, int verticalPosition) => verticalPosition;

        public int GetCartesianColumn(int horizontalPosition, int verticalPosition) => horizontalPosition;
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