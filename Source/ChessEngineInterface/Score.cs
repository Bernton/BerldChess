namespace ChessEngineInterface
{
    /// <summary>
    /// A UCI evaluation score from the side-to-move's point of view.
    /// </summary>
    public readonly struct Score
    {
        /// <summary>True if this is a forced-mate score rather than a centipawn score.</summary>
        public bool IsMate { get; }

        /// <summary>
        /// Centipawns when <see cref="IsMate"/> is false; otherwise the signed number of moves
        /// until mate (positive: the side to move gives mate, negative: it gets mated).
        /// </summary>
        public int Value { get; }

        internal Score(int value, bool isMate)
        {
            Value = value;
            IsMate = isMate;
        }
    }
}
