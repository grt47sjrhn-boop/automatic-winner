using System.Collections.Generic;
using System.Text;
using substrate_shared.interfaces;
using substrate_shared.Summaries.Base;
using substrate_shared.types;

namespace substrate_shared.Summaries.Types
{
    /// <summary>
    /// Wraps multiple summaries into one narratable artifact, 
    /// propagating resilience and traits from child summaries.
    /// </summary>
    public class CompositeSummary : EventSummary
    {
        private readonly List<ISummary> _summaries = new();
        private readonly Dictionary<string, string> _traits = new();

        public CompositeSummary(
            string title,
            string description,
            SummaryType type = SummaryType.Composite,
            bool resolved = true)
            : base(title, description, type, resolved)
        {
        }

        /// <summary>
        /// Add a child summary and propagate resilience values if applicable.
        /// </summary>
        public void AddSummary(ISummary summary)
        {
            _summaries.Add(summary);

            // 🔹 Propagate resilience if child is a DuelEventSummary
            if (summary is not DuelEventSummary duel) return;
            
            // Option: overwrite with latest duel
            ResilienceIndex = duel.ResilienceIndex;
            CumulativeResilience = duel.CumulativeResilience;

            // If you want accumulation across children instead:
            // ResilienceIndex += duel.ResilienceIndex;
            // CumulativeResilience += duel.CumulativeResilience;
        }
        
        public double CumulativeResilience { get; set; }
        
        /// <summary>
        /// Add or update a trait key/value pair.
        /// </summary>
        public void AddTrait(string key, string value) => _traits[key] = value;

        public override void Print()
        {
            foreach (var s in _summaries)
                s.Print();

            foreach (var kv in _traits)
                System.Console.WriteLine($"{kv.Key}: {kv.Value}");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var s in _summaries)
                sb.AppendLine(s.ToString());

            foreach (var kv in _traits)
                sb.AppendLine($"{kv.Key}: {kv.Value}");

            return sb.ToString();
        }
    }
}