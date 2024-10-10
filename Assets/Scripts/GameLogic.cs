using System;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    None,
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}

public enum PieceColor
{
    None,
    White,
    Black
}

public struct ChessPosition
{
    public int Row { get; set; }
    public int Column { get; set; }

    public ChessPosition(int row, int column)
    {
        Row = row;
        Column = column;
    }
}

public struct Piece
{
    public PieceType Type { get; }
    public PieceColor Color { get; }

    public Piece(PieceType type, PieceColor color)
    {
        Type = type;
        Color = color;
    }
}

public class ChessLogic
{
    private static ChessLogic? _instance;
    private readonly Piece[,] _board = new Piece[8, 8];
    private bool _isWhite = true;

    private ChessLogic()
    {
        InitializeBoard();
    }

    public static ChessLogic GetInstance()
    {
        return _instance ??= new ChessLogic();
    }

    public void ClearBoard()
    {
        InitializeBoard();
        _isWhite = true;
    }


    // Initializes the board with default positions
    private void InitializeBoard()
    {
        // Clear the board
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                _board[i, j] = new Piece(PieceType.None, PieceColor.None);
            }
        }

        // Set up pawns
        for (int i = 0; i < 8; i++)
        {
            _board[1, i] = new Piece(PieceType.Pawn, PieceColor.White);
            _board[6, i] = new Piece(PieceType.Pawn, PieceColor.Black);
        }

        // Other pieces for white
        _board[0, 0] = new Piece(PieceType.Rook, PieceColor.White);
        _board[0, 7] = new Piece(PieceType.Rook, PieceColor.White);
        _board[0, 1] = new Piece(PieceType.Knight, PieceColor.White);
        _board[0, 6] = new Piece(PieceType.Knight, PieceColor.White);
        _board[0, 2] = new Piece(PieceType.Bishop, PieceColor.White);
        _board[0, 5] = new Piece(PieceType.Bishop, PieceColor.White);
        _board[0, 3] = new Piece(PieceType.Queen, PieceColor.White);
        _board[0, 4] = new Piece(PieceType.King, PieceColor.White);

        // Other pieces for black
        _board[7, 0] = new Piece(PieceType.Rook, PieceColor.Black);
        _board[7, 7] = new Piece(PieceType.Rook, PieceColor.Black);
        _board[7, 1] = new Piece(PieceType.Knight, PieceColor.Black);
        _board[7, 6] = new Piece(PieceType.Knight, PieceColor.Black);
        _board[7, 2] = new Piece(PieceType.Bishop, PieceColor.Black);
        _board[7, 5] = new Piece(PieceType.Bishop, PieceColor.Black);
        _board[7, 3] = new Piece(PieceType.Queen, PieceColor.Black);
        _board[7, 4] = new Piece(PieceType.King, PieceColor.Black);
    }

    // Get turn
    public bool IsWhite()
    {
        return _isWhite;
    }

    // Change turn
    public void ChangeTurn()
    {
        _isWhite = !_isWhite;
    }

    // Setter and getter of board
    public void SetBoard(Piece[,] board)
    {
        for (int row = 0; row < 8; row++)
        for (int column = 0; column < 8; column++)
            _board[row, column] = board[row, column];
    }

    public Piece[,] GetBoard()
    {
        var board = new Piece[8, 8];
        for (int row = 0; row < 8; row++)
        for (int column = 0; column < 8; column++)
            board[row, column] = _board[row, column];
        return board;
    }

    public Piece GetPiece(ChessPosition pos)
    {
        return _board[pos.Row, pos.Column];
    }


    // Check if a move is valid
    public bool IsMoveValid(ChessPosition from, ChessPosition to)
    {
        int startX = from.Row, startY = from.Column;
        int endX = to.Row, endY = to.Column;

        var piece = _board[startX, startY];

        if (piece.Type == PieceType.None || startX < 0 || startX > 7 || startY < 0 || startY > 7 ||
            endX < 0 || endX > 7 || endY < 0 || endY > 7)
        {
            return false;
        }

        var destination = _board[endX, endY];
        if (destination.Color == piece.Color)
            return false;
        if (piece.Color == PieceColor.White && !_isWhite)
            return false;
        if (piece.Color == PieceColor.Black && _isWhite)
            return false;

        // Check based on the piece type
        switch (piece.Type)
        {
            case PieceType.Pawn:
                return IsValidPawnMove(piece.Color, startX, startY, endX, endY);
            case PieceType.Rook:
                return IsValidRookMove(piece.Color, startX, startY, endX, endY);
            case PieceType.Knight:
                return IsValidKnightMove(piece.Color, startX, startY, endX, endY);
            case PieceType.Bishop:
                return IsValidBishop(piece.Color, startX, startY, endX, endY);
            case PieceType.Queen:
                return IsValidQueen(piece.Color, startX, startY, endX, endY);
            case PieceType.King:
                return IsValidKing(piece.Color, startX, startY, endX, endY);
            default:
                return false;
        }
    }

    // Get all possible moves for a piece at a position
    public List<ChessPosition> GetPossibleMoves(ChessPosition pos)
    {
        PrintBoard();
        Debug.Log(pos.Row + ", " + pos.Column);
        var x = pos.Row;
        var y = pos.Column;
        var moves = new List<(int, int)>();
        var piece = _board[x, y];

        // Check possible moves based on the piece type
        switch (piece.Type)
        {
            case PieceType.Pawn:
                AddPawnMoves(piece.Color, x, y, moves);
                break;
            case PieceType.Rook:
                AddRookMoves(piece.Color, x, y, moves);
                break;
            case PieceType.Knight:
                AddKnightMoves(piece.Color, x, y, moves);
                break;
            case PieceType.Bishop:
                AddBishopMoves(piece.Color, x, y, moves);
                break;
            case PieceType.Queen:
                AddQueenMoves(piece.Color, x, y, moves);
                break;
            case PieceType.King:
                AddKingMoves(piece.Color, x, y, moves);
                break;
        }

        List<ChessPosition> movesInType = new List<ChessPosition>();
        for (var i = 0; i < moves.Count; i++)
        {
            movesInType.Add(new ChessPosition(moves[i].Item1, moves[i].Item2));
        }

        return movesInType;
    }


    // Move a piece
    public void MovePiece(ChessPosition from, ChessPosition to)
    {
        int startX = from.Row, startY = from.Column;
        int endX = to.Row, endY = to.Column;
        // Move piece if valid
        if (IsMoveValid(new ChessPosition(startX, startY), new ChessPosition(endX, endY)))
        {
            _board[endX, endY] = _board[startX, startY];
            _board[startX, startY] = new Piece(PieceType.None, PieceColor.None);
        }
    }

    // Remove a piece
    public void RemovePiece(ChessPosition pos)
    {
        if (!(pos.Row < 0 || pos.Row > 7 || pos.Column < 0 || pos.Column > 7))
            _board[pos.Row, pos.Column] = new Piece(PieceType.None, PieceColor.None);
    }


    // Is Game Over?
    //  0 -> continue game.
    //  1 -> white is winner.
    // -1 -> black is winner
    public int IsGameOver()
    {
        bool whiteKingFounded = false, blackKingFounded = false;
        for (var row = 0; row < 8; row++)
        {
            for (var column = 0; column < 8; column++)
            {
                if (_board[row, column].Color == PieceColor.White && _board[row, column].Type == PieceType.King)
                    whiteKingFounded = true;
                if (_board[row, column].Color == PieceColor.Black && _board[row, column].Type == PieceType.King)
                    blackKingFounded = true;
            }
        }

        if (!blackKingFounded)
            return 1;
        if (!whiteKingFounded)
            return -1;
        return 0;
    }


    // Helper methods for specific piece movements
    private bool IsValidPawnMove(PieceColor color, int startX, int startY, int endX, int endY)
    {
        var direction = color == PieceColor.White ? 1 : -1;
        if (_board[endX, endY].Type == PieceType.None)
        {
            if (startX + direction == endX && startY == endY)
                return true;
            if (startX + 2 * direction == endX && startY == endY &&
                _board[startX + direction, startY].Type == PieceType.None)
            {
                if (color == PieceColor.White && startX == 1)
                    return true;
                if (color == PieceColor.Black && startX == 6)
                    return true;
            }
        }
        else if (startX + direction == endX && Math.Abs(startY - endY) == 1)
            return true;

        return false;
    }

    private bool IsValidRookMove(PieceColor color, int startX, int startY, int endX, int endY)
    {
        // Check if the move is either in the same row or the same column
        if (startX != endX && startY != endY)
            return false; // Rook moves either horizontally or vertically


        // Check if there are any pieces between the start and end points
        if (startX == endX) // Vertical movement
        {
            int direction = startY < endY ? 1 : -1;
            for (int y = startY + direction; y != endY; y += direction)
                if (_board[startX, y].Type != PieceType.None) // If a piece exists between the points
                    return false;
        }
        else if (startY == endY) // Horizontal movement
        {
            int direction = startX < endX ? 1 : -1;
            for (int x = startX + direction; x != endX; x += direction)
                if (_board[x, startY].Type != PieceType.None) // If a piece exists between the points

                    return false;
        }

        // If no pieces are in the way, and it's a valid rook move, return true
        return true;
    }

    private bool IsValidKnightMove(PieceColor color, int startX, int startY, int endX, int endY)
    {
        return Math.Abs(startX - endX) == 2 && Math.Abs(startY - endY) == 1 ||
               Math.Abs(startX - endX) == 1 && Math.Abs(startY - endY) == 2;
    }

    private bool IsValidKing(PieceColor color, int startX, int startY, int endX, int endY)
    {
        int xDiff = Math.Abs(endX - startX);
        int yDiff = Math.Abs(endY - startY);
        if (xDiff <= 1 && yDiff <= 1)
            return true;
        return false;
    }

    private bool IsValidQueen(PieceColor color, int startX, int startY, int endX, int endY)
    {
        return IsValidBishop(color, startX, startY, endX, endY) || IsValidRookMove(color, startX, startY, endX, endY);
    }

    private bool IsValidBishop(PieceColor color, int startX, int startY, int endX, int endY)
    {
        // Check if the move is diagonal: absolute difference of x and y must be equal
        if (Math.Abs(endX - startX) != Math.Abs(endY - startY))
        {
            return false; // Not a valid diagonal move
        }

        // Determine the direction of movement for both x and y (positive or negative)
        int xDirection = (endX > startX) ? 1 : -1;
        int yDirection = (endY > startY) ? 1 : -1;

        // Check for any pieces in the way along the diagonal path
        int x = startX + xDirection;
        int y = startY + yDirection;
        while (x != endX && y != endY)
        {
            if (_board[x, y].Type != PieceType.None) // If a piece exists between the points
                return false;
            x += xDirection;
            y += yDirection;
        }

        // If no pieces are in the way, and it's a valid bishop move, return true
        return true;
    }


    private void AddPawnMoves(PieceColor color, int x, int y, List<(int, int)> moves)
    {
        int direction = color == PieceColor.White ? 1 : -1;
        if (IsMoveValid(new ChessPosition(x, y), new ChessPosition(x + direction, y)))
            moves.Add((x + direction, y));
        if (IsMoveValid(new ChessPosition(x, y), new ChessPosition(x + direction, y + 1)))
            moves.Add((x + direction, y + 1));
        if (IsMoveValid(new ChessPosition(x, y), new ChessPosition(x + direction, y - 1)))
            moves.Add((x + direction, y - 1));
        direction = color == PieceColor.White ? 2 : -2;
        if (IsMoveValid(new ChessPosition(x, y), new ChessPosition(x + direction, y)))
            moves.Add((x + direction, y));
    }

    private void AddRookMoves(PieceColor color, int x, int y, List<(int, int)> moves)
    {
        for (int i = 0; i < 8; i++)
        {
            if (IsMoveValid(new ChessPosition(x, y), new ChessPosition(x, i))) moves.Add((x, i));
            if (IsMoveValid(new ChessPosition(x, y), new ChessPosition(i, y))) moves.Add((i, y));
        }
    }

    private void AddKnightMoves(PieceColor color, int x, int y, List<(int, int)> moves)
    {
        int[] dx = {2, 2, -2, -2, 1, 1, -1, -1};
        int[] dy = {1, -1, 1, -1, 2, -2, 2, -2};

        for (int i = 0; i < 8; i++)
        {
            if (IsMoveValid(new ChessPosition(x, y), new ChessPosition(x + dx[i], y + dy[i])))
                moves.Add((x + dx[i], y + dy[i]));
        }
    }

    private void AddKingMoves(PieceColor color, int x, int y, List<(int, int)> moves)
    {
        int[] dx = {1, 1, 1, 0, 0, -1, -1, -1};
        int[] dy = {1, 0, -1, 1, -1, 1, 0, -1};
        for (int i = 0; i < 8; i++)
        {
            if (IsMoveValid(new ChessPosition(x, y), new ChessPosition(x + dx[i], y + dy[i])))
                moves.Add((x + dx[i], y + dy[i]));
        }
    }

    private void AddQueenMoves(PieceColor color, int x, int y, List<(int, int)> moves)
    {
        AddRookMoves(color, x, y, moves);
        AddBishopMoves(color, x, y, moves);
    }

    private void AddBishopMoves(PieceColor color, int x, int y, List<(int, int)> moves)
    {
        for (int i = -8; i < 8; i++)
        {
            if (IsMoveValid(new ChessPosition(x, y), new ChessPosition(x + i, y + i)))
                moves.Add((x + i, y + i));
            if (IsMoveValid(new ChessPosition(x, y), new ChessPosition(x + i, y - i)))
                moves.Add((x + i, y - i));
        }
    }


    public void PrintBoard()
    {
        string s = "";
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        s += ("* 0 1 2 3 4 5 6 7 \n");
        for (var i = 0; i < 8; i++)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            s += (i + " ");
            for (var j = 0; j < 8; j++)
            {
                var piece = _board[i, j];
                if (piece.Color == PieceColor.White)
                    Console.ForegroundColor = ConsoleColor.White;
                else if (piece.Color == PieceColor.Black)
                    Console.ForegroundColor = ConsoleColor.Black;
                else
                    Console.ForegroundColor = ConsoleColor.Green;

                s += (GetUnicodePiece(_board[i, j]) + " ");
            }

            s += "\n";
        }

        Console.ForegroundColor = color;
        Debug.Log(s);
    }

    // Method to return Unicode symbol based on the piece
    private static string GetUnicodePiece(Piece piece)
    {
        switch (piece.Color)
        {
            case PieceColor.White:
                switch (piece.Type)
                {
                    case PieceType.King: return "\u2654"; // ♔
                    case PieceType.Queen: return "\u2655"; // ♕
                    case PieceType.Rook: return "\u2656"; // ♖
                    case PieceType.Bishop: return "\u2657"; // ♗
                    case PieceType.Knight: return "\u2658"; // ♘
                    case PieceType.Pawn: return "\u2659"; // ♙
                    default: return " "; // Empty
                }

                break;
            case PieceColor.Black:
                switch (piece.Type)
                {
                    case PieceType.King: return "\u265A"; // ♚
                    case PieceType.Queen: return "\u265B"; // ♛
                    case PieceType.Rook: return "\u265C"; // ♜
                    case PieceType.Bishop: return "\u265D"; // ♝
                    case PieceType.Knight: return "\u265E"; // ♞
                    case PieceType.Pawn: return "\u265F"; // ♟️
                }

                break;
            default:
                return ".";
        }

        return "";
    }
}