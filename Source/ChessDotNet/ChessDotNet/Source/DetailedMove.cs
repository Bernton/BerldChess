﻿namespace ChessDotNet
{
    public class DetailedMove : Move
    {
        public ChessPiece Piece
        {
            get;
            private set;
        }

        public bool IsCapture
        {
            get;
            private set;
        }

        public CastlingType Castling
        {
            get;
            private set;
        }

        public DetailedMove(BoardPosition originalPosition, BoardPosition newPosition, ChessPlayer player, char? promotion, ChessPiece piece, bool isCapture, CastlingType castling) : 
            base(originalPosition, newPosition, player, promotion)
        {
            Piece = piece;
            IsCapture = isCapture;
            Castling = castling;
        }

        public DetailedMove(Move move, ChessPiece piece, bool isCapture, CastlingType castling)
            : this(move.OriginalPosition, move.NewPosition, move.Player, move.Promotion, piece, isCapture, castling)
        {
        }


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            DetailedMove move = (DetailedMove)obj;
            return OriginalPosition.Equals(move.OriginalPosition)
                && NewPosition.Equals(move.NewPosition)
                && Player == move.Player
                && Promotion == move.Promotion
                && Piece == move.Piece
                && IsCapture == move.IsCapture
                && Castling == move.Castling;
        }

        public override int GetHashCode()
        {
            return new { OriginalPosition, NewPosition, Player, Promotion, Piece, IsCapture, Castling }.GetHashCode();
        }

        public static bool operator ==(DetailedMove move1, DetailedMove move2)
        {
            if (ReferenceEquals(move1, move2))
                return true;
            if ((object)move1 == null || (object)move2 == null)
                return false;
            return move1.Equals(move2);
        }

        public static bool operator !=(DetailedMove move1, DetailedMove move2)
        {
            if (ReferenceEquals(move1, move2))
                return false;
            if ((object)move1 == null || (object)move2 == null)
                return true;
            return !move1.Equals(move2);
        }


    }
}
