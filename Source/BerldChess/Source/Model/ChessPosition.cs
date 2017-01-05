namespace BerldChess.Model
{
    public class ChessPosition
    {
        public string FEN { get; }
        public string PreviousMove { get; }

        public ChessPosition(string FEN, string previousMove = null)
        {
            this.FEN = FEN;
            PreviousMove = previousMove;
        }
    }
}
