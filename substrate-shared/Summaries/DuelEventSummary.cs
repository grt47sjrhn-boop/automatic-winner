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

        public override BiasVector ResolvedVector { get; protected set; }
        public override DuelOutcome Outcome { get; protected set; }
        public override ToneCut Brilliance { get; protected set; }
        public override FacetDistribution Shape { get; protected set; }

        public BiasDescriptor Bias { get; }
        public MoodType ResolvedMood { get; }
        public IntentAction ResolvedIntent { get; }
        public CrystalRarity ResolvedRarity { get; }
    
        // 🔹 New resilience fields
        public double CumulativeResilience { get; set; }
    
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
            MoodType resolvedMood,
            IntentAction resolvedIntent,
            CrystalRarity resolvedRarity,
            FacetDistribution shape,
            double resilienceIndex,
            double cumulativeResilience,
            bool isResolved = false)
            : base(title, description, type, isResolved)
        {
            DuelistA = duelistA;
            DuelistB = duelistB;
            ResolvedVector = resolvedVector;
            Outcome = outcome;
            Brilliance = brilliance;
            Bias = bias;
            ResolvedMood = resolvedMood;
            ResolvedIntent = resolvedIntent;
            ResolvedRarity = resolvedRarity;
            Shape = shape;
            // 🔹 Assign resilience values
            ResilienceIndex = resilienceIndex;
            CumulativeResilience = cumulativeResilience;

        }
    }
}