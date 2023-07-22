using ChessChallenge.API;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    static int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
    static Random rng = new();
    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        Move bestMove = moves[0];
        int bestScore = Evaluate(board, bestMove);
        for (int i = 0; i < moves.Length; i++)
        {
            int score = Evaluate(board, moves[i]);
            Console.WriteLine(score);
            if (score >= bestScore) {
                bestMove = moves[i];
                bestScore = score;
            }
        }
        return bestMove;
    }

    public static int Evaluate(Board board, Move move)
    {
        return 
            10 * Captures(board, move, 5) +
            15 * Forks(board, move, 2) +
            // Mobility(board, move) +
            Killer(board, move, 2);
    }

    public static int Mobility(Board board, Move move)
    {
        board.MakeMove(move);
        Move[] moves = board.GetLegalMoves();
        Move oppMove = moves[rng.Next(moves.Length)];
        int oppScore = moves.Length;
        board.MakeMove(oppMove);
        int myScore = board.GetLegalMoves().Length;
        board.UndoMove(oppMove);
        board.UndoMove(move);
        return myScore - oppScore;
    }

    public static int Captures(Board board, Move move, int depth = 1) {
        if (depth == 0) return 0;
        int myScore = pieceValues[(int)board.GetPiece(move.TargetSquare).PieceType];
        board.MakeMove(move);
        int oppScore = 0;
        foreach (var oppMove in board.GetLegalMoves(true)) {
            oppScore = Math.Max(oppScore, Captures(board, oppMove, depth - 1));
        }
        board.UndoMove(move);
        return myScore - oppScore;
    }

    public static int Forks(Board board, Move move, int depth = 1)
    {
        if (depth == 0) return 0;
        int capturables = board.GetLegalMoves(true).Length;
        board.MakeMove(move);
        int myScore = 0;
        int oppScore = 0;
        foreach (var oppMove in board.GetLegalMoves()) {
            board.MakeMove(oppMove);
            myScore = Math.Max(myScore, capturables - board.GetLegalMoves(true).Length);
            board.UndoMove(oppMove);
            oppScore = Math.Max(oppScore, Forks(board, oppMove, depth - 1));
        }
        board.UndoMove(move);
        return myScore - oppScore;
    }

    public static int Check(Board board, Move move, int depth = 1) 
    {
        if (depth == 0) return 0;
        board.MakeMove(move);
        int myScore = board.IsInCheck() ? 100 : 0;
        Move[] oppMoves = board.GetLegalMoves();
        int oppScore = 0;
        foreach (var oppMove in oppMoves) 
        {
            oppScore = Math.Max(oppScore, Check(board, oppMove, depth - 1));
        }
        board.UndoMove(move);
        return myScore - oppScore;
    }

    public static int Killer(Board board, Move move, int depth = 1) {
        if (depth == 0) return 0;
        board.MakeMove(move);
        int myScore = board.IsInCheckmate() ? 10000 : 0;
        int oppScore = 0;
        foreach (var oppMove in board.GetLegalMoves()) 
        {
            oppScore = Math.Max(oppScore, Killer(board, oppMove, depth - 1));
        }
        board.UndoMove(move);
        return myScore - oppScore;
    }


}