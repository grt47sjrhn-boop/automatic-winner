using System;
using System.Collections.Generic;
using System.Linq;
using substrate_core.Utilities;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;
using substrate_shared.types.Summaries;

namespace substrate_core.Resolvers
{
    /// <summary>
    /// Resolves trigger events (crystallization, fragmentation, trait activations, constellation formation).
    /// Emits contributor-facing TriggerSummary with narratable tilt derived from ToneClusterSummary.
    /// </summary>
    public class TriggerResolver : IResolver
    {
        // Tunables (lifted out for clarity and future calibration)
        private const float CrystalThreshold    = 50f;   // min |score| to consider crystallization
        private const float FragmentThreshold   = -50f;  // min negative score to consider fragmentation
        private const int   ConstellationMin    = 3;     // min crystallized traits to form constellation

        public ResolutionResult Resolve(VectorBias vb, Mood mv)
        {
            var tickId = vb.TickId;

            // Pull summaries (safe)
            var delta       = vb.GetSummary<DeltaSummary>();
            var traits      = vb.GetSummary<TraitSummary>();
            var persistence = vb.GetSummary<PersistenceSummary>();
            var volatility  = vb.GetSummary<VolatilitySummary>();
            var cluster     = vb.GetSummary<ToneClusterSummary>(); // provides baseline + palette

            // Safe floats
            float hyp          = DebugOverlay.SafeFloat(delta.Hypotenuse);
            float area         = DebugOverlay.SafeFloat(delta.Area);
            float persistenceVal = persistence?.Current   ?? 0f;
            float volatilityVal  = volatility?.Volatility ?? 0f;

            // Coherence score
            float crystallizationScore = hyp * area;

            // Derive narratable tilt from cluster baseline (fallback to vb.Legacy, then None)
            string tiltCategory = ResolveTiltCategory(cluster, vb);

            var events = new List<TriggerEvent>();

            // Crystallization attempt
            if (crystallizationScore >= CrystalThreshold)
            {
                events.Add(new TriggerEvent
                {
                    Type        = TriggerType.CrystallizationAttempt,
                    Description = $"Crystal formed (score={crystallizationScore:F2}), bias tilted toward {tiltCategory}",
                    Magnitude   = hyp,
                    Score       = crystallizationScore,
                    TickId      = tickId,
                    Persistence = persistenceVal,
                    Volatility  = volatilityVal
                });
            }
            // Fragmentation attempt
            else if (crystallizationScore <= FragmentThreshold)
            {
                events.Add(new TriggerEvent
                {
                    Type        = TriggerType.FragmentationAttempt,
                    Description = $"Fragmentation surge (score={crystallizationScore:F2}), legacy stressed toward {tiltCategory}",
                    Magnitude   = hyp,
                    Score       = crystallizationScore,
                    TickId      = tickId,
                    Persistence = persistenceVal,
                    Volatility  = volatilityVal
                });
            }

            // Trait activations
            if (traits?.ActiveTraitIds != null && traits.ActiveTraitIds.Count > 0)
            {
                foreach (var id in traits.ActiveTraitIds)
                {
                    events.Add(new TriggerEvent
                    {
                        Type        = TriggerType.TraitActivation,
                        Description = $"{id} awakened",
                        Magnitude   = 1f,
                        Score       = 1f,
                        TickId      = tickId,
                        Persistence = persistenceVal,
                        Volatility  = volatilityVal
                    });
                }
            }

            // Constellation formation
            if (traits != null && traits.CrystallizedCount >= ConstellationMin)
            {
                events.Add(new TriggerEvent
                {
                    Type        = TriggerType.ConstellationFormation,
                    Description = $"Constellation formed with {traits.CrystallizedCount} crystallized traits, weaving mythic resonance",
                    Magnitude   = traits.CrystallizedCount,
                    Score       = traits.CrystallizedCount,
                    TickId      = tickId,
                    Persistence = persistenceVal,
                    Volatility  = volatilityVal
                });
            }

            // Build summary
            var summary = new TriggerSummary
            {
                TickId = tickId,
                Events = events,
                Count  = events.Count
            };

            DebugOverlay.LogTrigger(vb, crystallizationScore, delta, summary);

            vb.AddSummary(summary);
            return new ResolutionResult(vb);
        }

        // Resolve narratable tilt:
        // 1) Use cluster baseline category if present
        // 2) Else use top category from distribution (highest weight)
        // 3) Else fallback to vb.Legacy
        // 4) Else "None"
        private static string ResolveTiltCategory(ToneClusterSummary cluster, VectorBias vb)
        {
            if (!string.IsNullOrWhiteSpace(cluster?.Baseline?.Category))
                return cluster.Baseline.Category;

            if (!string.IsNullOrWhiteSpace(vb.Legacy.ToString()))
                return vb.Legacy.ToString();

            return "None";
        }

    }
}