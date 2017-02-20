namespace BerldChess.Model
{
    public class ChessPosition
    {
        public string FEN { get; }
        public string Move { get; }
        public double Evaluation { get; }

        public ChessPosition(string FEN, double evaluation = 0.00, string move = null)
        {
            this.FEN = FEN;
            Move = move;
            Evaluation = evaluation;
        }
    }
}
