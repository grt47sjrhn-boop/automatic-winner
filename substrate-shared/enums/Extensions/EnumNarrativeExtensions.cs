namespace substrate_shared.enums.Extensions
{
    public static class EnumNarrativeExtensions
    {
        public static string GetNarrativeName(this Tone tone) => tone switch
        {
            Tone.Harmony => "Harmony",
            Tone.Resonance => "Resonance",
            Tone.Scar => "Scar",
            Tone.Fracture => "Fracture",
            Tone.Neutral => "Equilibrium", // map Neutral → Equilibrium
            _ => tone.ToString()
        };

        public static string GetNarrativeName(this TraitAffinity legacy) => legacy switch
        {
            TraitAffinity.None => "None",
            TraitAffinity.Memory => "Memory",
            TraitAffinity.Inertia => "Inertia",
            TraitAffinity.Absorption => "Absorption",
            _ => legacy.ToString()
        };

        public static string GetNarrativeName(this IntentType intent) => intent switch
        {
            IntentType.None => "None",
            IntentType.Creation => "Creation",
            IntentType.Destruction => "Destruction",
            IntentType.Transformation => "Transformation",
            _ => intent.ToString()
        };
    }
}