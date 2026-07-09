using System.Collections.Generic;

namespace ChessEngineInterface
{
    /// <summary>
    /// A parsed UCI "info ... pv ..." line: one principal variation with its search statistics.
    /// </summary>
    public sealed class Evaluation
    {
        /// <summary>1-based index of this variation (1 for the best line, or in MultiPV mode).</summary>
        public int MultiPv { get; }

        /// <summary>Search depth in plies.</summary>
        public int Depth { get; }

        /// <summary>Selective search depth in plies (0 if the engine did not report it).</summary>
        public int SelDepth { get; }

        /// <summary>The evaluation score from the side-to-move's point of view.</summary>
        public Score Score { get; }

        /// <summary>Number of nodes searched.</summary>
        public long Nodes { get; }

        /// <summary>Nodes per second.</summary>
        public long Nps { get; }

        /// <summary>Time searched, in milliseconds.</summary>
        public int Time { get; }

        /// <summary>Endgame tablebase hits.</summary>
        public long TbHits { get; }

        /// <summary>The principal variation as moves in long algebraic notation (e.g. "e2e4").</summary>
        public IReadOnlyList<string> PrincipalVariation { get; }

        internal Evaluation(int multiPv, int depth, int selDepth, Score score,
                            long nodes, long nps, int time, long tbHits, IReadOnlyList<string> principalVariation)
        {
            MultiPv = multiPv;
            Depth = depth;
            SelDepth = selDepth;
            Score = score;
            Nodes = nodes;
            Nps = nps;
            Time = time;
            TbHits = tbHits;
            PrincipalVariation = principalVariation;
        }
    }
}
