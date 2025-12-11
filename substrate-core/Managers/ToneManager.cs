using System;
using System.Collections.Generic;
using System.Linq;
using substrate_core.Models;
using substrate_shared.interfaces;
using substrate_shared.interfaces.Details;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums;
using substrate_shared.structs;

namespace substrate_core.Managers
{
    /// <summary>
    /// Manager responsible for interpreting and narrating tones.
    /// Implements IManager for orchestration consistency.
    /// </summary>
    public class ToneManager : IToneManager
    {
        /// <summary>
        /// Summarize a tone into a BiasDescriptor.
        /// </summary>
        public BiasDescriptor Summarize(NarrativeTone tone)
        {
            return new BiasDescriptor
            {
                Bias = tone.BiasValue,
                Narrative = $"Tone summary → {tone.Label} (Category: {tone.Category}, Bias: {tone.BiasValue}, Group: {tone.Group})"
            };
        }

        /// <summary>
        /// Blend one tone against another, producing a new NarrativeTone and descriptor.
        /// </summary>
        public BiasDescriptor Blend(NarrativeTone winner, NarrativeTone opponent)
        {
            var blended = winner.BlendAgainst(opponent);
            return new BiasDescriptor
            {
                Bias = blended.BiasValue,
                Narrative = $"Tone blend → {blended.Label}"
            };
        }

        /// <summary>
        /// Merge two tones into a neutral equilibrium.
        /// </summary>
        public BiasDescriptor Merge(NarrativeTone a, NarrativeTone b)
        {
            var merged = a.MergeNeutral(b);
            return new BiasDescriptor
            {
                Bias = merged.BiasValue,
                Narrative = $"Tone merge → {merged.Label}"
            };
        }

        /// <summary>
        /// Resolve a set of tones into the dominant descriptor.
        /// </summary>
        public BiasDescriptor ResolveCluster(IEnumerable<NarrativeTone> tones)
        {
            NarrativeTone? dominant = null;
            foreach (var tone in tones)
            {
                if (dominant == null || tone.BiasMultiplier > dominant.BiasMultiplier)
                    dominant = tone;
            }

            if (dominant == null)
            {
                return new BiasDescriptor
                {
                    Bias = Bias.Neutral,
                    Narrative = "No tones provided → Neutral bias"
                };
            }

            return Summarize(dominant);
        }

        /// <summary>
        /// Determine the dominant tone type between two bias vectors.
        /// </summary>
        public ToneType DetermineDominant(BiasVector a, BiasVector b)
        {
            return a.Magnitude >= b.Magnitude ? a.DominantTone : b.DominantTone;
        }

        public bool CheckBalance(BiasVector a, BiasVector b)
        {
            var diff = Math.Abs(a.Magnitude - b.Magnitude);
            return diff <= 1;
        }

        public IToneCut ResolveToneCut(BiasVector persistentBiasVector, BiasVector opponent)
        {
            var cut = new ToneCut();

            // If balanced, return an equilibrium cut
            if (CheckBalance(persistentBiasVector, opponent))
            {
                cut.Primary = ToneType.Neutral;
                cut.Distribution[persistentBiasVector.DominantTone] = persistentBiasVector.Magnitude;
                cut.Distribution[opponent.DominantTone] = opponent.Magnitude;
                return cut;
            }

            // Otherwise, determine dominant tone
            var dominantTone = DetermineDominant(persistentBiasVector, opponent);
            cut.Primary = dominantTone;

            // Build distribution from both vectors
            var combined = new Dictionary<ToneType,int>();

            void addTone(ToneType tone, int magnitude)
            {
                if (combined.ContainsKey(tone))
                    combined[tone] += magnitude;
                else
                    combined[tone] = magnitude;
            }

            addTone(persistentBiasVector.DominantTone, persistentBiasVector.Magnitude);
            addTone(opponent.DominantTone, opponent.Magnitude);

            // Normalize distribution into the cut
            foreach (var kv in combined)
            {
                if (kv.Value > 0)
                    cut.Distribution[kv.Key] = kv.Value;
            }

            return cut;
        }

        /// <summary>
        /// Cut a normalized facet shape into a ToneCut (brilliance representation).
        /// </summary>
        public IToneCut Cut(IReadOnlyDictionary<ToneType,int> facets)
        {
            var cut = new ToneCut();
            if (facets == null || facets.Count == 0)
            {
                cut.Primary = ToneType.Neutral;
                return cut;
            }

            // Primary = highest facet
            var primary = facets.OrderByDescending(f => f.Value).First();
            cut.Primary = primary.Key;

            // Distribution = all facets above threshold
            foreach (var kv in facets)
            {
                if (kv.Value > 0)
                    cut.Distribution[kv.Key] = kv.Value;
            }

            return cut;
        }
    }
}