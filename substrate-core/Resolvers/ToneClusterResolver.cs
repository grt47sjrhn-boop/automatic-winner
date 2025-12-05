using System;
using System.Collections.Generic;
using System.Linq;
using substrate_core.Utilities;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.Registries;
using substrate_shared.Registries.Lookups;
using substrate_shared.types.models;
using substrate_shared.types.models.Maps;
using substrate_shared.types.structs;
using substrate_shared.types.Summaries;

namespace substrate_core.Resolvers
{
    public class ToneClusterResolver : IResolver
    {
        #region Public API
        public ResolutionResult Resolve(VectorBias vb, Mood mv)
        {
            #region Pre-requisites
            var delta = vb.GetSummary<DeltaSummary>();
            var persistence = vb.GetSummary<PersistenceSummary>();

            if (delta == null)
                throw new InvalidOperationException("DeltaSummary must be resolved before ToneClusterResolver.");
            if (persistence == null)
                throw new InvalidOperationException("PersistenceSummary must be resolved before ToneClusterResolver.");
            #endregion

            var traces = new List<string>();

            #region Build bias map
            var map = ToneRegistry.PopulateBiasMap(
                delta.AngleTheta,
                delta.DeltaAxis,
                persistence.Current,
                persistence.ErosionFactor,
                persistence.Direction
            );

            traces.Add($"[BiasMap] θ={delta.AngleTheta:F2}, Δ={delta.DeltaAxis:F2}, " +
                       $"Persistence={persistence.Current:F2}, Erosion={persistence.ErosionFactor:F2}, " +
                       $"Direction={persistence.Direction:+0;-0;0}");
            #endregion

            #region Baseline resolution (single tone)
            var baseline = ToneRegistryLookups.ResolveFromAngle(delta.AngleTheta);
            var baselineAffinity = ToneRegistryLookups.ResolveAffinityFromCategory(baseline.Category);
            traces.Add($"[Baseline] {baseline.Label} ({baseline.Category}, {baseline.BiasValue})");

            // Use bridge lookup to ensure baseline tones are seeded
            var baselineTones = ToneRegistry.GetTonesByCategory(baseline.Category);
            if (baselineTones.Count == 0)
            {
                traces.Add($"[Warn] No seeded tones for baseline category {baseline.Category}. Falling back.");
                baselineTones = ToneRegistry.GetTonesByCategory("Neutral");
            }
            #endregion

            #region Candidate grouping (no blending; structured palette for BlendResolver)
            const float epsilon = 0.01f;

            var sorted = map.GroupBiases
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => (Category: kvp.Key, Weight: kvp.Value))
                .ToList();

            traces.Add("[Distribution] " + string.Join(", ", sorted.Select(s => $"{s.Category}:{s.Weight:F2}")));

            var candidates = new List<(TraitAffinity Affinity, NarrativeTone Tone)>
            {
                (baselineAffinity, baseline) // 1) Baseline
            };

            // Helper: sample a tone for a given category with guards and bridge lookup
            NarrativeTone? SampleToneForCategory(string category, float weight)
            {
                if (weight <= epsilon) return null;

                // Try bridge lookup first
                var tones = ToneRegistry.GetTonesByCategory(category);
                if (tones.Count > 0)
                {
                    var picked = ToneRegistryLookups.SelectFromListWeighted(tones, category);
                    return true ? picked :
                        // Fallback: first in list if weighted selection fails
                        tones.FirstOrDefault();
                }

                // Last resort: strict resolve (may return Neutral-style)
                traces.Add($"[Warn] No tones found via bridge for {category} (w={weight:F2}). Using strict resolve.");
                var strict = ToneRegistryLookups.ResolveFromCategory(category);
                return strict;
            }

            // 2) Top 2 adjacents (highest non-zero weights excluding baseline category, dedup)
            var topAdjacents = sorted
                .Where(e => e.Weight > epsilon &&
                            !string.Equals(e.Category, baseline.Category, StringComparison.OrdinalIgnoreCase))
                .Take(2)
                .ToList();

            foreach (var entry in topAdjacents)
            {
                var tone = SampleToneForCategory(entry.Category, entry.Weight);
                if (tone == null) continue;

                var aff = ToneRegistryLookups.ResolveAffinityFromCategory(tone.Category);
                candidates.Add((aff, tone));
                traces.Add($"[Adjacent] {entry.Category} → {tone.Label} ({tone.Category}, {tone.BiasValue}) w={entry.Weight:F2}");
            }

            // 3) Midpoint: from remaining non-zero entries not used above/below
            var chosenCategories = new HashSet<string>(candidates.Select(c => c.Tone.Category),
                StringComparer.OrdinalIgnoreCase);

            var remaining = sorted
                .Where(e => e.Weight > epsilon && !chosenCategories.Contains(e.Category))
                .ToList();

            if (remaining.Count > 0)
            {
                var midIndex = remaining.Count / 2;
                var midEntry = remaining[midIndex];
                var midTone = SampleToneForCategory(midEntry.Category, midEntry.Weight);
                if (midTone != null)
                {
                    var midAff = ToneRegistryLookups.ResolveAffinityFromCategory(midTone.Category);
                    candidates.Add((midAff, midTone));
                    traces.Add($"[Midpoint] {midEntry.Category} → {midTone.Label} ({midTone.Category}, {midTone.BiasValue}) w={midEntry.Weight:F2}");
                    chosenCategories.Add(midEntry.Category);
                }
            }
            else
            {
                traces.Add("[Midpoint] Skipped (no remaining non-zero categories).");
            }

            // 4) Bottom 2: lowest non-zero weights not yet chosen
            var bottomTwo = sorted
                .Where(e => e.Weight > epsilon && !chosenCategories.Contains(e.Category))
                .Reverse()
                .Take(2)
                .ToList();

            foreach (var entry in bottomTwo)
            {
                var tone = SampleToneForCategory(entry.Category, entry.Weight);
                if (tone == null) continue;

                var aff = ToneRegistryLookups.ResolveAffinityFromCategory(tone.Category);
                candidates.Add((aff, tone));
                traces.Add($"[Complementary] {entry.Category} → {tone.Label} ({tone.Category}, {tone.BiasValue}) w={entry.Weight:F2}");
            }

            // De-duplicate by (Category:Label) while preserving order
            candidates = candidates
                .GroupBy(t => $"{t.Tone.Category}:{t.Tone.Label}", StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();

            // Enforce target size: 1 baseline + 5 = 6
            if (candidates.Count > 6)
            {
                candidates = candidates.Take(6).ToList();
                traces.Add("[Trim] Candidates trimmed to baseline + 5 grouped entries.");
            }

            traces.Add("[Palette] " + string.Join(", ",
                candidates.Select(c => $"{c.Tone.Label}({c.Tone.Category},{c.Affinity},{c.Tone.BiasValue})")));
            #endregion

            #region Build summary
            var summary = new ToneClusterSummary
            {
                Baseline = baseline,
                BaseLineTones = candidates,
                TraceLogs = traces,
                TickId = vb.TickId
            };
            #endregion

            #region Attach & return
            vb.AddSummary(summary);
            DebugOverlay.LogResolver(nameof(ToneClusterResolver), vb);
            return new ResolutionResult(vb);
            #endregion
        }
        #endregion
    }
}