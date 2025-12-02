using System;

namespace substrate_shared.enums
{
    public enum TraitAffinity
    {
        None,
        ResilientHarmony,
        FracturedLegacy,
        Equilibrium,
        Memory,
        Inertia,
        Absorption
    }

    public static class TraitAffinityExtensions
    {
        public static string Describe(this TraitAffinity affinity)
        {
            return affinity switch
            {
                TraitAffinity.None              => "No special affinity; trait remains neutral.",
                TraitAffinity.ResilientHarmony  => "Resists fracture, stabilizes clusters, and promotes cohesion.",
                TraitAffinity.FracturedLegacy   => "Carries scars from past states, influencing persistence and bias.",
                TraitAffinity.Equilibrium       => "Seeks balance, dampening extremes and restoring neutrality.",
                TraitAffinity.Memory            => "Retains echoes of prior ticks, shaping continuity and recall.",
                TraitAffinity.Inertia           => "Maintains momentum, resisting sudden shifts in tone or bias.",
                TraitAffinity.Absorption        => "Draws in external influences, blending them into the current state.",
                _                               => "Unknown affinity."
            };
        }
    }
}