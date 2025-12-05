using System;
using System.Collections.Generic;
using System.Linq;

namespace substrate_shared.Registries.Tools
{
    internal static class ToneRegistryDiagnostics
    {
        // Explicit opposites map. Extend this when adding new categories.
        private static readonly Dictionary<string, string> Opposites = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Neutral"]     = "Intensity",
            ["Intensity"]   = "Neutral",
            ["Joy"]         = "Despair",
            ["Despair"]     = "Joy",
            ["Calm"]        = "Anxiety",
            ["Anxiety"]     = "Calm",
            ["Light"]       = "Darkness",
            ["Darkness"]    = "Light",
            ["Resonance"]   = "Hostility",
            ["Hostility"]   = "Resonance",

            // Overlay categories
            ["Confidence"]  = "Despair",
            ["Love"]        = "Hostility",
            ["Wonder"]      = "Darkness",

            // Legacy archetypes
            ["Scar"]        = "Harmony",
            ["Harmony"]     = "Scar",
            ["Fracture"]    = "Equilibrium",
            ["Equilibrium"] = "Fracture",
            
            // Undertones
            ["Irony"]     = "Sincerity",   // or Confidence
            ["Volatility"] = "Stability",  // or Calm
            ["Duality"]   = "Singularity"  // or Neutral
        };

        public static string ResolvePersistence(string candidate, string resolved)
        {
            if (resolved.Equals("Neutral", StringComparison.OrdinalIgnoreCase) &&
                !candidate.Equals("Neutral", StringComparison.OrdinalIgnoreCase))
            {
                return GetOpposite(candidate); // always nudge to opposite
            }
            return resolved;
        }
        
        public static void LogCandidateResolution(string kind, string candidate, string resolved)
        {
            var final = ResolvePersistence(candidate, resolved);

            if (!final.Equals(resolved, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"[OK] Candidate {kind} resolved to {resolved} → nudged to {final}");
            }
            else if (final.Equals("Neutral", StringComparison.OrdinalIgnoreCase) &&
                     !candidate.Equals("Neutral", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"[WARN] Candidate {kind} collapsed to Neutral");
            }
            else
            {
                Console.WriteLine($"[OK] Candidate {kind} resolved to {final}");
            }
        }

        public static string GetOpposite(string category) =>
            Opposites.TryGetValue(category, out var opp) ? opp : "Neutral";

        public static void ValidateOpposites(List<(float min, float max, string category)> angularCategories)
        {
            Console.WriteLine("=== ToneRegistry Diagnostics ===");

            var midpoints = angularCategories
                .Select(c => (Mid: NormalizeToPiRange((c.min + c.max) / 2f), Category: c.category))
                .ToList();

            const float tolerance = 0.1f; // ~6 degrees

            foreach (var src in midpoints)
            {
                // First check explicit dictionary
                if (Opposites.TryGetValue(src.Category, out var explicitOpp))
                {
                    Console.WriteLine($"[OK] {src.Category}  {explicitOpp}");
                    continue;
                }

                // Fallback: geometric validation
                var oppositeMid = NormalizeToPiRange(src.Mid + MathF.PI);

                var match = midpoints
                    .OrderBy(m => Math.Abs(NormalizeToPiRange(m.Mid - oppositeMid)))
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(match.Category) ||
                    Math.Abs(NormalizeToPiRange(match.Mid - oppositeMid)) > tolerance)
                {
                    Console.WriteLine($"[WARN] {src.Category} has no opposite near {oppositeMid:F2} rad");
                }
                else
                {
                    Console.WriteLine($"[OK] {src.Category}  {match.Category}");
                }
            }
        }

        private static float NormalizeToPiRange(float angle)
        {
            while (angle <= -MathF.PI) angle += 2 * MathF.PI;
            while (angle > MathF.PI) angle -= 2 * MathF.PI;
            return angle;
        }
    }
}