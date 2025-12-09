using System;
using substrate_shared.Models;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.Summaries.Base;
using substrate_shared.types;
using substrate_shared.Registries.Extensions;

namespace substrate_shared.Summaries
{
    public class DuelEventSummary : EventSummary
    {
        public BiasVector DuelistA { get; }
        public BiasVector DuelistB { get; }
        public BiasVector ResolvedVector { get; }
        public DuelOutcome Outcome { get; }
        
        // 🔹 Enriched fields
        public ToneCut Brilliance { get; }
        public BiasDescriptor Bias { get; }

        // Optional: carry resolved registry enrichments
        public MoodType ResolvedMood { get; }
        public IntentAction ResolvedIntent { get; }
        public CrystalRarity ResolvedRarity { get; }

        public DuelEventSummary(
            string title,
            string description,
            SummaryType type,
            BiasVector duelistA,
            BiasVector duelistB,
            BiasVector resolvedVector,
            DuelOutcome outcome,
            ToneCut brilliance,
            BiasDescriptor bias,
            MoodType resolvedMood,
            IntentAction resolvedIntent,
            CrystalRarity resolvedRarity,
            bool isResolved = false)
            : base(title, description, type, isResolved)
        {
            DuelistA = duelistA;
            DuelistB = duelistB;
            ResolvedVector = resolvedVector;
            Outcome = outcome;
            Brilliance = brilliance;
            Bias = bias;
            ResolvedMood = resolvedMood;
            ResolvedIntent = resolvedIntent;
            ResolvedRarity = resolvedRarity;
        }

        public override void Print()
        {
            var resolvedText   = Type == SummaryType.Duel ? $" (Resolved: {IsResolved})" : string.Empty;

            var duelistALabel  = DuelistA.Tone?.Label ?? "[None]";
            var duelistBLabel  = DuelistB.Tone?.Label ?? "[None]";
            var resolvedLabel  = ResolvedVector.Tone?.Label ?? "[None]";
            var resolvedMag    = ResolvedVector.Magnitude.ToString();
            var brillianceText = Brilliance?.Primary.ToString() ?? "[None]";
            var biasText       = Bias?.Bias.ToString() ?? "[None]";
            var moodText       = ResolvedMood.GetDescription() ?? "[None]";
            var intentText     = ResolvedIntent.GetDescription() ?? "[None]";
            var rarityText     = ResolvedRarity.GetDescription() ?? "[None]";

            Console.WriteLine(
                $"{Title ?? "[Untitled]"} - {Description ?? "[No description]"} [{Type}] " +
                $"Duelists: {duelistALabel} vs {duelistBLabel} " +
                $"Resolved: {resolvedLabel} (Mag {resolvedMag}) " +
                $"Outcome: {Outcome}{resolvedText}\n" +
                $"Brilliance: {brillianceText}\n" +
                $"Bias: {biasText}\n" +
                $"Mood: {moodText}\n" +
                $"Intent: {intentText}\n" +
                $"Rarity: {rarityText}"
            );
        }
    }
}