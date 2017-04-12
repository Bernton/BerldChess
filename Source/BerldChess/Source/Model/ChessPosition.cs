namespace BerldChess.Model
{
    public class ChessPly
    {
        public string FEN { get; }
        public string Move { get; }
        public double Evaluation { get; set; }
        public int EvaluationDepth { get; set; }

        public ChessPly(string FEN, double evaluation = 0.00, string move = null)
        {
            this.FEN = FEN;
            Move = move;
            Evaluation = evaluation;
            EvaluationDepth = 0;
        }
    }
}
