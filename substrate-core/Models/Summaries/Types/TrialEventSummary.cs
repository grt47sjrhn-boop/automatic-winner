using System;
using System.Collections.Generic;
using System.Linq;
using substrate_core.Models.Summaries.Base;
using substrate_shared.interfaces;
using substrate_shared.interfaces.Details;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.types;

namespace substrate_core.Models.Summaries.Types
{
    public class TrialEventSummary : EventSummary, ISummary
    {
        public IReadOnlyList<DuelEventSummary> DuelSummaries { get; }
        public IRarityTier TrialRarity { get; }
        public BiasDescriptor AggregateBias { get; }
        public ToneType DominantTone { get; }

        public TrialEventSummary(
            string title,
            string description,
            SummaryType type,
            IEnumerable<DuelEventSummary> duelSummaries,
            IRarityTier trialRarity,
            BiasDescriptor aggregateBias,
            ToneType dominantTone,
            bool isResolved = false)
            : base(title, description, type, isResolved)
        {
            DuelSummaries = duelSummaries.ToList();
            TrialRarity = trialRarity;
            AggregateBias = aggregateBias;
            DominantTone = dominantTone;
        }

        public override void Print()
        {
            var resolvedText = Type == SummaryType.Trial ? $" (Resolved: {IsResolved})" : string.Empty;

            Console.WriteLine($"{Title} - {Description} [{Type}]");
            Console.WriteLine($"Trial Rarity: {TrialRarity}, Aggregate Bias: {AggregateBias}, Dominant Tone: {DominantTone}{resolvedText}");

            foreach (var duel in DuelSummaries)
            {
                var biasLabel = duel.ResolvedVector.Tone?.Label ?? "[None]";
                Console.WriteLine($"  Duel → {duel.Title}: {duel.Outcome}, Bias: {biasLabel}");
            }
        }
    }
}