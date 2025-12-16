using System.Collections.Generic;
using System.Text.Json;
using substrate_shared.Descriptors.Enums;
using substrate_shared.Descriptors.Payloads;
using substrate_shared.Descriptors.Validators;
using substrate_shared.Registries.Base;

namespace substrate_shared.Descriptors.Base
{
    public abstract class BaseDescriptor : IDescriptor
    {
        public string Id { get; set; }
        public abstract DescriptorType Type { get; }

        public string? Source { get; set; }
        public PayloadMap? Payload { get; set; }
        public NarrativeTone? Tone { get; set; } // Optional override

        public virtual Dictionary<string, object> Describe()
        {
            var dict = new Dictionary<string, object>
            {
                { "id", Id },
                { "type", Type.ToString() }
            };

            if (!string.IsNullOrWhiteSpace(Source))
                dict["source"] = Source;

            if (Payload != null && !Payload.IsEmpty())
                dict["payload"] = Payload.ToDictionary();

            if (Tone != null)
                dict["tone"] = Tone.ToString();

            return dict;
        }

        public string ToJson() =>
            JsonSerializer.Serialize(Describe(), new JsonSerializerOptions { WriteIndented = true });

        public virtual bool IsValid(out string? error) =>
            DescriptorValidatorDispatcher.Validate(this, out error);
    }

    public interface IDescriptor
    {
        string Id { get; }
        DescriptorType Type { get; }
        string? Source { get; }
        PayloadMap? Payload { get; }
        NarrativeTone? Tone { get; }

        Dictionary<string, object> Describe();
        string ToJson();
        bool IsValid(out string? error);
    }

}