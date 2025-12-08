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
using substrate_shared.Registries.Extensions.structs;
using substrate_shared.Registries.Factories;

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
            _magnitudeScaler = magnitudeScaler ?? (d => Math.Max(1, d));
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

            // 🔹 Determine outcome heuristically
            DuelOutcome outcome;
            if (collectiveBias.Bias == Bias.Positive)
                outcome = DuelOutcome.Recovery;
            else if (collectiveBias.Bias == Bias.Negative)
                outcome = DuelOutcome.Collapse;
            else if (collectiveBias.Bias == Bias.Mixed)
                outcome = DuelOutcome.MixedConflict;
            else
                outcome = DuelOutcome.Equilibrium;

            var description =
                $"Multi-axis duel resolved with {_vectors.Count()} participants. " +
                $"Outcome: {outcome}, Bias: {collectiveBias.Bias}, Rarity: {rarityTier}, " +
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
                true
            );
        }

        public override void Describe()
        {
            Resolve().Print();
        }
    }
}