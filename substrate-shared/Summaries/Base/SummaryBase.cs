using System;
using substrate_shared.interfaces;
using substrate_shared.Registries.enums;
using substrate_shared.structs;
using substrate_shared.types;

namespace substrate_shared.Summaries.Base
{
    public abstract class SummaryBase : ISummary
    {
        public abstract string Title { get; }
        public abstract string Description { get; }

        public virtual void Print()
        {
            Console.WriteLine($"{Title} - {Description}");
        }
        public override string ToString() => $"{Title} - {Description}";
        public BiasVector? ResolvedVector { get; set; }
        public DuelOutcome Outcome { get; set; }
        public SummaryType SummaryType { get; }
    }
}