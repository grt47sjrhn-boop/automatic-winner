using System;
using System.Collections.Generic;
using substrate_core.Utilities;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.Registries;
using substrate_shared.types.models;
using substrate_shared.types.models.Maps;
using substrate_shared.types.structs;
using substrate_shared.types.Summaries;

namespace substrate_core.Resolvers
{
    public class ToneClusterResolver : IResolver
    {
        public ResolutionResult Resolve(VectorBias vb, Mood mv)
        {
            // --- 1) Require summaries ---
            var delta = vb.GetSummary<DeltaSummary>();
            var persistence = vb.GetSummary<PersistenceSummary>();
            if (delta == null)
                throw new InvalidOperationException("DeltaSummary must be resolved before ToneClusterResolver.");
            if (persistence == null)
                throw new InvalidOperationException("PersistenceSummary must be resolved before ToneClusterResolver.");

            var traces = new List<string>();

            // --- 2) Build bias map step by step ---
            var map = new BiasMap();

            // Angle bias
            var category = ToneRegistry.ResolveCategoryFromAngle(delta.AngleTheta);
            map.AddBias(category, 1.0f);
            traces.Add($"[1] Angle θ={delta.AngleTheta:F2} → category={category}, bias +1.0");

            // Axis bias
            ToneRegistry.ResolveAxisInfluence(map, delta.DeltaAxis);
            traces.Add($"[1b] Axis Δ={delta.DeltaAxis:F2} → axis bias applied (Confidence/Despair/Neutral)");

            // Persistence bias
            var baseStrength = Math.Clamp(persistence.Current / 10f, 0f, 1f);
            var erosion = Math.Clamp(persistence.ErosionFactor, 0f, 0.85f);
            var temperedStrength = baseStrength * (1f - erosion);
            map.AddBias(category, temperedStrength);
            traces.Add($"[2] Persistence={persistence.Current:F2}, Erosion={erosion:F2}, bias +{temperedStrength:F2} to {category}");

            // Directional bias
            if (persistence.Direction > 0)
            {
                map.AddBias("Confidence", persistence.Direction, "Positive");
                traces.Add($"[2b] Direction={persistence.Direction:+0;-0;0} → Confidence bias");
            }
            else if (persistence.Direction < 0)
            {
                map.AddBias("Despair", Math.Abs(persistence.Direction), "Negative");
                traces.Add($"[2b] Direction={persistence.Direction:+0;-0;0} → Despair bias");
            }

            // --- 3) Baseline tone from angle ---
            var baseline = ToneRegistry.ResolveFromAngle(delta.AngleTheta);
            traces.Add($"[3] Baseline tone from angle → {baseline}");

            // --- 4) Final tone from the strongest group ---
            var strongestGroup = map.GetStrongestGroup();
            var finalTone = ToneRegistry.SelectWeighted(strongestGroup);
            traces.Add($"[4] Strongest group={strongestGroup}, final tone={finalTone}");
        
        
            // --- 5) Build summary ---
            var summary = new ToneClusterSummary
            {
                Baseline       = baseline,
                Blended        = baseline, // axis blending optional now
                BiasAdjusted   = finalTone,
                FinalTone      = finalTone,
                ClusterWeights = ToneRegistry.CurrentWeights(),
                AngularCategories = ToneRegistry.GetAngularCategories(),
                TraceLogs      = traces,
                TickId         = vb.TickId,
                AdjacentTones  = ToneRegistry.GetAdjacentByTone(finalTone),
                ClusterNeighborhood = ToneRegistry.GetNeighborhoodByTone(finalTone),
                ComplementaryTones = ToneRegistry.GetComplementNeighborhood(finalTone),
                ResolvedAffinity = ToneRegistry.ResolveAffinityFromCategory(category)
            };

            summary.TraceLogs.Add($"[5] Adjacent tones → {string.Join(", ", summary.AdjacentTones)}");
            summary.TraceLogs.Add($"[6] Cluster neighborhood → {string.Join(", ", summary.ClusterNeighborhood)}");
            summary.TraceLogs.Add($"[7] Complementary tones → {string.Join(", ", summary.ComplementaryTones)}");
        
            vb.AddSummary(summary);

            DebugOverlay.LogResolver(nameof(ToneClusterResolver), vb);
            return new ResolutionResult(vb);
        }
    }
}