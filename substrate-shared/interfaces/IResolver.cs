using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_shared.interfaces
{
    public interface IResolver
    {
        ResolutionResult Resolve(VectorBias vb, Mood mv);
    }

}