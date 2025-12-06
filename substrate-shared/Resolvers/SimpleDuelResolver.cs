using System;
using substrate_core.Resolvers.Base;
using substrate_shared.interfaces;
using substrate_shared.Overlays;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.Summaries;
using substrate_shared.types;

namespace substrate_shared.Resolvers
{
    public class SimpleDuelResolver : Resolver
    {
        public override string Name { get; } = "Simple Duel Resolver";

        private readonly BiasVector _a;
        private readonly BiasVector _b;
        private readonly int _conflictBand;
        private readonly Func<int, int> _magnitudeScaler;

        public SimpleDuelResolver(BiasVector a, BiasVector b, int conflictBand = 1, Func<int, int>? magnitudeScaler = null)
        {
            _a = a;
            _b = b;
            _conflictBand = conflictBand;
            _magnitudeScaler = magnitudeScaler ?? (d => Math.Max(1, d));
        }

        public override ISummary Resolve()
        {
            var aStrength = _a.SignedStrength;
            var bStrength = _b.SignedStrength;
            var delta = Math.Abs(aStrength - bStrength);

            DuelOutcome outcome;
            NarrativeTone resolvedTone;
            int resolvedMagnitude;

            if (aStrength == bStrength)
            {
                resolvedTone = _a.Tone.MergeNeutral(_b.Tone);
                resolvedMagnitude = Math.Max(1, Math.Max(_a.Magnitude, _b.Magnitude) / 2);
                outcome = DuelOutcome.Equilibrium;
            }
            else
            {
                var winner = aStrength > bStrength ? _a : _b;
                var loser = aStrength > bStrength ? _b : _a;

                resolvedMagnitude = _magnitudeScaler(delta);
                resolvedTone = winner.Tone.BlendAgainst(loser.Tone);

                if (delta <= _conflictBand && Math.Sign(aStrength) != Math.Sign(bStrength))
                    outcome = DuelOutcome.MixedConflict;
                else if (delta <= _conflictBand && (Math.Abs(aStrength) > 5 || Math.Abs(bStrength) > 5))
                    outcome = DuelOutcome.Wound;
                else if (resolvedTone.BiasValue == Bias.Positive)
                    outcome = DuelOutcome.Recovery;
                else if (resolvedTone.BiasValue == Bias.Negative)
                    outcome = DuelOutcome.Collapse;
                else
                    outcome = DuelOutcome.Equilibrium;
            }

            var resolvedVector = new BiasVector(resolvedTone, resolvedMagnitude);

            // Sequence-based overlay check
            string overlay;
            if ((_a.SignedStrength > 0 && _b.SignedStrength < 0) ||
                (_a.SignedStrength < 0 && _b.SignedStrength > 0))
            {
                overlay = GeometryOverlay.DescribeOverlay(_a, _b);
            }
            else
            {
                overlay = "Overlay → No opposing vectors for geometry.";
            }

            string description =
                $"Outcome: {outcome}, Delta: {delta}, Resolved: {resolvedVector}. {overlay}";

            return new EventSummary("Duel Resolution", description, SummaryType.Duel, true);
        }

        public override void Describe()
        {
            Resolve().Print();
        }
    }
}