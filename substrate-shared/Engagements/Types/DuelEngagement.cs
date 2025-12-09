using System;
using System.Linq;
using substrate_shared.Engagements.Base;
using substrate_shared.Facets.Enums;
using substrate_shared.Factories;
using substrate_shared.interfaces;
using substrate_shared.Managers;
using substrate_shared.Mappers;
using substrate_shared.Models;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Extensions;
using substrate_shared.structs;
using substrate_shared.Summaries;
using substrate_shared.types;

// 🔹 for ResultFactory

namespace substrate_shared.Engagements.Types
{
    /// <summary>
    /// Engagement representing a duel between two bias vectors.
    /// </summary>
    public sealed class DuelEngagement : EngagementBase
    {
        private readonly InventoryManager _inventory;

        // Injected managers
        private readonly IBiasManager _biasManager;
        private readonly IFacetManager _facetManager;
        private readonly IToneManager _toneManager;
        private readonly IRarityManager _rarityManager;

        public BiasVector DuelistA { get; }
        public BiasVector DuelistB { get; }
        public BiasVector ResolvedVector { get; private set; }
        public DuelOutcome Outcome { get; private set; }

        public DuelEngagement(
            InventoryManager inventory,
            BiasVector duelistA,
            BiasVector duelistB,
            IBiasManager biasManager,
            IFacetManager facetManager,
            IToneManager toneManager,
            IRarityManager rarityManager,
            Guid? biasSeedId = null)
        {
            _inventory     = inventory;
            DuelistA       = duelistA;
            DuelistB       = duelistB;
            _biasManager   = biasManager;
            _facetManager  = facetManager;
            _toneManager   = toneManager;
            _rarityManager = rarityManager;
            BiasSeedId     = biasSeedId;

            // Hydrate seed bias if provided
            if (BiasSeedId.HasValue)
            {
                var seedCrystal = _inventory.GetCrystal(BiasSeedId.Value);
                if (seedCrystal != null)
                {
                    Bias = new BiasDescriptor(
                        seedCrystal.GetBias(),
                        "Seeded from crystal facets",
                        seedCrystal.Facets
                    );
                }
            }
        }

        protected override void ResolveTick()
{
    // Resolve vector
    ResolvedVector = _biasManager.Resolve(DuelistA, DuelistB, Bias);

    // Determine outcome
    var dominantTone = ResolvedVector.DominantTone;
    var biasValue    = Bias?.Value ?? 0;

    switch (dominantTone)
    {
        case ToneType.Resilient:
        case ToneType.Radiant:
        case ToneType.Joy:
        case ToneType.Playful:
            Outcome = DuelOutcome.Recovery;
            break;
        case ToneType.Hostile:
        case ToneType.Critical:
        case ToneType.Despairing:
        case ToneType.Wound:
        case ToneType.Forsaken:
        case ToneType.Corrupted:
        case ToneType.Doomed:
            Outcome = DuelOutcome.Collapse;
            break;
        case ToneType.Harmonious:
        case ToneType.Calm:
        case ToneType.Reflective:
        case ToneType.Equilibrium:
            Outcome = DuelOutcome.Equilibrium;
            break;
        case ToneType.Conflict:
            Outcome = DuelOutcome.Conflict;
            break;
        case ToneType.Composite:
            Outcome = DuelOutcome.MixedConflict;
            break;
        default:
            Outcome = DuelOutcome.Unknown;
            break;
    }

    // Bias magnitude overrides
    if (biasValue > 5) Outcome = DuelOutcome.Recovery;
    else if (biasValue < -5) Outcome = DuelOutcome.Collapse;
    else if (Math.Abs(biasValue) <= 2) Outcome = DuelOutcome.Wound;

    // Convert resolved vector into facets
    Shape = ResolvedVector.ToFacetDistribution();

    // Update Bias, Brilliance
    Bias = _biasManager.Summarize(Shape);
    var toneDict = FacetToneMapper.ToToneDictionary(Shape);
    Brilliance = _toneManager.Cut(toneDict);

    // Outcome-aware rarity assignment
    Rarity = ResultFactory.CreateRarityTier(Outcome, Shape.Threshold, toneDict);

    // 🔹 Forge a crystal for this duel
    var crystal = TraitCrystalFactory.CreateCrystal(
        threshold: Shape.Threshold,
        // Treat only Recovery as positive; Equilibrium is neutral
        isPositive: Outcome == DuelOutcome.Recovery,
        facets: toneDict,
        narrative: Rarity.Description,
        existingCrystals: _inventory.GetCrystals().ToList(),
        toneCut: Brilliance,
        rarityTier: Rarity
    );

    // 🔹 Persist crystal so ReportBuilder can see it
    _inventory.AddCrystal(crystal);
}

        public override void UpdateFacets()
        {
            Shape = ResolvedVector.ToFacetDistribution();
        }

        public override void UpdateBias()
        {
            Bias = _biasManager.Summarize(Shape);
        }

        public override void UpdateBrilliance()
        {
            var toneDict = FacetToneMapper.ToToneDictionary(Shape);
            Brilliance = _toneManager.Cut(toneDict);
        }

        public override void UpdateRarity()
        {
            var toneDict = FacetToneMapper.ToToneDictionary(Shape);
            Rarity = ResultFactory.CreateRarityTier(Outcome, Shape.Threshold, toneDict);
        }

        public override bool IsComplete =>
            Math.Abs(Shape.Values[FacetType.Resilience]) >= 6;

        public override ISummary Finalize()
        {
            var resolvedMood   = ResolveMoodFromBias(Bias.Value);
            var resolvedIntent = ResolveIntentFromBias(Bias.Value);
            var resolvedRarity = MapToCrystalRarity(Rarity);

            return new DuelEventSummary(
                "Duel Engagement Complete",
                $"Resilience index: {Shape.Values[FacetType.Resilience]}",
                SummaryType.Duel,
                DuelistA,
                DuelistB,
                ResolvedVector,
                Outcome,
                Brilliance,
                Bias,
                resolvedMood,
                resolvedIntent,
                resolvedRarity,
                isResolved: IsComplete
            );
        }

        // Helpers
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

        public void Resolve()
        {
            // Run the engagement tick
            ResolveTick();

            // Update derived properties
            UpdateFacets();
            UpdateBias();
            UpdateBrilliance();
            UpdateRarity();
        }

    }
}