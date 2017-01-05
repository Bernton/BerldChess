﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ChessDotNet.Pieces
{
    public class Bishop : ChessPiece
    {
        public Bishop(ChessPlayer owner)
        {
            Owner = owner;
        }

        public override char GetFENLetter()
        {
            return Owner == ChessPlayer.White ? 'B' : 'b';
        }

        public override bool IsLegalMove(Move move, ChessGame game)
        {
            ChessUtility.ThrowIfNull(move, "move");
            ChessUtility.ThrowIfNull(game, "game");
            BoardPosition origin = move.OriginalPosition;
            BoardPosition destination = move.NewPosition;

            BoardDistance posDelta = new BoardDistance(origin, destination);
            if (posDelta.X != posDelta.Y)
                return false;
            bool increasingRank = destination.Rank > origin.Rank;
            bool increasingFile = (int)destination.File > (int)origin.File;
            for (int f = (int)origin.File + (increasingFile ? 1 : -1), r = origin.Rank + (increasingRank ? 1 : -1);
                 increasingFile ? f < (int)destination.File : f > (int)destination.File;
                 f += increasingFile ? 1 : -1, r += increasingRank ? 1 : -1)
            {
                if (game.GetPieceAt((ChessFile)f, r) != null)
                {
                    return false;
                }
            }
            return true;
        }

        public override ReadOnlyCollection<Move> GetLegalMoves(BoardPosition from, bool returnIfAny, ChessGame game, Func<Move, bool> gameMoveValidator)
        {
            List<Move> validMoves = new List<Move>();
            ChessPiece piece = game.GetPieceAt(from);
            int l0 = game.BoardHeight;
            int l1 = game.BoardWidth;
            for (int i = -7; i < 8; i++)
            {
                if (i == 0)
                    continue;
                if (from.Rank + i > 0 && from.Rank + i <= l0
                    && (int)from.File + i > -1 && (int)from.File + i < l1)
                {
                    Move move = new Move(from, new BoardPosition(from.File + i, from.Rank + i), piece.Owner);
                    if (gameMoveValidator(move))
                    {
                        validMoves.Add(move);
                        if (returnIfAny)
                            return new ReadOnlyCollection<Move>(validMoves);
                    }
                }
                if (from.Rank - i > 0 && from.Rank - i <= l0
                    && (int)from.File + i > -1 && (int)from.File + i < l1)
                {
                    Move move = new Move(from, new BoardPosition(from.File + i, from.Rank - i), piece.Owner);
                    if (gameMoveValidator(move))
                    {
                        validMoves.Add(move);
                        if (returnIfAny)
                            return new ReadOnlyCollection<Move>(validMoves);
                    }
                }
            }
            return new ReadOnlyCollection<Move>(validMoves);
        }
    }
}
