namespace substrate_shared.interfaces
{
    // General contract for all duel resolvers
    public interface IDuelResolver
    {
        string Name { get; }
        ISummary Resolve();
        void Describe();
    }
}

   