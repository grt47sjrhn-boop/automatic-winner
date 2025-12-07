using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.interfaces;
using substrate_shared.Overlays;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums;
using substrate_shared.Resolvers.Base;
using substrate_shared.structs;
using substrate_shared.Summaries;
using substrate_shared.types;

namespace substrate_shared.Resolvers
{
    public class MultiAxisDuelResolver : Resolver
    {
        public override string Name { get; } = "Multi-Axis Duel Resolver";

        private readonly IEnumerable<BiasVector> _vectors;
        private readonly int _conflictBand;
        private readonly Func<int, int> _magnitudeScaler;

        public MultiAxisDuelResolver(IEnumerable<BiasVector> vectors, int conflictBand = 1, Func<int, int>? magnitudeScaler = null)
        {
            _vectors = vectors ?? throw new ArgumentNullException(nameof(vectors));
            _conflictBand = conflictBand;
            _magnitudeScaler = magnitudeScaler ?? (d => Math.Max(1, d));
        }

        public override ISummary Resolve()
        {
            if (!_vectors.Any())
                throw new ArgumentException("No vectors provided for resolution.");

            var totalStrength = _vectors.Sum(v => v.SignedStrength);
            var totalMagnitude = _vectors.Sum(v => v.Magnitude);

            var bias = Math.Sign(totalStrength) switch
            {
                > 0 => Bias.Positive,
                < 0 => Bias.Negative,
                _   => Bias.Neutral
            };

            var compositeTone = new NarrativeTone(ToneType.Composite,"Composite Duel", "MultiAxis", bias, "Composite");
            var resolvedVector = new BiasVector(compositeTone, Math.Max(1, totalMagnitude / _vectors.Count()));

            var outcome = bias switch
            {
                Bias.Positive => DuelOutcome.Recovery,
                Bias.Negative => DuelOutcome.Collapse,
                _             => DuelOutcome.Equilibrium
            };

            if (Math.Abs(totalStrength) <= _conflictBand && totalMagnitude > 10)
                outcome = DuelOutcome.Wound;

            // Sequence checks instead of null comparison
            var posSeq = _vectors.Where(v => v.SignedStrength > 0).OrderByDescending(v => v.Magnitude);
            var negSeq = _vectors.Where(v => v.SignedStrength < 0).OrderByDescending(v => v.Magnitude);

            string overlay;
            if (posSeq.Any() && negSeq.Any())
            {
                var strongestPositive = posSeq.First();
                var strongestNegative = negSeq.First();
                overlay = GeometryOverlay.DescribeOverlay(strongestPositive, strongestNegative);
            }
            else
            {
                overlay = "Overlay → No opposing vectors for geometry.";
            }

            var description = $"Outcome: {outcome}, Delta: {Math.Abs(totalStrength)}, Resolved: {resolvedVector}. {overlay}";

            return new EventSummary("Multi-Axis Duel Resolution", description, SummaryType.Duel, true);
        }

        public override void Describe()
        {
            Resolve().Print();
        }
    }
}