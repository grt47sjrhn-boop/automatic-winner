using System;
using System.Linq;
using substrate_shared.interfaces;
using substrate_shared.Mappers;
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
using substrate_shared.Models; // for RarityTier

namespace substrate_shared.Resolvers
{
    public class SimpleDuelResolver : Resolver
    {
        public override string Name { get; } = "Simple Duel Resolver (Unified)";

        private readonly BiasVector _a;
        private readonly BiasVector _b;
        private readonly int _conflictBand;
        private readonly Func<int, int> _magnitudeScaler;
        private readonly Random _rng = new Random();

        // Managers
        private readonly IBiasManager _biasManager;
        private readonly IFacetManager _facetManager;
        private readonly IToneManager _toneManager;
        private readonly IRarityManager _rarityManager;

        public SimpleDuelResolver(
            BiasVector a,
            BiasVector b,
            IBiasManager biasManager,
            IFacetManager facetManager,
            IToneManager toneManager,
            IRarityManager rarityManager,
            int conflictBand = 2,
            Func<int, int>? magnitudeScaler = null)
        {
            _a = a;
            _b = b;
            _biasManager = biasManager;
            _facetManager = facetManager;
            _toneManager = toneManager;
            _rarityManager = rarityManager;
            _conflictBand = conflictBand;

            _magnitudeScaler = magnitudeScaler ?? (d => d);
        }

        private NarrativeTone PickToneByBias(Bias bias)
        {
            var candidates = Enum.GetValues(typeof(ToneType))
                                 .Cast<ToneType>()
                                 .Where(t => t.GetBias() == bias)
                                 .ToList();

            if (!candidates.Any())
                return NarrativeToneFactory.FromRegistry(new RegistryValue<ToneType>(ToneType.Composite));

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

            // Equilibrium on exact tie
            if (aStrength == bStrength)
            {
                resolvedTone = _a.Tone.MergeNeutral(_b.Tone);
                resolvedMagnitude = Math.Max(1, Math.Max(_a.Magnitude, _b.Magnitude) / 2);
                outcome = DuelOutcome.Equilibrium;
            }
            else
            {
                var winner = aStrength > bStrength ? _a : _b;
                var loser  = aStrength > bStrength ? _b : _a;

                resolvedMagnitude = _magnitudeScaler(delta);

                var sampledTone = PickToneByBias(winner.Tone.BiasValue);
                resolvedTone = sampledTone.BlendAgainst(loser.Tone);

                // Recovery/Collapse require tone alignment AND sufficient magnitude
                var strong = Math.Abs(resolvedMagnitude) > _conflictBand;

                if (resolvedTone.BiasValue == Bias.Positive && strong)
                    outcome = DuelOutcome.Recovery;
                else if (resolvedTone.BiasValue == Bias.Negative && strong)
                    outcome = DuelOutcome.Collapse;
                else if (delta <= _conflictBand && Math.Sign(aStrength) != Math.Sign(bStrength))
                    outcome = DuelOutcome.MixedConflict;
                else if (Math.Abs(resolvedMagnitude) <= _conflictBand)
                    outcome = DuelOutcome.Wound;
                else
                    outcome = DuelOutcome.Equilibrium;
            }

            var resolvedVector = new BiasVector(resolvedTone, resolvedMagnitude);

            var overlay = (_a.SignedStrength > 0 && _b.SignedStrength < 0) ||
                          (_a.SignedStrength < 0 && _b.SignedStrength > 0)
                ? GeometryOverlay.DescribeOverlay(_a, _b)
                : "Overlay → No opposing vectors for geometry.";

            var description = $"Outcome: {outcome}, Delta: {delta}, Resolved: {resolvedVector}. {overlay}";

            // Managers compute brilliance and bias
            var shape = _facetManager.Normalize(resolvedVector.ToFacetDistribution().Values);
            var toneDict = FacetToneMapper.ToToneDictionary(shape);
            var brilliance = _toneManager.Cut(toneDict);
            var bias = _biasManager.Summarize(shape);

            // Enrich with Mood, Intent, Rarity
            var resolvedMood   = ResolveMoodFromBias(bias.Value);
            var resolvedIntent = ResolveIntentFromBias(bias.Value);

            var rarityTier = _rarityManager.AssignTier(_rarityManager.ComputeScore(shape));
            var resolvedRarity = MapToCrystalRarity(rarityTier);

            return new DuelEventSummary(
                "Duel Resolution",
                description,
                SummaryType.Duel,
                _a,
                _b,
                resolvedVector,
                outcome,
                brilliance,
                bias,
                resolvedMood,
                resolvedIntent,
                resolvedRarity,
                shape,
                resilienceIndex: resolvedMagnitude,          // 🔹 per-duel resolved index
                cumulativeResilience: Math.Abs(resolvedMagnitude), // 🔹 or another cumulative metric
                true
            );
        }

        private static MoodType ResolveMoodFromBias(double biasValue)
        {
            var clamped = Math.Max(-11, Math.Min(11, (int)Math.Round(biasValue)));
            var entries = Enum.GetValues(typeof(MoodType)).Cast<MoodType>();
            return entries.FirstOrDefault(m => m.GetScaleValue() == clamped);
        }

        private static IntentAction ResolveIntentFromBias(double biasValue)
        {
            return biasValue switch
            {
                > 0 => IntentAction.Encourage,
                < 0 => IntentAction.Criticize,
                _ => IntentAction.Observe
            };
        }

        private static CrystalRarity MapToCrystalRarity(RarityTier tier)
        {
            return tier.Tier switch
            {
                "Common"    => CrystalRarity.Common,
                "Rare"      => CrystalRarity.Rare,
                "Epic"      => CrystalRarity.Epic,
                "Mythic"    => CrystalRarity.Mythic,
                "Legendary" => CrystalRarity.Legendary,
                "UltraRare" => CrystalRarity.UltraRare,

                "Fragile"   => CrystalRarity.Fragile,
                "Corrupted" => CrystalRarity.Corrupted,
                "Doomed"    => CrystalRarity.Doomed,

                _           => CrystalRarity.Common
            };
        }

        public override void Describe()
        {
            Resolve().Print();
        }
    }
}