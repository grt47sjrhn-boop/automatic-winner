using System;
using substrate_shared.interfaces.core;

namespace substrate_shared.types.core
{
    public class Trigger : ITrigger
    {
        public string Id { get; private set; }
        public Func<CycleContext, bool> Condition { get; private set; }
        public double EmissionValue { get; private set; }
        public string ContributorNote { get; private set; }

        public Trigger(string id, Func<CycleContext, bool> condition, double emissionValue, string note)
        {
            Id = id;
            Condition = condition;
            EmissionValue = emissionValue;
            ContributorNote = note;
        }

        public void Execute(CycleContext context)
        {
            if (!Condition(context)) return;
            context.FiredTriggers.Add(this);
            Console.WriteLine($"Trigger {Id} fired: {ContributorNote}");
        }
    }
}