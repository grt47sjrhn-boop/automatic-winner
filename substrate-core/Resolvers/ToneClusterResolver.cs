using System;
using System.Collections.Generic;
using System.Linq;
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
            traces.Add($"[3] Baseline tone from angle → {baseline.Label} ({baseline.Category}, {baseline.BiasValue})");

            // --- 4) Final tone from the strongest group ---
            var strongestGroup = map.GetStrongestGroup();
            var finalTone = ToneRegistry.SelectWeighted(strongestGroup);
            traces.Add($"[4] Strongest group={strongestGroup}, final tone={finalTone.Label} ({finalTone.Category}, {finalTone.BiasValue})");

            // Neighborhoods based on enum key where needed
            var resolvedEnumForNeighborhood = MapCategoryToEnum(finalTone.Category);
            var adjacent = ToneRegistry.GetAdjacentByTone(resolvedEnumForNeighborhood);
            var neighborhood = ToneRegistry.GetNeighborhoodByTone(resolvedEnumForNeighborhood);
            var complementary = ToneRegistry.GetComplementNeighborhood(resolvedEnumForNeighborhood);

            // Affinity
            var affinity = ToneRegistry.ResolveAffinityFromCategory(category);

            // --- 5) Build summary ---
            var summary = new ToneClusterSummary
            {
                Baseline          = baseline,
                Blended           = baseline, // axis blending optional now; keep same for now
                Complement        = null,     // can compute if you want: pick from complementary by weight
                BiasAdjusted      = finalTone,
                FinalTone         = finalTone,
                ClusterWeights    = ToneRegistry.CurrentWeights(),
                AngularCategories = ToneRegistry.GetAngularCategories(),
                TraceLogs         = traces,
                TickId            = vb.TickId,
                AdjacentTones     = adjacent,
                ClusterNeighborhood = neighborhood,
                ComplementaryTones = complementary,
                Category          = category,
                ResolvedAffinity  = affinity
            };

            summary.TraceLogs.Add($"[5] Adjacent tones → {string.Join(", ", summary.AdjacentTones.Select(t => t.Label))}");
            summary.TraceLogs.Add($"[6] Cluster neighborhood → {string.Join(", ", summary.ClusterNeighborhood.Select(t => t.Label))}");
            summary.TraceLogs.Add($"[7] Complementary tones → {string.Join(", ", summary.ComplementaryTones.Select(t => t.Label))}");

            vb.AddSummary(summary);
            DebugOverlay.LogResolver(nameof(ToneClusterResolver), vb);
            return new ResolutionResult(vb);
        }

        // Map category string to a canonical Tone enum for neighborhood lookups
        private static Tone MapCategoryToEnum(string category)
        {
            // Minimal mapping: pick a representative enum in that category
            // You can expand this map or derive from your registry to find the canonical representative
            return category?.ToLowerInvariant() switch
            {
                "despair"   => Tone.Sad,
                "hostility" => Tone.Angry,
                "darkness"  => Tone.Somber,
                "neutral"   => Tone.Neutral,
                "anxiety"   => Tone.Tense,
                "resonance" => Tone.Nostalgic, // neutral-leaning resonance bucket
                "joy"       => Tone.Joyful,
                _           => Tone.Neutral
            };
        }
    }
}