using System;
using System.Collections.Generic;
using substrate_shared.Facets.Enums;
using substrate_shared.Factories;
using substrate_shared.interfaces;
using substrate_shared.Models;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Managers;
using substrate_shared.structs;
using substrate_shared.Summaries;
using substrate_shared.Traits.Base;
using substrate_shared.types;

namespace substrate_shared.Engagements.Types;

public class DuelEngagement : IEngagement
{
    private readonly List<TraitCrystal> _forgedCrystals = new();
    private readonly IToneManager _toneManager;
    private readonly IRarityManager _rarityManager;

    // Track cumulative resilience across ticks/duels
    private int _cumulativeResilience;

    public DuelEngagement(
        IToneManager toneManager,
        IRarityManager rarityManager,
        BiasVector duelistA,
        BiasVector duelistB,
        Guid? biasSeedId = null)
    {
        Id = Guid.NewGuid();
        _toneManager = toneManager;
        _rarityManager = rarityManager;

        DuelistA = duelistA;
        DuelistB = duelistB;
        BiasSeedId = biasSeedId;

        Shape = MergeFacetDistributions(DuelistA.ToFacetDistribution(), DuelistB.ToFacetDistribution());
        Bias = new BiasDescriptor { Bias = substrate_shared.Registries.enums.Bias.Neutral, Value = 0, Narrative = "Neutral baseline" };
        Brilliance = new ToneCut();
    }

    public Guid Id { get; }
    public Guid? BiasSeedId { get; private set; }

    public BiasVector DuelistA { get; }
    public BiasVector DuelistB { get; }

    public BiasVector ResolvedVector { get; private set; }
    public MoodType ResolvedMood { get; private set; } = MoodType.Neutral;
    public IntentAction Intent { get; private set; } = IntentAction.Observe;
    public RarityTier Rarity { get; private set; } = new RarityTier("Common", "Default rarity tier.");

    public FacetDistribution Shape { get; private set; } = new();
    public BiasDescriptor Bias { get; private set; } = new();
    public ToneCut Brilliance { get; private set; } = new();
    public bool IsComplete { get; private set; }

    public IEnumerable<TraitCrystal> ForgedCrystals => _forgedCrystals;
    public int CumulativeResilience => _cumulativeResilience;

    public void ResolveStep(int ticks = 1)
    {
        UpdateFacets();

        var avgMagnitude = (DuelistA.Magnitude + DuelistB.Magnitude) / 2;
        var dominantTone =
            DuelistA.Tone ?? DuelistB.Tone ??
            new NarrativeTone(ToneType.Harmonious, "Neutral", "Equilibrium",
                substrate_shared.Registries.enums.Bias.Neutral, "Equilibrium");

        ResolvedVector = new BiasVector(dominantTone, avgMagnitude);

        ResolvedMood = SuperRegistryManager.GetMoodByStrength(ResolvedVector.SignedStrength);

        UpdateBias();
        UpdateBrilliance();
        UpdateRarity();

// Add duel tension to cumulative resilience for reporting
        var delta = Math.Abs(DuelistA.SignedStrength - DuelistB.SignedStrength);
        _cumulativeResilience += delta;


        if (Math.Abs(Bias.Value) >= 6)
        {
            var facets = SafeToneDictionary(Shape);
            var narrative = SuperRegistryManager.DescribeClusterWithScore(NarrativeGroup.Crystal);
            var toneCut = _toneManager.Cut(facets);
            var rarityTier = _rarityManager.AssignTier((int)Bias.Value);

            var crystal = TraitCrystalFactory.CreateCrystal(
                threshold: Bias.Value > 0 ? 6 : -6,
                isPositive: Bias.Value > 0,
                facets: facets,
                narrative: narrative,
                existingCrystals: _forgedCrystals,
                toneCut: toneCut,
                rarityTier: rarityTier
            );

            _forgedCrystals.Add(crystal);

            // Clamp bias only, don’t reset momentum
            Bias = new BiasDescriptor
            {
                Bias = Bias.Bias,
                Value = Math.Max(-11, Math.Min(11, Bias.Value)),
                Narrative = Bias.Narrative
            };
        }

        // Intent modulation
        Intent = Bias.Value switch
        {
            >= 3  => IntentAction.Approve,
            <= -3 => IntentAction.Reject,
            _     => IntentAction.Observe
        };
    }

