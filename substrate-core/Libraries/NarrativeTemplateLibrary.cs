using System;
using System.Collections.Generic;
using substrate_shared.enums;
using substrate_shared.types.structs;

namespace substrate_core.Libraries;

public static class NarrativeTemplateLibrary
{
    private static readonly Dictionary<Tone, List<string>> ToneTemplates = new()
    {
        { Tone.Equilibrium, new List<string> { "Equilibrium steadied the axis, balance held firm..." } },
        { Tone.Harmony,     new List<string> { "Harmony shimmered across the bias, traits aligned..." } },
        { Tone.Resonance,   new List<string> { "Resonance echoed, persistence amplified into mythic chords..." } },
        { Tone.Scar,        new List<string> { "Scar pressed faintly, legacy tilt carved into the axis..." } },
        { Tone.Fracture,    new List<string> { "Fracture surged, volatility tore the bias apart..." } }
    };

    private static readonly Dictionary<IntentType, List<string>> IntentTemplates = new()
    {
        { IntentType.Stabilize, new List<string> { "Stability shimmered, persistence anchored resilience..." } },
        { IntentType.Disrupt,   new List<string> { "Disruption fractured the balance, scars deepened..." } },
        { IntentType.Reflect,   new List<string> { "Reflection shimmered, duality revealed hidden truths..." } },
        { IntentType.Amplify,   new List<string> { "Amplification surged, harmony magnified into mythic chords..." } },
        { IntentType.Creation,  new List<string> { "Creation shimmered into being, equilibrium birthed new form..." } },
        { IntentType.Destruction,new List<string>{ "Destruction tore legacy apart, fracture collapsed the axis..." } },
        { IntentType.Transformation,new List<string>{ "Transformation surged, resonance mutated under volatility..." } }
    };

    public static string GetTemplate(ToneTuple toneTuple, int tickId)
    {
        if (!ToneTemplates.ContainsKey(toneTuple.Primary))
            return $"[Tick {tickId}] No narrative template defined for tone {toneTuple.Primary}.";

        var options = ToneTemplates[toneTuple.Primary];
        var random = new Random();
        return $"[Tick {tickId}] {options[random.Next(options.Count)]}";
    }

    public static string GetTemplate(IntentType intent, int tickId)
    {
        if (!IntentTemplates.ContainsKey(intent))
            return $"[Tick {tickId}] No narrative template defined for intent {intent}.";

        var options = IntentTemplates[intent];
        var random = new Random();
        return $"[Tick {tickId}] {options[random.Next(options.Count)]}";
    }
}