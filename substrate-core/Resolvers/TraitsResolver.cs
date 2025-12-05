using System.Collections.Generic;
using System.Linq;
using substrate_core.Utilities;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;
using substrate_shared.types.Summaries;

namespace substrate_core.Resolvers
{
    /// <summary>
    /// Resolves trait state for the current tick:
    /// - Increments Age for crystallized traits
    /// - Updates State based on weight thresholds
    /// - Captures active/crystallized traits for contributor-facing narratability
    /// Produces TraitSummary for downstream resolvers.
    /// </summary>
    public class TraitsResolver : IResolver
    {
        // Internal list of traits managed by this resolver
        private readonly List<Trait> _traits = [];

        /// <summary>
        /// Add a new trait to the resolver (e.g. during initialization or contributor input).
        /// </summary>
        public void AddTrait(Trait trait)
        {
            _traits.Add(trait);
        }

        /// <summary>
        /// Resolve trait lifecycle and produce TraitSummary.
        /// </summary>
        public ResolutionResult Resolve(VectorBias vb, Mood mv)
        {
            var tickId = vb.TickId;

            foreach (var trait in _traits)
            {
                var w = DebugOverlay.SafeFloat(trait.Weight);

                // Increment age if crystallized
                if (trait.IsCrystallized)
                    trait.Age++;

                trait.IsCrystallized = w switch
                {
                    // Update crystallization flag
                    > 0.7f when !trait.IsCrystallized => true,
                    < 0.3f when trait.IsCrystallized => false,
                    _ => trait.IsCrystallized
                };

                trait.State = w switch
                {
                    // Update state based on weight
                    < 0.3f => TraitState.Dormant,
                    < 0.7f => TraitState.Active,
                    _ => TraitState.Dominant
                };
            }

            var activeTraits       = _traits.Where(t => t.State == TraitState.Active).ToList();
            var crystallizedTraits = _traits.Where(t => t.IsCrystallized).ToList();

            var summary = new TraitSummary
            {
                TickId               = tickId,
                TotalTraits          = _traits.Count,
                ActiveCount          = activeTraits.Count,
                CrystallizedCount    = crystallizedTraits.Count,
                ActiveTraitIds       = activeTraits.Select(t => t.Id.ToString()).ToList(),
                CrystallizedTraitIds = crystallizedTraits.Select(t => t.Id.ToString()).ToList(),
                ActiveTraitLabels    = activeTraits.Select(t => t.Label).ToList(),
                CrystallizedTraitLabels = crystallizedTraits.Select(t => t.Label).ToList(),
                LifecycleNotes       = _traits.Select(t => $"{t.Id}: State={t.State}, Age={t.Age}").ToList()
            };

            vb.AddSummary(summary);
            return new ResolutionResult(vb);
        }
    }
}