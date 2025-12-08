using System;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.Summaries.Base;
using substrate_shared.types;

namespace substrate_core.Summaries.Types
{
    public class DialogueEventSummary : EventSummary
    {
        public BiasVector SpeakerA { get; }
        public BiasVector SpeakerB { get; }
        public ToneType DominantTone { get; }
        public bool IsBalanced { get; }

        public DialogueEventSummary(
            string title,
            string description,
            SummaryType type,
            BiasVector speakerA,
            BiasVector speakerB,
            ToneType dominantTone,
            bool isBalanced,
            bool isResolved = false)
            : base(title, description, type, isResolved)
        {
            SpeakerA = speakerA;
            SpeakerB = speakerB;
            DominantTone = dominantTone;
            IsBalanced = isBalanced;
        }

        public override void Print()
        {
            var resolvedText = Type == SummaryType.Dialogue ? $" (Resolved: {IsResolved})" : string.Empty;
            Console.WriteLine(
                $"{Title} - {Description} [{Type}] " +
                $"Speakers: {SpeakerA.Tone.Label} vs {SpeakerB.Tone.Label} " +
                $"Dominant Tone: {DominantTone} " +
                $"Balanced: {IsBalanced}{resolvedText}"
            );
        }
    }
}