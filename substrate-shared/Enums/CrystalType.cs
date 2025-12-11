namespace substrate_shared.Enums
{
    public enum CrystalType
    {
        Resilience, // Forged when resilience index crosses positive thresholds
        Collapse,   // Forged when resilience index crosses negative thresholds
        Fusion,      // Ultra rare blend of resilience + collapse facets
        
        Equilibrium,  // Forged when duels resolve into balance/stasis
        Conflict,     // Forged when duels lock into unresolved tension
        Wound,        // Forged when duelist suffers injury outcomes
        Recovery      // Forged when duelist achieves recovery outcomes

    }
}