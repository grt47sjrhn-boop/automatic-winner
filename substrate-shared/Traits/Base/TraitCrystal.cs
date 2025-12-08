using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.Models;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.interfaces;
using substrate_shared.Registries.Managers;
using substrate_shared.Traits.Enums;

namespace substrate_shared.Traits.Base
{
    public abstract class TraitCrystal : IReadableRegistry
    {
        public CrystalType Type { get; }
        public CrystalRarity Rarity { get; }
        public int Threshold { get; }
        public IReadOnlyDictionary<ToneType,int> Facets { get; }
        public int ModifierValue { get; }
        public Guid EngagementId { get; set; }
        public ToneCut ToneCut { get; set; }
        public RarityTier RarityTier { get; set; }


        protected TraitCrystal(CrystalType type, CrystalRarity rarity, int threshold,
            IReadOnlyDictionary<ToneType,int> facets, ToneCut toneCut, RarityTier rarityTier)
        {
            Type = type;
            Rarity = rarity;
            Threshold = threshold;
            Facets = facets;
            ToneCut = toneCut;
            RarityTier = rarityTier;
            ModifierValue = CalculateModifier(facets, rarity);
        }

        // Each subtype defines how it modifies duel outcomes
        public abstract int ModifyOutcome(DuelOutcome outcome, int baseValue);

        // Shared modifier calculation logic
        protected virtual int CalculateModifier(
            IReadOnlyDictionary<ToneType,int> facets, CrystalRarity rarity)
        {
            var sum = facets.Values.Sum();
            return rarity switch
            {
                CrystalRarity.Rare => (int)(sum * 1.5),
                CrystalRarity.UltraRare => sum * 2,
                _ => sum
            };
        }

        // IReadableRegistry implementation
        public string GetDescription()
        {
            var facetSummary = string.Join(", ",
                Facets.Select(f => $"{f.Key}({f.Value})"));
            return $"{Rarity} {Type} Crystal forged at {Threshold}, facets: {facetSummary}, modifier: {ModifierValue}";
        }

        public Bias GetBias()
        {
            // Bias scoring derived from facet metadata
            var positives = Facets.Keys.Count(t => RegistryManager<ToneType>.Get(t).GetBias() == Bias.Positive);
            var negatives = Facets.Keys.Count(t => RegistryManager<ToneType>.Get(t).GetBias() == Bias.Negative);

            switch (positives)
            {
                case > 0 when negatives > 0:
                    return Bias.Mixed;
                case > 0:
                    return Bias.Positive;
            }

            return negatives > 0 ? Bias.Negative : Bias.Neutral;
        }

        public NarrativeGroup GetGroup() => NarrativeGroup.Crystal;
        public ToneType GetToneType()
        {
            // Return the dominant tone based on facet counts
            if (!Facets.Any())
                return ToneType.Neutral; // fallback

            return Facets
                .OrderByDescending(f => f.Value)
                .First().Key;
        }
    }
}