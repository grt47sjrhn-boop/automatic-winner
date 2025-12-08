using System;
using substrate_shared.Models;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.Summaries.Base;
using substrate_shared.types;

namespace substrate_shared.Summaries
{
    public class DuelEventSummary : EventSummary
    {
        public BiasVector DuelistA { get; }
        public BiasVector DuelistB { get; }
        public BiasVector ResolvedVector { get; }
        public DuelOutcome Outcome { get; }
        
        // 🔹 New enriched fields
        public ToneCut Brilliance { get; }
        public BiasDescriptor Bias { get; }

        public DuelEventSummary(
            string title,
            string description,
            SummaryType type,
            BiasVector duelistA,
            BiasVector duelistB,
            BiasVector resolvedVector,
            DuelOutcome outcome,
            ToneCut brilliance,
            BiasDescriptor bias,
            bool isResolved = false)
            : base(title, description, type, isResolved)
        {
            DuelistA = duelistA;
            DuelistB = duelistB;
            ResolvedVector = resolvedVector;
            Outcome = outcome;
            Brilliance = brilliance;
            Bias = bias;
        }

        public override void Print()
        {
            var resolvedText = Type == SummaryType.Duel ? $" (Resolved: {IsResolved})" : string.Empty;
            Console.WriteLine(
                $"{Title} - {Description} [{Type}] " +
                $"Duelists: {DuelistA.Tone.Label} vs {DuelistB.Tone.Label} " +
                $"Resolved: {ResolvedVector.Tone.Label} (Mag {ResolvedVector.Magnitude}) " +
                $"Outcome: {Outcome}{resolvedText} " +
                $"Brilliance: {Brilliance?.Primary} " +
                $"Bias: {Bias?.Bias}"
            );
        }
    }
}