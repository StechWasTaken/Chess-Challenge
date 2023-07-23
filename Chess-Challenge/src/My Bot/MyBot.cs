using ChessChallenge.API;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

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
        for (int i = 1; i < moves.Length; i++)
        {
            int score = Evaluate(board, moves[i]);
            if (score >= bestScore) {
                bestMove = moves[i];
                bestScore = score;
            }
        }
        return bestMove;
    }

    public static int Evaluate(Board board, Move move)
    {
        return (Captures(board, move, 10) + Killer(board, move)) * board.PlyCount;
    }

    public static int Captures(Board board, Move move, int depth = 2, int alpha = int.MaxValue, int beta = int.MinValue) {
        if (depth == 0) return 0;
        int myScore = pieceValues[(int)board.GetPiece(move.TargetSquare).PieceType];
        board.MakeMove(move);
        int oppScore = 0;
        foreach (var oppMove in board.GetLegalMoves(true)) 
        {
            oppScore = Math.Max(oppScore, Captures(board, oppMove, depth - 1, beta, alpha));
            alpha = Math.Max(oppScore, alpha);
            if (alpha >= beta) break;
        }
        board.UndoMove(move);
        return myScore - oppScore;
    }

    public static int Killer(Board board, Move move, int depth = 2) {
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