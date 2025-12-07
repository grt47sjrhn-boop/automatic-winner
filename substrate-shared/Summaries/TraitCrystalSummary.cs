using System.Collections.Generic;
using System.Linq;
using substrate_shared.Summaries.Base;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Managers;
using substrate_shared.Traits.Base;

namespace substrate_shared.Summaries
{
    public class TraitCrystalSummary : SummaryBase
    {
        private readonly IEnumerable<TraitCrystal> _crystals;

        public TraitCrystalSummary(IEnumerable<TraitCrystal> crystals)
        {
            _crystals = crystals;
            Title = "Crystal Inventory Summary";
            Description = BuildDescription();
        }

        public override string Title { get; }
        public override string Description { get; }

        private string BuildDescription()
        {
            var report = new List<string>();

            // 🔹 Add Crystal Inventory section if registry is active
            if (SuperRegistryManager.GetAll().Any(r => r.GetGroup() == NarrativeGroup.Crystal))
            {
                report.Add("Crystal Inventory:");
                report.Add(SuperRegistryManager.DescribeCrystalCluster(_crystals));
                report.Add("");
            }
            else
            {
                report.Add("No crystals registered in SuperRegistryManager.");
            }

            return string.Join(System.Environment.NewLine, report);
        }
    }
}