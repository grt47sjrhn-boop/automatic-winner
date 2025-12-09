using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.Facets.Enums;
using substrate_shared.interfaces;
using substrate_shared.structs;
using substrate_shared.Resolvers.Base;
using substrate_shared.Summaries;
using substrate_shared.Registries.enums;
using substrate_shared.types;
using substrate_shared.Mappers;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.Extensions;
using substrate_shared.Registries.Extensions.structs;
using substrate_shared.Registries.Factories;
using substrate_shared.Models; // for RarityTier

namespace substrate_shared.Resolvers
{
    public class MultiAxisDuelResolver : Resolver
    {
        public override string Name { get; } = "Multi-Axis Duel Resolver";

        private readonly IEnumerable<BiasVector> _vectors;
        private readonly int _conflictBand;
        private readonly Func<int,int> _magnitudeScaler;

        // 🔹 Injected managers
        private readonly IBiasManager _biasManager;
        private readonly IFacetManager _facetManager;
        private readonly IToneManager _toneManager;
        private readonly IRarityManager _rarityManager;

        public MultiAxisDuelResolver(
            IEnumerable<BiasVector> vectors,
            IBiasManager biasManager,
            IFacetManager facetManager,
            IToneManager toneManager,
            IRarityManager rarityManager,
            int conflictBand = 1,
            Func<int,int>? magnitudeScaler = null)
        {
            _vectors        = vectors;
            _biasManager    = biasManager;
            _facetManager   = facetManager;
            _toneManager    = toneManager;
            _rarityManager  = rarityManager;
            _conflictBand   = conflictBand;

            // 🔹 Preserve negatives instead of flattening to ≥ 1
            _magnitudeScaler = magnitudeScaler ?? (d => d);
        }

        public override ISummary Resolve()
        {
            if (_vectors == null || !_vectors.Any())
                throw new InvalidOperationException("MultiAxisDuelResolver requires at least one BiasVector.");

            // 🔹 Aggregate all participant vectors into a collective facet distribution
            var distributions = _vectors.Select(v => v.ToFacetDistribution()).ToList();
            var collectiveShape = _facetManager.Aggregate(distributions);

            // 🔹 Summarize bias from collective shape
            var collectiveBias = _biasManager.Summarize(collectiveShape);

            // 🔹 Compute brilliance (ToneCut) from facets
            var toneDict = FacetToneMapper.ToToneDictionary(collectiveShape);
            var brilliance = _toneManager.Cut(toneDict);

            // 🔹 Compute rarity tier from resilience score
            var rarityTier = _rarityManager.AssignTier(_rarityManager.ComputeScore(collectiveShape));

            // 🔹 Map RarityTier → CrystalRarity
            var resolvedRarity = MapToCrystalRarity(rarityTier);

            // 🔹 Determine outcome heuristically (balanced positive/negative arcs)
            DuelOutcome outcome;
            if (collectiveBias.Bias == Bias.Positive && collectiveBias.Value > 5)
                outcome = DuelOutcome.Recovery;
            else if (collectiveBias.Bias == Bias.Negative && collectiveBias.Value < -5)
                outcome = DuelOutcome.Collapse;
            else if (collectiveBias.Bias == Bias.Mixed)
                outcome = DuelOutcome.MixedConflict;
            else if (Math.Abs(collectiveBias.Value) <= 2)
                outcome = DuelOutcome.Wound;
            else
                outcome = DuelOutcome.Equilibrium;

            var description =
                $"Multi-axis duel resolved with {_vectors.Count()} participants. " +
                $"Outcome: {outcome}, Bias: {collectiveBias.Bias}, Rarity: {rarityTier.Tier}, " +
                $"Brilliance: {brilliance.Primary}.";

            // 🔹 For now, pick first two vectors as 'duelists' for summary compatibility
            var duelists = _vectors.Take(2).ToList();
            var duelistA = duelists.ElementAtOrDefault(0);
            var duelistB = duelists.ElementAtOrDefault(1);

            // 🔹 Derive dominant facet type and map to NarrativeTone
            var dominantFacetType = collectiveShape.Values
                .OrderByDescending(kv => kv.Value)
                .First().Key;

            var dominantTone = NarrativeToneFactory.FromRegistry(
                new RegistryValue<FacetType>(dominantFacetType)
            );

            var resolvedVector = new BiasVector(
                dominantTone,
                collectiveShape.Values.Values.Sum()
            );

            // 🔹 Enrich with Mood, Intent, Rarity
            var resolvedMood   = ResolveMoodFromBias(collectiveBias.Value);
            var resolvedIntent = ResolveIntentFromBias(collectiveBias.Value);

            return new DuelEventSummary(
                "Multi-Axis Duel Resolution",
                description,
                SummaryType.Duel,
                duelistA,
                duelistB,
                resolvedVector,
                outcome,
                brilliance,
                collectiveBias,
                resolvedMood,
                resolvedIntent,
                resolvedRarity,
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
            if (biasValue > 0) return IntentAction.Encourage;
            if (biasValue < 0) return IntentAction.Criticize;
            return IntentAction.Observe;
        }

        private static CrystalRarity MapToCrystalRarity(RarityTier tier)
        {
            return tier.Tier switch
            {
                // 🌞 Recovery path
                "Common"    => CrystalRarity.Common,
                "Rare"      => CrystalRarity.Rare,
                "Epic"      => CrystalRarity.Epic,
                "Mythic"    => CrystalRarity.Mythic,
                "Legendary" => CrystalRarity.Legendary,
                "UltraRare" => CrystalRarity.UltraRare,

                // 🌑 Collapse path (new abyssal tiers)
                "Fragile"   => CrystalRarity.Fragile,    // collapse with weak resonance
                "Corrupted" => CrystalRarity.Corrupted,  // collapse with twisted resonance
                "Doomed"    => CrystalRarity.Doomed,     // irreversible collapse

                _           => CrystalRarity.Common
            };
        }


        public override void Describe()
        {
            Resolve().Print();
        }
    }
}