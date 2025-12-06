using substrate_shared.interfaces;

namespace substrate_shared.Resolvers.Base
{
    public abstract class Resolver : IResolver
    {
        public abstract string Name { get; }

        public abstract ISummary Resolve();

        public abstract void Describe();
    }
}