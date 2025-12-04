using System;
using System.Collections.Generic;
using substrate_shared.enums;
using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_core.Libraries
{
    public static class NarrativeTemplateLibrary
    {
        private static readonly Dictionary<string, List<string>> ToneTemplates = new(StringComparer.OrdinalIgnoreCase)
        {
            // Special states
            { "Equilibrium", new List<string>
                {
                    "Equilibrium steadied the axis, balance held firm...",
                    "Equilibrium shimmered, dual forces aligned in quiet symmetry...",
                    "Equilibrium anchored persistence, volatility dissolved into calm..."
                }
            },
            { "Harmony", new List<string>
                {
                    "Harmony shimmered across the bias, traits aligned...",
                    "Harmony resonated, chords of resilience intertwined...",
                    "Harmony pulsed gently, weaving disparate tones into unity..."
                }
            },
            { "Resonance", new List<string>
                {
                    "Resonance echoed, persistence amplified into mythic chords...",
                    "Resonance surged, vibrations carried legacy forward...",
                    "Resonance shimmered, tonal arcs magnified across the cycle..."
                }
            },
            { "Scar", new List<string>
                {
                    "Scar pressed faintly, legacy tilt carved into the axis...",
                    "Scar shimmered, a reminder etched in persistence...",
                    "Scar deepened, fracture lines whispered of past volatility..."
                }
            },
            { "Fracture", new List<string>
                {
                    "Fracture surged, volatility tore the bias apart...",
                    "Fracture widened, persistence strained under pressure...",
                    "Fracture shimmered, legacy arcs splintered into shards..."
                }
            },

            // Core categories
            { "Confidence", new List<string>
                {
                    "Confidence arcs rose, resilience strengthened...",
                    "Confidence shimmered, persistence anchored in bold clarity...",
                    "Confidence surged, bias tilted toward strength and resolve..."
                }
            },
            { "Despair", new List<string>
                {
                    "Despair arcs fractured, legacy strained...",
                    "Despair deepened, persistence eroded under shadow...",
                    "Despair shimmered, bias tilted toward sorrow and collapse..."
                }
            },
            { "Calm", new List<string>
                {
                    "Calm arcs anchored, stability held...",
                    "Calm shimmered, persistence steadied into quiet balance...",
                    "Calm radiated, volatility dissolved into serenity..."
                }
            },
            { "Anxiety", new List<string>
                {
                    "Anxiety arcs surged, tension built...",
                    "Anxiety shimmered, persistence strained under unease...",
                    "Anxiety pulsed, volatility sharpened into restless arcs..."
                }
            },
            { "Light", new List<string>
                {
                    "Light arcs radiated, clarity and hope shone...",
                    "Radiance surged, illuminating persistence with brilliance...",
                    "Luminosity shimmered, bias tilted toward clarity and renewal..."
                }
            },
            { "Darkness", new List<string>
                {
                    "Darkness arcs deepened, obscurity spread...",
                    "Darkness shimmered, persistence drowned in shadow...",
                    "Darkness surged, bias tilted toward concealment and doubt..."
                }
            },
            { "Intensity", new List<string>
                {
                    "Intensity arcs flared, persistence burned hot...",
                    "Intensity surged, volatility magnified into blazing arcs...",
                    "Intensity shimmered, bias tilted toward fervor and drive..."
                }
            },
            { "Joy", new List<string>
                {
                    "Joy arcs shimmered, harmony resonated...",
                    "Joy radiated, persistence lifted into celebration...",
                    "Joy pulsed brightly, bias tilted toward delight and unity..."
                }
            },
            { "Neutral", new List<string>
                {
                    "Neutral arcs steadied, balance returned...",
                    "Neutral shimmered, persistence held without tilt...",
                    "Neutral pulsed softly, bias dissolved into equilibrium..."
                }
            },
            { "Hostility", new List<string>
                {
                    "Hostility arcs sharpened, resonance turned discordant...",
                    "Hostility surged, persistence fractured under aggression...",
                    "Hostility shimmered, bias tilted toward conflict and strain..."
                }
            }
        };

        private static readonly Dictionary<IntentType, List<string>> IntentTemplates = new()
        {
            { IntentType.Stabilize,     new List<string> { "Stability shimmered, persistence anchored resilience..." } },
            { IntentType.Disrupt,       new List<string> { "Disruption fractured the balance, scars deepened..." } },
            { IntentType.Reflect,       new List<string> { "Reflection shimmered, duality revealed hidden truths..." } },
            { IntentType.Amplify,       new List<string> { "Amplification surged, harmony magnified into mythic chords..." } },
            { IntentType.Creation,      new List<string> { "Creation shimmered into being, equilibrium birthed new form..." } },
            { IntentType.Destruction,   new List<string> { "Destruction tore legacy apart, fracture collapsed the axis..." } },
            { IntentType.Transformation,new List<string> { "Transformation surged, resonance mutated under volatility..." } }
        };

        public static string GetTemplate(ToneTuple toneTuple, int tickId)
        {
            var category = toneTuple.Primary?.Category ?? "Neutral";

            if (!ToneTemplates.ContainsKey(category))
                return $"[Tick {tickId}] No narrative template defined for tone category {category}.";

            var options = ToneTemplates[category];
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
}