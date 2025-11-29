using System;
using substrate_shared.types.core;

namespace substrate_shared.interfaces.core
{
    public interface ITrigger
    {
        string Id { get; }
        Func<CycleContext, bool> Condition { get; }
        double EmissionValue { get; }
        string ContributorNote { get; }
        void Execute(CycleContext context);
    }
}