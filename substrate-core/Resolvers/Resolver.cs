using substrate_shared.interfaces;

namespace substrate_core.Resolvers
{
    public abstract class Resolver : IResolver
    {
        public abstract string Name { get; }

        public abstract ISummary Resolve();

        public abstract void Describe();
    }
}