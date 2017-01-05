﻿namespace ChessDotNet
{
    public class Move
    {
        public BoardPosition OriginalPosition
        {
            get;
            private set;
        }

        public BoardPosition NewPosition
        {
            get;
            private set;
        }

        public ChessPlayer Player
        {
            get;
            private set;
        }

        public char? Promotion
        {
            get;
            private set;
        }

        public Move(BoardPosition originalPosition, BoardPosition newPosition, ChessPlayer player)
            : this(originalPosition, newPosition, player, null)
        { }

        public Move(string originalPosition, string newPosition, ChessPlayer player)
            : this(originalPosition, newPosition, player, null)
        { }

        public Move(BoardPosition originalPosition, BoardPosition newPosition, ChessPlayer player, char? promotion)
        {
            OriginalPosition = originalPosition;
            NewPosition = newPosition;
            Player = player;
            if (promotion.HasValue)
            {
                Promotion = char.ToUpper(promotion.Value);
            }
            else
            {
                Promotion = null;
            }
        }

        public Move(string originalPosition, string newPosition, ChessPlayer player, char? promotion)
        {
            OriginalPosition = new BoardPosition(originalPosition);
            NewPosition = new BoardPosition(newPosition);
            Player = player;
            if (promotion.HasValue)
            {
                Promotion = char.ToUpper(promotion.Value);
            }
            else
            {
                Promotion = null;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            Move move1 = this;
            Move move2 = (Move)obj;
            return move1.OriginalPosition.Equals(move2.OriginalPosition)
                && move1.NewPosition.Equals(move2.NewPosition)
                && move1.Player == move2.Player
                && move1.Promotion == move2.Promotion;
        }

        public override int GetHashCode()
        {
            return new { OriginalPosition, NewPosition, Player, Promotion }.GetHashCode();
        }

        public static bool operator ==(Move move1, Move move2)
        {
            if (ReferenceEquals(move1, move2))
                return true;
            if ((object)move1 == null || (object)move2 == null)
                return false;
            return move1.Equals(move2);
        }

        public static bool operator !=(Move move1, Move move2)
        {
            if (ReferenceEquals(move1, move2))
                return false;
            if ((object)move1 == null || (object)move2 == null)
                return true;
            return !move1.Equals(move2);
        }

        public override string ToString()
        {
            return OriginalPosition.ToString() + "-" + NewPosition.ToString();
        }

        public string ToString(string seperation)
        {
            return OriginalPosition.ToString() + seperation + NewPosition.ToString();
        }
    }
}
