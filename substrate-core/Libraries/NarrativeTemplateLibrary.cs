using System.Collections.Generic;
using substrate_shared.enums;
using substrate_shared.types.structs;

namespace substrate_core.Libraries
{
    public static class NarrativeTemplateLibrary
    {
        // Templates keyed by Primary + Complementary tone pair
        private static readonly Dictionary<(Tone, Tone), string> Templates = new()
        {
            // Resonance pairings
            { (Tone.Resonance, Tone.Scar), "Bias tilted toward {Primary}, undertones of {Adj1} and {Adj2} rose, while {Complementary} lingered as complement." },
            { (Tone.Resonance, Tone.Neutral), "Resonance pulsed as {Primary}, balanced by {Complementary}, undertones weaving {Adj1} and {Adj2}." },
            { (Tone.Resonance, Tone.Harmony), "Resonance surged into {Primary}, yet harmony tempered as {Complementary}, undertones shimmered {Adj1} and {Adj2}." },
            { (Tone.Resonance, Tone.Fracture), "Resonance strained against fractures, undertones of {Adj1} and {Adj2} echoed, while {Complementary} shadowed the chord." },

            // Scar pairings
            { (Tone.Scar, Tone.Resonance), "Scars deepened into {Primary}, yet resonance balanced as {Complementary}, undertones of {Adj1} and {Adj2} shimmered." },
            { (Tone.Scar, Tone.Neutral), "Scar lingered as {Primary}, neutral ground held as {Complementary}, undertones whispered {Adj1} and {Adj2}." },
            { (Tone.Scar, Tone.Harmony), "Scar carved into {Primary}, harmony rose as {Complementary}, undertones of {Adj1} and {Adj2} flickered." },
            { (Tone.Scar, Tone.Fracture), "Scar merged with fracture, undertones of {Adj1} and {Adj2} resonated, while {Complementary} completed the cycle." },

            // Neutral pairings
            { (Tone.Neutral, Tone.Resonance), "Neutral ground held, but resonance stirred beneath, undertones of {Adj1} and {Adj2} whispered alongside {Complementary}." },
            { (Tone.Neutral, Tone.Scar), "Neutral persisted as {Primary}, scar pressed as {Complementary}, undertones of {Adj1} and {Adj2} rose faintly." },
            { (Tone.Neutral, Tone.Harmony), "Neutral steadied as {Primary}, harmony balanced as {Complementary}, undertones of {Adj1} and {Adj2} shimmered." },
            { (Tone.Neutral, Tone.Fracture), "Neutral cracked into fracture, undertones of {Adj1} and {Adj2} echoed, while {Complementary} shadowed the chord." },

            // Harmony pairings
            { (Tone.Harmony, Tone.Resonance), "Harmony swelled as {Primary}, resonance echoed as {Complementary}, undertones weaving {Adj1} and {Adj2}." },
            { (Tone.Harmony, Tone.Scar), "Harmony steadied as {Primary}, scar pressed as {Complementary}, undertones of {Adj1} and {Adj2} shimmered." },
            { (Tone.Harmony, Tone.Neutral), "Harmony balanced as {Primary}, neutral held as {Complementary}, undertones whispered {Adj1} and {Adj2}." },
            { (Tone.Harmony, Tone.Fracture), "Harmony strained against fracture, undertones of {Adj1} and {Adj2} echoed, while {Complementary} shadowed the chord." },

            // Fracture pairings
            { (Tone.Fracture, Tone.Resonance), "Fracture split into {Primary}, resonance tempered as {Complementary}, undertones of {Adj1} and {Adj2} shimmered." },
            { (Tone.Fracture, Tone.Scar), "Fracture deepened as {Primary}, scar balanced as {Complementary}, undertones whispered {Adj1} and {Adj2}." },
            { (Tone.Fracture, Tone.Neutral), "Fracture cracked into {Primary}, neutral steadied as {Complementary}, undertones of {Adj1} and {Adj2} rose faintly." },
            { (Tone.Fracture, Tone.Harmony), "Fracture strained against harmony, undertones of {Adj1} and {Adj2} echoed, while {Complementary} shadowed the chord." },

            // Undertone-specific templates
            { (Tone.Irony, Tone.Resonance), "Irony surfaced beneath {Primary}, resonance tempered as {Complementary}, undertones shimmered {Adj1} and {Adj2}." },
            { (Tone.Irony, Tone.Scar), "Irony cut through {Primary}, scar lingered as {Complementary}, undertones whispered {Adj1} and {Adj2}." },
            { (Tone.Volatility, Tone.Harmony), "Volatility surged under {Primary}, harmony steadied as {Complementary}, undertones flickered {Adj1} and {Adj2}." },
            { (Tone.Volatility, Tone.Neutral), "Volatility rippled beneath {Primary}, neutral balanced as {Complementary}, undertones shimmered {Adj1} and {Adj2}." },
            { (Tone.Duality, Tone.Resonance), "Duality unfolded within {Primary}, resonance echoed as {Complementary}, undertones of {Adj1} and {Adj2} intertwined." },
            { (Tone.Duality, Tone.Fracture), "Duality fractured into {Primary}, undertones of {Adj1} and {Adj2} shimmered, while {Complementary} shadowed the chord." }
        };

        public static string GetTemplate(ToneTuple tuple)
        {
            // Fallback generic template
            return Templates.GetValueOrDefault((tuple.Primary, tuple.Complementary), 
                "Bias tilted toward {Primary}, undertones of {Adj1} and {Adj2} rose, while {Complementary} lingered as complement.");
        }
    }
}