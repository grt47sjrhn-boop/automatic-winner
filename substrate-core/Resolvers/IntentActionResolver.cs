using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;
using substrate_shared.types.Summaries;

namespace substrate_core.Resolvers
{
    public class IntentActionResolver : IResolver
    {
        public ResolutionResult Resolve(VectorBias vb, Mood mv)
        {
            // Pull summaries instead of raw values
            var toneCluster  = vb.GetSummary<ToneClusterSummary>();
            var persistence  = vb.GetSummary<PersistenceSummary>();
            var volatility   = vb.GetSummary<VolatilitySummary>();
            var delta        = vb.GetSummary<DeltaSummary>();
            var traitSummary = vb.GetSummary<TraitSummary>();

            if (toneCluster == null || persistence == null || volatility == null || delta == null)
                return new ResolutionResult(vb);

            var traceLogs = new List<string>();

            // Choose tone: prefer first candidate in BaseLineTones, fallback to Baseline
            var toneTuple = toneCluster.BaseLineTones.FirstOrDefault();
            var tone      = toneTuple.Tone ?? toneCluster.Baseline;
            var category  = tone?.Category ?? "Neutral";

            var persistenceVal = persistence.Current;
            var volatilityVal  = volatility.Volatility;
            var area           = delta.Area;
            var hasDuality     = traitSummary?.ActiveTraitLabels.Any(label => label != null && label.Contains("Duality")) ?? false;

            // Default intent
            var intent = IntentType.None;

            // Intent resolution logic (category-based)
            if (category.Equals("Resonance", StringComparison.OrdinalIgnoreCase) && persistenceVal > 2.5f)
            {
                intent = IntentType.Stabilize;
                traceLogs.Add("Resonance with strong persistence → Stabilize intent.");
            }
            else if (category.Equals("Scar", StringComparison.OrdinalIgnoreCase) && volatilityVal > 1.2f)
            {
                intent = IntentType.Disrupt;
                traceLogs.Add("Scar tone with high volatility → Disrupt intent.");
            }
            else if (category.Equals("Neutral", StringComparison.OrdinalIgnoreCase) && hasDuality)
            {
                intent = IntentType.Reflect;
                traceLogs.Add("Neutral tone with duality trait → Reflect intent.");
            }
            else if (category.Equals("Harmony", StringComparison.OrdinalIgnoreCase) && area > 10f)
            {
                intent = IntentType.Amplify;
                traceLogs.Add("Harmony tone with large area → Amplify intent.");
            }
            else if (category.Equals("Equilibrium", StringComparison.OrdinalIgnoreCase) && persistenceVal > 5f && volatilityVal < 1f)
            {
                intent = IntentType.Creation;
                traceLogs.Add("Equilibrium with high persistence and low volatility → Creation intent.");
            }
            else if (category.Equals("Fracture", StringComparison.OrdinalIgnoreCase) && persistenceVal < -5f && volatilityVal > 2f)
            {
                intent = IntentType.Destruction;
                traceLogs.Add("Fracture with low persistence and high volatility → Destruction intent.");
            }
            else if (category.Equals("Resonance", StringComparison.OrdinalIgnoreCase) && volatilityVal > 3f)
            {
                intent = IntentType.Transformation;
                traceLogs.Add("Resonance under extreme volatility → Transformation intent.");
            }
            else
            {
                traceLogs.Add("No matching conditions → Intent remains None.");
            }

            // Build IntentActionSummary
            var summary = new IntentActionSummary
            {
                TickId      = vb.TickId,
                Tone        = tone,
                Persistence = persistenceVal,
                Volatility  = volatilityVal,
                Area        = area,
                HasDuality  = hasDuality,
                Intent      = intent,
                TraceLogs   = traceLogs
            };

            vb.AddSummary(summary);
            return new ResolutionResult(vb);
        }
    }
}