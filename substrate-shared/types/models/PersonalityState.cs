namespace substrate_shared.types.models
{
    public enum PersonalityState
    {
        Neutral,
        Fracturing,    // persistence decreasing, wound forming
        Recovering,    // persistence increasing, healing trajectory
        SettlingScar,  // wound unresolved, hardened into scar
        Resonating,    // recovery stabilized into resonance
        Hardened       // recovery or fracture completed, bias locked in
    }
}