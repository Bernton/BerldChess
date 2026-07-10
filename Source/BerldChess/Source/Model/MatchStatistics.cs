using System;
using System.Collections.Generic;

namespace BerldChess.Model
{
    /// <summary>The outcome of a single competitive game, from engine 1's point of view.</summary>
    public enum MatchOutcome
    {
        Win,
        Draw,
        Loss
    }

    /// <summary>
    /// Running statistics for an engine-vs-engine match, from engine 1's point of view.
    /// Records each result and derives score, Elo difference, its 95% error margin, and the
    /// likelihood that engine 1 is the stronger engine (LOS). Pure logic, no UI.
    /// </summary>
    public class MatchStatistics
    {
        private const double Z95 = 1.959964; // 95% two-sided normal quantile

        private readonly List<MatchOutcome> _results = new List<MatchOutcome>();

        public int Wins { get; private set; }
        public int Draws { get; private set; }
        public int Losses { get; private set; }

        public int Games => Wins + Draws + Losses;

        /// <summary>Match points for engine 1: a win is 1, a draw is ½.</summary>
        public double Score => Wins + 0.5 * Draws;

        /// <summary>The results in the order they were played (engine 1's point of view).</summary>
        public IReadOnlyList<MatchOutcome> Results => _results;

        public void AddResult(MatchOutcome outcome)
        {
            _results.Add(outcome);

            switch (outcome)
            {
                case MatchOutcome.Win:
                    Wins++;
                    break;
                case MatchOutcome.Draw:
                    Draws++;
                    break;
                case MatchOutcome.Loss:
                    Losses++;
                    break;
            }
        }

        public void Reset()
        {
            _results.Clear();
            Wins = Draws = Losses = 0;
        }

        /// <summary>
        /// Elo difference (engine 1 minus engine 2). Positive means engine 1 is ahead.
        /// Infinite for a perfect (or zero) score.
        /// </summary>
        public double Elo => RateToElo(ScoreRate);

        /// <summary>Half-width of the 95% confidence interval on <see cref="Elo"/>, in Elo points.</summary>
        public double EloErrorMargin
        {
            get
            {
                if (Games < 2)
                {
                    return 0.0;
                }

                double n = Games;
                double rate = ScoreRate;

                // Variance of the per-game score around the mean, then standard error of the mean.
                double variance =
                    Wins / n * Square(1.0 - rate) +
                    Draws / n * Square(0.5 - rate) +
                    Losses / n * Square(0.0 - rate);

                double standardError = Math.Sqrt(variance / n);

                double low = RateToElo(Clamp01(rate - Z95 * standardError));
                double high = RateToElo(Clamp01(rate + Z95 * standardError));

                return (high - low) / 2.0;
            }
        }

        /// <summary>
        /// Likelihood of superiority: the probability that engine 1 is genuinely stronger,
        /// based on the decisive games. 0.5 when there are no decisive games.
        /// </summary>
        public double LikelihoodOfSuperiority
        {
            get
            {
                int decisive = Wins + Losses;

                if (decisive == 0)
                {
                    return 0.5;
                }

                return 0.5 * (1.0 + Erf((Wins - Losses) / Math.Sqrt(2.0 * decisive)));
            }
        }

        private double ScoreRate => Games == 0 ? 0.5 : Score / Games;

        private static double RateToElo(double rate)
        {
            if (rate <= 0.0)
            {
                return double.NegativeInfinity;
            }

            if (rate >= 1.0)
            {
                return double.PositiveInfinity;
            }

            return -400.0 * Math.Log10(1.0 / rate - 1.0);
        }

        private static double Clamp01(double value)
        {
            const double epsilon = 1e-9;

            if (value < epsilon)
            {
                return epsilon;
            }

            return value > 1.0 - epsilon ? 1.0 - epsilon : value;
        }

        private static double Square(double x) => x * x;

        // Error function, Abramowitz &amp; Stegun 7.1.26 (max error ~1.5e-7).
        private static double Erf(double x)
        {
            double t = 1.0 / (1.0 + 0.3275911 * Math.Abs(x));
            double y = 1.0 - (((((1.061405429 * t - 1.453152027) * t) + 1.421413741) * t - 0.284496736) * t + 0.254829592)
                       * t * Math.Exp(-x * x);

            return Math.Sign(x) * y;
        }
    }
}
