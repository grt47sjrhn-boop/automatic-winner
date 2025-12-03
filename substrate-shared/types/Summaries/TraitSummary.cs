using System.Collections.Generic;
using substrate_shared.interfaces;
using substrate_shared.types.models;

namespace substrate_shared.types.Summaries
{
    /// <summary>
    /// Contributor-facing summary of trait state for a tick.
    /// </summary>
    public class TraitSummary : ISummary
    {
        public string Name => nameof(TraitSummary);

        public int TickId { get; set; }
        public int TotalTraits { get; set; }
        public int ActiveCount { get; set; }
        public int CrystallizedCount { get; set; }

        // Contributor-facing IDs and labels
        public List<string> ActiveTraitIds { get; set; } = new();
        public List<string> CrystallizedTraitIds { get; set; } = new();
        public List<string> ActiveTraitLabels { get; set; } = new();
        public List<string> CrystallizedTraitLabels { get; set; } = new();

        // Lifecycle notes: state + age per trait
        public List<string> LifecycleNotes { get; set; } = new();

        // Full trait objects for weight tracking and metrics
        public List<Trait> Traits { get; set; } = new();

        public string Describe()
        {
            return $"[TraitSummary] Tick={TickId}, Total={TotalTraits}, Active={ActiveCount}, " +
                   $"Crystallized={CrystallizedCount}, Active=[{string.Join(", ", ActiveTraitLabels)}], " +
                   $"Crystallized=[{string.Join(", ", CrystallizedTraitLabels)}], " +
                   $"Lifecycle=[{string.Join("; ", LifecycleNotes)}]";
        }
    }
}