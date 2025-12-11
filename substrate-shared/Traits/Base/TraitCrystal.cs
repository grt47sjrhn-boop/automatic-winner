using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.Enums;
using substrate_shared.interfaces.Details;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.interfaces;
using substrate_shared.Registries.Managers;

namespace substrate_shared.Traits.Base
{
    /// <summary>
    /// Represents a crystal forged during duels, with facets, rarity, and narrative overlays.
    /// </summary>
    public abstract class TraitCrystal : IReadableRegistry
    {
        public CrystalType Type { get; }
        public CrystalRarity Rarity { get; }
        public int Threshold { get; }
        public IReadOnlyDictionary<ToneType,int> Facets { get; }
        public Guid EngagementId { get; set; }
        public IToneCut ToneCut { get; set; }
        public IRarityTier RarityTier { get; set; }

        /// <summary>
        /// Effective rarity resolved from RarityTier.
        /// </summary>
        public CrystalRarity ResolvedRarity
        {
            get
            {
                var tier = RarityTier?.Tier?.Trim();
                return tier?.ToLowerInvariant() switch
                {
                    "Rare"      => CrystalRarity.Rare,
                    "Epic"      => CrystalRarity.Epic,
                    "Mythic"    => CrystalRarity.Mythic,
                    "UltraRare" => CrystalRarity.UltraRare,
                    "Legendary" => CrystalRarity.Legendary,   // ✅ add this case
                    "Fragile"   => CrystalRarity.Fragile,     // if supported
                    "Corrupted" => CrystalRarity.Corrupted,   // if supported
                    "Doomed"    => CrystalRarity.Doomed,      // if supported
                    "equilibrium" => CrystalRarity.Equilibrium,
                    _           => CrystalRarity.Common
                };
            }
        }

        /// <summary>
        /// Modifier value recalculated dynamically based on ResolvedRarity.
        /// </summary>
        public int ModifierValue => CalculateModifier(Facets, ResolvedRarity);

        protected TraitCrystal(
            CrystalType type,
            CrystalRarity rarity,
            int threshold,
            IReadOnlyDictionary<ToneType,int> facets,
            IToneCut toneCut,
            IRarityTier rarityTier)
        {
            Type = type;
            Rarity = rarity;
            Threshold = threshold;
            Facets = facets ?? new Dictionary<ToneType,int>();
            ToneCut = toneCut;
            RarityTier = rarityTier;
        }

        /// <summary>
        /// Each subtype defines how it modifies duel outcomes.
        /// </summary>
        public abstract int ModifyOutcome(DuelOutcome outcome, int baseValue);

        /// <summary>
        /// Shared modifier calculation logic based on rarity tier.
        /// </summary>
        protected virtual int CalculateModifier(
            IReadOnlyDictionary<ToneType,int> facets,
            CrystalRarity rarity)
        {
            var sum = facets?.Values.Sum() ?? 0;
            return rarity switch
            {
                CrystalRarity.Rare      => (int)(sum * 1.5),
                CrystalRarity.Epic      => (int)(sum * 1.75),
                CrystalRarity.Mythic    => sum * 2,
                CrystalRarity.UltraRare => sum * 2,
                _                       => sum
            };
        }

        /// <summary>
        /// Generate a description of the crystal, optionally enriched with overlay metrics.
        /// </summary>
        public string GetDescription(double? hypotenuse = null, double? area = null)
        {
            var facetSummary = string.Join(", ", Facets.Select(f => $"{f.Key}({f.Value})"));
            var overlayInfo = hypotenuse.HasValue && area.HasValue
                ? $" [Hypotenuse {hypotenuse:F1}, Area {area:F1}]"
                : string.Empty;

            return $"{ResolvedRarity} {Type} Crystal forged at {Threshold}, facets: {facetSummary}, modifier: {ModifierValue}{overlayInfo}";
        }

        /// <summary>
        /// Bias scoring derived from facet metadata.
        /// </summary>
        public Bias GetBias()
        {
            var positives = Facets.Keys.Count(t => RegistryManager<ToneType>.Get(t).GetBias() == Bias.Positive);
            var negatives = Facets.Keys.Count(t => RegistryManager<ToneType>.Get(t).GetBias() == Bias.Negative);

            return positives switch
            {
                > 0 when negatives > 0 => Bias.Mixed,
                > 0 => Bias.Positive,
                _ => negatives > 0 ? Bias.Negative : Bias.Neutral
            };
        }

        public NarrativeGroup GetGroup() => NarrativeGroup.Crystal;

        /// <summary>
        /// Return the dominant tone based on facet counts.
        /// </summary>
        public ToneType GetToneType()
        {
            if (!Facets.Any())
                return ToneType.Neutral; // fallback

            return Facets
                .OrderByDescending(f => f.Value)
                .First().Key;
        }
    }
}