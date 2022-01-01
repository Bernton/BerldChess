using System;
using System.Linq;
using System.Collections.ObjectModel;

namespace ChessDotNet.Pieces
{
    public class Queen : ChessPiece
    {
        public Queen(ChessPlayer owner)
        {
            Player = owner;
        }

        public override char GetFENLetter()
        {
            return Player == ChessPlayer.White ? 'Q' : 'q';
        }

        public override bool IsLegalMove(Move move, ChessGame game)
        {
            ChessUtility.ThrowIfNull(move, "move");
            ChessUtility.ThrowIfNull(game, "game");
            return new Bishop(Player).IsLegalMove(move, game) || new Rook(Player).IsLegalMove(move, game);
        }

        public override ReadOnlyCollection<Move> GetLegalMoves(BoardPosition from, bool returnIfAny, ChessGame game, Func<Move, bool> gameMoveValidator)
        {
            ChessUtility.ThrowIfNull(from, "from");
            ReadOnlyCollection<Move> horizontalVerticalMoves = new Rook(Player).GetLegalMoves(from, returnIfAny, game, gameMoveValidator);
            if (returnIfAny && horizontalVerticalMoves.Count > 0)
                return horizontalVerticalMoves;
            ReadOnlyCollection<Move> diagonalMoves = new Bishop(Player).GetLegalMoves(from, returnIfAny, game, gameMoveValidator);
            return new ReadOnlyCollection<Move>(horizontalVerticalMoves.Concat(diagonalMoves).ToList());
        }
    }
}
