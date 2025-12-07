namespace substrate_shared.Traits.Enums
{
    public enum CrystalType
    {
        Resilience, // Forged when resilience index crosses positive thresholds
        Collapse,   // Forged when resilience index crosses negative thresholds
        Fusion      // Ultra rare blend of resilience + collapse facets
    }
}