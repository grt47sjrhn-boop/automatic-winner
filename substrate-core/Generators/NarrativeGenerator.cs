using System.Linq;
using substrate_core.Libraries;
using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_core.Generators
{
    public class NarrativeGenerator
    {
        public static string Generate(VectorBias vb)
        {
            // Guard clauses
            if (vb.ToneTuple.Equals(default(ToneTuple)))
                return "No tonal resolution available.";
            vb.TriggerEvents ??= new System.Collections.Generic.List<TriggerEvent>();

            // Get template from library based on Primary + Complementary tones
            var template = NarrativeTemplateLibrary.GetTemplate(vb.ToneTuple);

            // Replace placeholders with actual tone values
            var line = template
                .Replace("{Primary}", vb.ToneTuple.Primary.ToString())
                .Replace("{Adj1}", vb.ToneTuple.Adjacent1.ToString())
                .Replace("{Adj2}", vb.ToneTuple.Adjacent2.ToString())
                .Replace("{Complementary}", vb.ToneTuple.Complementary.ToString());

            // Add trigger events if any
            if (vb.TriggerEvents.Any())
            {
                var triggerDescriptions = vb.TriggerEvents.Select(t => t.Description);
                line += $". Trigger events: {string.Join("; ", triggerDescriptions)}";
            }

            // Add intent if resolved
            if (vb.Intent != substrate_shared.enums.IntentType.None)
            {
                line += $" Intent resolved as {vb.Intent}.";
            }

            // Add persistence and volatility
            line += $" Persistence held at {vb.Persistence:F2}, volatility amplified to {vb.ExpVolatility:F2}.";

            return line;
        }
    }
}