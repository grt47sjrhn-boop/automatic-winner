using System;
using System.Linq;
using substrate_shared.interfaces;
using substrate_shared.Overlays;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Extensions;
using substrate_shared.Registries.Extensions.structs;
using substrate_shared.Registries.Factories;
using substrate_shared.Resolvers.Base;
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
        private readonly Random _rng = new Random();

        public SimpleDuelResolver(BiasVector a, BiasVector b, int conflictBand = 1, Func<int, int>? magnitudeScaler = null)
        {
            _a = a;
            _b = b;
            _conflictBand = conflictBand;
            _magnitudeScaler = magnitudeScaler ?? (d => Math.Max(1, d));
        }

        private NarrativeTone PickToneByBias(Bias bias)
        {
            // Get all ToneType values that match the bias
            var candidates = Enum.GetValues(typeof(ToneType))
                                 .Cast<ToneType>()
                                 .Where(t => t.GetBias() == bias)
                                 .ToList();

            if (!candidates.Any())
            {
                var compositeTone = NarrativeToneFactory.FromRegistry(new RegistryValue<ToneType>(ToneType.Composite));
                return compositeTone;
            }

            // Randomly select one candidate
            var chosen = candidates[_rng.Next(candidates.Count)];
            return NarrativeToneFactory.FromRegistry(new RegistryValue<ToneType>(chosen));
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

                // Instead of always blending Joy, pick a tone by bias
                var sampledTone = PickToneByBias(winner.Tone.BiasValue);
                resolvedTone = sampledTone.BlendAgainst(loser.Tone);

                if (delta <= _conflictBand && Math.Sign(aStrength) != Math.Sign(bStrength))
                    outcome = DuelOutcome.MixedConflict;
                else if ((delta <= _conflictBand && (Math.Abs(aStrength) > 5 || Math.Abs(bStrength) > 5))
                         || resolvedMagnitude <= 5)
                {
                    outcome = DuelOutcome.Wound;
                }
                else
                {
                    outcome = resolvedTone.BiasValue switch
                    {
                        Bias.Positive => DuelOutcome.Recovery,
                        Bias.Negative => DuelOutcome.Collapse,
                        _ => DuelOutcome.Equilibrium
                    };
                }
            }

            var resolvedVector = new BiasVector(resolvedTone, resolvedMagnitude);

            string overlay = (_a.SignedStrength > 0 && _b.SignedStrength < 0) ||
                             (_a.SignedStrength < 0 && _b.SignedStrength > 0)
                ? GeometryOverlay.DescribeOverlay(_a, _b)
                : "Overlay → No opposing vectors for geometry.";

            string description =
                $"Outcome: {outcome}, Delta: {delta}, Resolved: {resolvedVector}. {overlay}";

            return new DuelEventSummary(
                "Duel Resolution",
                description,
                SummaryType.Duel,
                _a,
                _b,
                resolvedVector,
                outcome,
                true
            );
        }

        public override void Describe()
        {
            Resolve().Print();
        }
    }
}