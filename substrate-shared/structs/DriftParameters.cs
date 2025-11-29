namespace substrate_shared.structs
{
    /// <summary>
    /// Tunable parameters controlling bias drift scaling across artifact rarity tiers.
    /// Immutable and balanced via simulation harness.
    /// </summary>
    public readonly struct DriftParameters
    {
        // Common artifacts
        public float CommonBase { get; }
        public float CommonSlope { get; }

        // Rare artifacts
        public float RareBase { get; }
        public float RareSlope { get; }

        // Chaos artifacts
        public float ChaosBase { get; }
        public float ChaosSlope { get; }

        // Collapse artifacts
        public float CollapseBase { get; }
        public float CollapseSlope { get; }

        // Emphasis factor for dominant resolution axis
        public float DominantEmphasis { get; }

        /// <summary>
        /// Creates a new immutable set of drift parameters.
        /// </summary>
        public DriftParameters(
            float commonBase = 0.2f, float commonSlope = 0.05f,
            float rareBase = 0.15f, float rareSlope = 0.05f,
            float chaosBase = 0.12f, float chaosSlope = 0.06f,
            float collapseBase = 0.2f, float collapseSlope = 0.1f,
            float dominantEmphasis = 0.2f)
        {
            CommonBase = commonBase;
            CommonSlope = commonSlope;
            RareBase = rareBase;
            RareSlope = rareSlope;
            ChaosBase = chaosBase;
            ChaosSlope = chaosSlope;
            CollapseBase = collapseBase;
            CollapseSlope = collapseSlope;
            DominantEmphasis = dominantEmphasis;
        }

        #region Summary
        // Signature: DriftParameters(float commonBase, float commonSlope, float rareBase, float rareSlope,
        //                            float chaosBase, float chaosSlope, float collapseBase, float collapseSlope,
        //                            float dominantEmphasis)
        // PRISMx Version: 1.1 | Date: 2025.11.29
        // Tags: #drift #parameters #substrate
        #endregion
    }
}