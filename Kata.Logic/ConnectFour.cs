using System.Collections.Generic;

namespace Kata.Logic
{
    public static class ConnectFour
    {
        public static string WhoIsWinner(List<string> piecesPositionList)
        {
            var board = new Board();
            var engine = new Engine();

            var winner = Player.None;
            using (var piecePositionEnumerator = piecesPositionList.GetEnumerator())
            {
                while (winner == Player.None && piecePositionEnumerator.MoveNext())
                {
                    board.AddPiece(piecePositionEnumerator.Current);
                    winner = engine.CalculateWinner(board);
                }
            }

            return winner == Player.None ? "Draw" : winner.ToString();
        }
    }

    public class Engine
    {
        private readonly List<WinnerCalculator> _winnerCalculators = new List<WinnerCalculator>();

        public Engine()
        {
            _winnerCalculators.Add(new WinnerCalculator(new IdentityTransformation())); // Win by Column
            _winnerCalculators.Add(new WinnerCalculator(new TransposedTransformation())); // Win by Row
            _winnerCalculators.Add(new WinnerCalculator(new AscendingDiagonalTransformation()));
            _winnerCalculators.Add(new WinnerCalculator(new DescendingDiagonalTransformation()));
        }

        public Player CalculateWinner(Board board)
        {
            var winner = Player.None;
            using (var winnerCalculator = _winnerCalculators.GetEnumerator())
            {
                while (winner == Player.None && winnerCalculator.MoveNext())
                {
                    winner = winnerCalculator.Current.CalculateWinner(board);
                }
            }

            return winner;
        }
    }

    public class WinnerCalculator
    {
        private readonly ICoordinateTransformation _coordinateTransformation;

        private const int SameColorPiecesRequiredToWin = 4;

        public WinnerCalculator(ICoordinateTransformation coordinateTransformation) =>
            _coordinateTransformation = coordinateTransformation;

        public Player CalculateWinner(Board board)
        {
            var winner = Player.None;
            using (var columns = GetGroupedPieces(board).GetEnumerator())
            {
                winner = CalculateWinnerByGroupedBoard(columns);
            }

            return winner;
        }

        private IEnumerable<IEnumerable<Player>> GetGroupedPieces(Board board)
        {
            for (var horizontalPosition = 0;
                horizontalPosition < _coordinateTransformation.HorizontalPositions;
                horizontalPosition++)
            {
                var maxVerticalPosition = _coordinateTransformation.MaximumVerticalPosition(horizontalPosition);
                var result = new List<Player>();
                for (var verticalPosition = 0; verticalPosition < maxVerticalPosition; verticalPosition++)
                {
                    var row = _coordinateTransformation.GetCartesianRow(horizontalPosition, verticalPosition);
                    var column = _coordinateTransformation.GetCartesianColumn(horizontalPosition, verticalPosition);
                    result.Add(board.GetPieceAt(row, column));
                }

                yield return result;
            }
        }

        private static Player CalculateWinnerByGroupedBoard(IEnumerator<IEnumerable<Player>> groupedBoard)
        {
            var winner = Player.None;
            while (winner == Player.None && groupedBoard.MoveNext())
            {
                winner = CalculateWinnerByGroup(groupedBoard.Current);
            }

            return winner;
        }

        private static Player CalculateWinnerByGroup(IEnumerable<Player> pieces)
        {
            var currentColor = Player.None;
            var count = 1;
            var winner = Player.None;

            foreach (var piece in pieces)
            {
                if (piece == currentColor)
                {
                    count++;
                }
                else
                {
                    currentColor = piece;
                    count = 1;
                }

                if (currentColor != Player.None && count == SameColorPiecesRequiredToWin)
                {
                    winner = currentColor;
                }
            }

            return winner;
        }
    }

    public interface ICoordinateTransformation
    {
        int HorizontalPositions { get; }

        int MaximumVerticalPosition(int horizontalPosition);
        int GetCartesianRow(int horizontalPosition, int verticalPosition);
        int GetCartesianColumn(int horizontalPosition, int verticalPosition);
    }

    public abstract class DiagonalCoordinateTransformation : ICoordinateTransformation
    {
        public int HorizontalPositions => 12;
        public int VerticalPositions => HorizontalPositions / 2 - 1;

        public virtual int MaximumVerticalPosition(int horizontalPosition) =>
            horizontalPosition <= VerticalPositions ? horizontalPosition + 1 : HorizontalPositions - horizontalPosition;

        public abstract int GetCartesianRow(int horizontalPosition, int verticalPosition);
        public abstract int GetCartesianColumn(int horizontalPosition, int verticalPosition);
    }

    public class AscendingDiagonalTransformation : DiagonalCoordinateTransformation
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

    public class DescendingDiagonalTransformation : DiagonalCoordinateTransformation
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

    public class TransposedTransformation : ICoordinateTransformation
    {
        public int HorizontalPositions => Board.Rows;
        private static int VerticalPositions => Board.Columns;

        public int MaximumVerticalPosition(int horizontalPosition) => VerticalPositions;

        public int GetCartesianRow(int horizontalPosition, int verticalPosition) => horizontalPosition;

        public int GetCartesianColumn(int horizontalPosition, int verticalPosition) => verticalPosition;
    }

    public class IdentityTransformation : ICoordinateTransformation
    {
        public int HorizontalPositions => Board.Columns;
        private static int VerticalPositions => Board.Rows;

        public int MaximumVerticalPosition(int horizontalPosition) => VerticalPositions;

        public int GetCartesianRow(int horizontalPosition, int verticalPosition) => verticalPosition;

        public int GetCartesianColumn(int horizontalPosition, int verticalPosition) => horizontalPosition;
    }

    public class Board
    {
        public const int Columns = 7;
        public const int Rows = 6;

        private readonly Player[,] _pieces = new Player[Rows, Columns];
        private readonly int[] _piecesPerColumn = new int[Columns];

        public Player GetPieceAt(int row, int column) => _pieces[row, column];

        public void AddPiece(string piecePosition)
        {
            var column = ParsePieceColumn(piecePosition);
            var player = ParsePlayer(piecePosition);
            AddPieceToColumn(column, player);
        }

        private static Player ParsePlayer(string piecePosition)
        {
            var colorString = piecePosition.Substring(2);
            var piece = colorString == "Red" ? Player.Red : Player.Yellow;
            return piece;
        }

        private static int ParsePieceColumn(string piecePosition) => piecePosition[0] - 'A';

        private void AddPieceToColumn(int column, Player player)
        {
            var numberOfPiecesInColumn = _piecesPerColumn[column];

            _pieces[numberOfPiecesInColumn, column] = player;
            _piecesPerColumn[column]++;
        }
    }

    public enum Player
    {
        None = 0,
        Red,
        Yellow,
    }
}