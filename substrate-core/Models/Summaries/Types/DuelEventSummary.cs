using substrate_core.Models.Summaries.Base;
using substrate_shared.interfaces.core;
using substrate_shared.interfaces.Details;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.types;

namespace substrate_core.Models.Summaries.Types
{
    public class DuelEventSummary : EventSummary, IDuelEventSummary
    {
        public BiasVector DuelistA { get; }
        public BiasVector DuelistB { get; }

        public override BiasVector ResolvedVector { get; protected set; }
        public override DuelOutcome Outcome { get; protected set; }
        public override IToneCut Brilliance { get; protected set; }
        public override FacetDistribution Shape { get; protected set; }

        public BiasDescriptor Bias { get; }
        public MoodType ResolvedMood { get; }
        public IntentAction ResolvedIntent { get; }
        public CrystalRarity ResolvedRarity { get; }
    
        // 🔹 New resilience fields
        public double CumulativeResilience { get; set; }
        public int Tick { get; }

        public DuelEventSummary(
            string title,
            string description,
            SummaryType type,
            BiasVector duelistA,
            BiasVector duelistB,
            BiasVector resolvedVector,
            DuelOutcome outcome,
            IToneCut brilliance,
            BiasDescriptor bias,
            MoodType resolvedMood,
            IntentAction resolvedIntent,
            CrystalRarity resolvedRarity,
            FacetDistribution shape,
            double resilienceIndex,
            double cumulativeResilience,
            int tick,
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
            Tick = tick;

        }
    }
}