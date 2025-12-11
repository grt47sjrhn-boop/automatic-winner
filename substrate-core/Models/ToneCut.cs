using System.Collections.Generic;
using System.Linq;
using substrate_shared.interfaces.Details;
using substrate_shared.Registries.enums;

namespace substrate_core.Models
{
    /// <summary>
    /// Represents a cut of a tone distribution, with a primary tone and optional filtered set.
    /// </summary>
    public class ToneCut : IToneCut
    {
        /// <summary>
        /// The dominant tone selected from the distribution.
        /// </summary>
        public ToneType Primary { get; set; } = ToneType.Neutral;

        /// <summary>
        /// The filtered distribution of tones (above threshold, etc.).
        /// </summary>
        public Dictionary<ToneType,int> Distribution { get; set; } = new();

        public ToneCut() { }
        
        /// <summary>
        /// Convenience property: just the tone keys as an array.
        /// </summary>
        public ToneType[] Tones => Distribution?.Keys.ToArray() ?? [];


        public ToneCut(ToneType primary, Dictionary<ToneType,int> distribution)
        {
            Primary = primary;
            Distribution = distribution;
        }

        public override string ToString() =>
            $"Primary: {Primary}, Distribution size: {Distribution.Count}";
    }
}