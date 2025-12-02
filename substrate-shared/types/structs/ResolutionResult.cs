using substrate_shared.types.models;

namespace substrate_shared.types.structs
{
    /// <summary>
    /// Composite result returned by resolvers: updated VectorBias plus DeltaSummary context.
    /// </summary>
    public struct ResolutionResult
    {
        public VectorBias Bias { get; set; }
        public DeltaSummary Summary { get; set; }

        public ResolutionResult(VectorBias bias, DeltaSummary summary)
        {
            Bias = bias;
            Summary = summary;
        }
    }
}