    public void UpdateFacets()
    {
        Shape = MergeFacetDistributions(DuelistA.ToFacetDistribution(), DuelistB.ToFacetDistribution());
    }

    public void UpdateBias()
    {
        var signed = ResolvedVector.SignedStrength;

        var polarity = signed > 0 ? substrate_shared.Registries.enums.Bias.Positive :
            signed < 0 ? substrate_shared.Registries.enums.Bias.Negative :
            substrate_shared.Registries.enums.Bias.Neutral;

        var progressed = Bias.Value + signed;
        var decayed = progressed + (progressed > 0 ? -1 : progressed < 0 ? 1 : 0);
        var newValue = Math.Max(-11, Math.Min(11, decayed));

        Bias = new BiasDescriptor
        {
            Bias = polarity,
            Value = newValue,
            Narrative = $"Bias progressed to {newValue} (Δ {signed}, decay applied)"
        };
    }

    public void UpdateBrilliance()
    {
        var facets = SafeToneDictionary(Shape);
        Brilliance = _toneManager.Cut(facets);
    }

    public void UpdateRarity()
    {
        var tierLabel = Bias.Value >= 8 ? "Epic" :
            Bias.Value >= 4 ? "Rare" :
            Bias.Value >= 2 ? "Common" : "Fragile";

        var tierDesc = Bias.Value >= 8 ? "Forged under high resilience." :
            Bias.Value >= 4 ? "Forged under moderate resilience." :
            Bias.Value >= 2 ? "Default rarity tier." :
            "Delicate, unstable resonance.";

        Rarity = new RarityTier(tierLabel, tierDesc);
    }

    public ISummary Finalize()
    {
        IsComplete = true;

        // 🔹 Compute duel tension (difference in signed strengths)
        var delta = Math.Abs(DuelistA.SignedStrength - DuelistB.SignedStrength);

        string title = "Duel Engagement Complete";
        string description = $"Resilience index: {delta} (Cumulative: {_cumulativeResilience})";

        // 🔹 Outcome decision based on bias value
        DuelOutcome outcome = Bias.Value switch
        {
            >= 8  => DuelOutcome.Recovery,
            <= -8 => DuelOutcome.Collapse,
            >= 4  => DuelOutcome.Conflict,
            <= -4 => DuelOutcome.Wound,
            _     => DuelOutcome.Equilibrium
        };

        // 🔹 Map rarity tier to enum
        CrystalRarity rarityEnum = Rarity.Tier switch
        {
            "Epic"      => CrystalRarity.Epic,
            "Rare"      => CrystalRarity.Rare,
            "Common"    => CrystalRarity.Common,
            "Fragile"   => CrystalRarity.Fragile,
            "Mythic"    => CrystalRarity.Mythic,
            "Legendary" => CrystalRarity.Legendary,
            "UltraRare" => CrystalRarity.UltraRare,
            _           => CrystalRarity.Common
        };

        return new DuelEventSummary(
            title,
            description,
            SummaryType.Duel,
            DuelistA,
            DuelistB,
            ResolvedVector,
            outcome,
            Brilliance,
            Bias,
            ResolvedMood,
            Intent,
            rarityEnum,
            Shape,
            resilienceIndex: delta,                  // 🔹 per-duel resilience index
            cumulativeResilience: _cumulativeResilience, // 🔹 running total across ticks
            isResolved: IsComplete
        );
    }

    private static FacetDistribution MergeFacetDistributions(FacetDistribution a, FacetDistribution b)
    {
        var result = new Dictionary<FacetType, int>();
        foreach (var type in (FacetType[])Enum.GetValues(typeof(FacetType)))
        {
            var va = a.Values.TryGetValue(type, out var av) ? av : 0;
            var vb = b.Values.TryGetValue(type, out var bv) ? bv : 0;
            result[type] = va + vb;
        }
        return new FacetDistribution(result);
    }

    private static Dictionary<ToneType, int> SafeToneDictionary(FacetDistribution facets)
    {
        var values = facets.Values ?? new Dictionary<FacetType, int>();
        int get(FacetType t) => values.TryGetValue(t, out var v) ? v : 0;

        return new Dictionary<ToneType, int>
        {
            [ToneType.Resilient]  = get(FacetType.Resilience),
            [ToneType.Harmonious] = get(FacetType.Harmony),
            [ToneType.Conflict]   = get(FacetType.Conflict),
            [ToneType.Radiant]    = get(FacetType.Radiance)
        };
    }
}