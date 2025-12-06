using System.Collections.Generic;
using substrate_shared.structs;

namespace substrate_shared.interfaces
{
    public interface IResilienceTracker
    {
        // Existing narrative-facing properties
        IReadOnlyList<ISummary> DuelSummaries { get; }
        int ResilienceIndex { get; }

        // Existing methods
        public ISummary Record(ISummary summary, BiasVector? a = null, BiasVector? b = null);
        ISummary ComputeResilience();

        // 🔹 New math overlay properties
        double AverageHypotenuse { get; }
        double CumulativeArea { get; }
        double MeanCos { get; }
        double MeanSin { get; }
        double LogScaledIndex { get; }
        double ExpScaledIndex { get; }
    }

}