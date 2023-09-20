using ChessDotNet.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChessDotNet
{
    public class PgnReader<TGame> where TGame : ChessGame, new()
    {
        public TGame Game
        {
            get;
            private set;
        }

        public PgnReader()
        {
            Game = new TGame();
        }

        public void ReadPgnFromString(string pgn)
        {
            string pgnWithoutComments = Regex.Replace(pgn, @"\{[^}]*\}", "");
            int t;
            List<string> moves = pgnWithoutComments.Split(new char[] { '.', ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                           .Where(x => !(int.TryParse(x, out t) || x[0] == '$')).ToList();
            if (new string[] { "*", "1-0", "0-1", "1/2-1/2" }.Contains(moves[moves.Count - 1]))
            {
                moves.RemoveAt(moves.Count - 1);
            }
            TGame game = new TGame();
            int ply = 0;
            foreach (string _ in moves)
            {
                ply++;
                ChessPlayer player = ply % 2 == 0 ? ChessPlayer.Black : ChessPlayer.White;
                string move = _.TrimEnd('#', '?', '!', '+').Trim();

                BoardPosition origin = null;
                BoardPosition destination = null;
                ChessPiece piece = null;
                char? promotion = null;

                if (move.Length > 2)
                {
                    string possiblePromotionPiece = move.Substring(move.Length - 2).ToUpperInvariant();
                    if (possiblePromotionPiece[0] == '=')
                    {
                        promotion = possiblePromotionPiece[1];
                        move = move.Remove(move.Length - 2, 2);
                    }
                }

                if (move.ToUpperInvariant() == "O-O")
                {
                    int r = player == ChessPlayer.White ? 1 : 8;
                    origin = new BoardPosition(ChessFile.E, r);
                    destination = new BoardPosition(ChessFile.G, r);
                    piece = new King(player);
                }
                else if (move.ToUpperInvariant() == "O-O-O")
                {
                    int r = player == ChessPlayer.White ? 1 : 8;
                    origin = new BoardPosition(ChessFile.E, r);
                    destination = new BoardPosition(ChessFile.C, r);
                    piece = new King(player);
                }

                if (piece == null)
                {
                    piece = game.MapPgnCharToPiece(move[0], player);
                }
                if (!(piece is Pawn))
                {
                    move = move.Remove(0, 1);
                }

                int rankRestriction = -1;
                ChessFile fileRestriction = ChessFile.None;
                if (destination == null)
                {
                    if (move[0] == 'x')
                    {
                        move = move.Remove(0, 1);
                    }
                    else if (move.Length == 4 && move[1] == 'x')
                    {
                        move = move.Remove(1, 1);
                    }

                    if (move.Length == 2)
                    {
                        destination = new BoardPosition(move);
                    }
                    else if (move.Length == 3)
                    {
                        if (char.IsDigit(move[0]))
                        {
                            rankRestriction = int.Parse(move[0].ToString());
                        }
                        else
                        {
                            bool recognized = Enum.TryParse<ChessFile>(move[0].ToString(), true, out fileRestriction);
                            if (!recognized)
                            {
                                throw new PgnException("Invalid PGN: unrecognized origin file.");
                            }
                        }
                        destination = new BoardPosition(move.Remove(0, 1));
                    }
                    else if (move.Length == 4)
                    {
                        origin = new BoardPosition(move.Substring(0, 2));
                        destination = new BoardPosition(move.Substring(2, 2));
                    }
                    else
                    {
                        throw new PgnException("Invalid PGN.");
                    }
                }

                if (origin != null)
                {
                    Move m = new Move(origin, destination, player, promotion);
                    if (game.IsValidMove(m))
                    {
                        game.ApplyMove(m, true);
                    }
                    else
                    {
                        throw new PgnException("Invalid PGN: contains invalid moves.");
                    }
                }
                else
                {
                    ChessPiece[][] board = game.GetBoard();
                    List<Move> validMoves = new List<Move>();
                    for (int r = 0; r < game.BoardHeight; r++)
                    {
                        if (rankRestriction != -1 && r != 8 - rankRestriction) continue;
                        for (int f = 0; f < game.BoardWidth; f++)
                        {
                            if (fileRestriction != ChessFile.None && f != (int)fileRestriction) continue;
                            if (board[r][f] != piece) continue;
                            Move m = new Move(new BoardPosition((ChessFile)f, 8 - r), destination, player, promotion);
                            if (game.IsValidMove(m))
                            {
                                validMoves.Add(m);
                            }
                        }
                    }
                    if (validMoves.Count == 0) throw new PgnException("Invalid PGN: contains invalid moves.");
                    if (validMoves.Count > 1) throw new PgnException("Invalid PGN: contains ambiguous moves.");
                    game.ApplyMove(validMoves[0], true);
                }
            }
            Game = game;
        }
    }
}