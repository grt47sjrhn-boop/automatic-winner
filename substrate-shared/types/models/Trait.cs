using System.Collections.Generic;

namespace substrate_shared.types.models
{
    public class Trait
    {
        public TraitId Id;                // Enum identifier
        public float Weight;              // Influence strength
        public int Age;                   // Cycles since crystallization
        public TraitState State;          // Dormant, Active, Dominant
        public float OrientationTilt;     // Preferred drift direction
        public HashSet<TraitTag> Tags;    // Synergy hooks
        public bool IsCrystallized;       // Bias that completed all cycles
        public string Label { get; set; }
    }

    public enum TraitId { ResilientHarmony, FracturedLegacy, IronyUndertone, VolatileSpread, Duality }
    public enum TraitState { Dormant, Active, Dominant }
    public enum TraitTag { Depth, Spread, Volatility, Reinforcement, Duality }
}