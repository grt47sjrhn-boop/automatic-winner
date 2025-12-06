using System;
using substrate_shared.interfaces;

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
    }
}