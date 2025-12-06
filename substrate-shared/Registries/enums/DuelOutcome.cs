namespace substrate_shared.Registries.enums
{
    public enum DuelOutcome
    {
        Collapse,        // Negative dominates
        Recovery,        // Positive dominates
        Equilibrium,     // Neutral or balanced
        MixedConflict,   // Positive and Negative with near parity
        Wound            // Persistent consequence beyond the duel
    }
}