using substrate_shared.interfaces.Details;
using substrate_shared.Registries.enums;
using substrate_shared.structs;

namespace substrate_shared.interfaces.core
{
    public interface IDuelEventSummary : ISummary
    {
        BiasVector DuelistA { get; }
        BiasVector DuelistB { get; }

        BiasVector ResolvedVector { get; }
        DuelOutcome Outcome { get; }
        IToneCut Brilliance { get; }
        FacetDistribution Shape { get; }

        BiasDescriptor Bias { get; }
        MoodType ResolvedMood { get; }
        IntentAction ResolvedIntent { get; }
        CrystalRarity ResolvedRarity { get; }

        double ResilienceIndex { get; }
        double CumulativeResilience { get; }
        
        int Tick { get; }
    }
}