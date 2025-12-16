using System.Collections.Generic;
using System.Linq;
using substrate_shared.DescriptorTypes.Base;
using substrate_shared.DescriptorTypes.Enums;

namespace substrate_shared.DescriptorTypes.Types
{
    public class TriggerDescriptor : BaseDescriptor
    {
        public override DescriptorType Type => DescriptorType.Trigger;
        public List<SubjectDescriptor> Subjects { get; set; } = new();
        public SubjectMatchMode MatchMode { get; set; } = SubjectMatchMode.Any;

        public List<ResultDescriptor> Results { get; set; } = new();
        public List<ConditionDescriptor> Conditions { get; set; } = new();

        public string? ActivationLabel { get; set; }
        public bool IsInterruptive { get; set; }

        public override Dictionary<string, object> Describe()
        {
            var dict = base.Describe();

            dict["subjects"] = Subjects.Select(s => s.Id).ToList();
            dict["matchMode"] = MatchMode.ToString();
            dict["results"] = Results.Select(r => r.Id).ToList();
            dict["conditions"] = Conditions.Select(c => c.Id).ToList();
            dict["isInterruptive"] = IsInterruptive;

            if (!string.IsNullOrWhiteSpace(ActivationLabel))
                dict["activationLabel"] = ActivationLabel;

            return dict;
        }
    }
}