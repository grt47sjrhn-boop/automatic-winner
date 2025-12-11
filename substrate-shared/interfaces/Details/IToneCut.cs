using System.Collections.Generic;
using substrate_shared.Registries.enums;

namespace substrate_shared.interfaces.Details
{
    /// <summary>
    /// Contract for a cut of a tone distribution.
    /// Provides the dominant tone and filtered distribution.
    /// </summary>
    public interface IToneCut
    {
        /// <summary>
        /// The dominant tone selected from the distribution.
        /// </summary>
        ToneType Primary { get; set; }

        /// <summary>
        /// The filtered distribution of tones (above threshold, etc.).
        /// </summary>
        Dictionary<ToneType,int> Distribution { get; set; }

        /// <summary>
        /// Convenience property: just the tone keys as an array.
        /// </summary>
        ToneType[] Tones { get; }

        /// <summary>
        /// String representation of the tone cut.
        /// </summary>
        string ToString();
    }
}