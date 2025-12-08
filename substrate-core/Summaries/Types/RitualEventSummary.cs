using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.Models;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.Summaries.Base;
using substrate_shared.types;

namespace substrate_core.Summaries.Types
{
    public class RitualEventSummary : EventSummary
    {
        public IReadOnlyList<BiasVector> Participants { get; }
        public FacetDistribution CollectiveShape { get; }
        public BiasDescriptor CollectiveBias { get; }
        public ToneCut CollectiveBrilliance { get; }
        public RarityTier RitualRarity { get; }

        public RitualEventSummary(
            string title,
            string description,
            SummaryType type,
            IEnumerable<BiasVector> participants,
            FacetDistribution collectiveShape,
            BiasDescriptor collectiveBias,
            ToneCut collectiveBrilliance,
            RarityTier ritualRarity,
            bool isResolved = false)
            : base(title, description, type, isResolved)
        {
            Participants = participants.ToList();
            CollectiveShape = collectiveShape;
            CollectiveBias = collectiveBias;
            CollectiveBrilliance = collectiveBrilliance;
            RitualRarity = ritualRarity;
        }

        public override void Print()
        {
            var resolvedText = Type == SummaryType.Ritual ? $" (Resolved: {IsResolved})" : string.Empty;

            Console.WriteLine($"{Title} - {Description} [{Type}]");
            Console.WriteLine($"Participants: {string.Join(", ", Participants.Select(p => p.Tone.Label))}");
            Console.WriteLine($"Collective Bias: {CollectiveBias}, Brilliance: {CollectiveBrilliance.Primary}, Rarity: {RitualRarity}{resolvedText}");
        }
    }
}