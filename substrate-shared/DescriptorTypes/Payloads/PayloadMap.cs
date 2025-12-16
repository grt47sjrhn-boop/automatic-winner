using System.Collections.Generic;
using System.Text.Json;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums;

namespace substrate_shared.DescriptorTypes.Payloads;

public class PayloadMap
{
    public Dictionary<string, int>? Reputation { get; set; }
    public Dictionary<string, int>? Trust { get; set; }
    public Dictionary<string, int>? Influence { get; set; }
    public Dictionary<string, bool>? Flags { get; set; }
    public Dictionary<string, double>? Scalars { get; set; }
    public List<string>? Tags { get; set; }
    public NarrativeTone? Tone { get; set; }
    public string? Voice { get; set; }
    public string? Alignment { get; set; }
    public Bias? Bias { get; set; }

    public Dictionary<string, object> ToDictionary()
    {
        var dict = new Dictionary<string, object>();

        if (Reputation?.Count > 0) dict["reputation"] = Reputation;
        if (Trust?.Count > 0) dict["trust"] = Trust;
        if (Influence?.Count > 0) dict["influence"] = Influence;
        if (Flags?.Count > 0) dict["flags"] = Flags;
        if (Scalars?.Count > 0) dict["scalars"] = Scalars;
        if (Tags?.Count > 0) dict["tags"] = Tags;
        if (!string.IsNullOrWhiteSpace(Tone.Label)) dict["tone"] = Tone;
        if (!string.IsNullOrWhiteSpace(Voice)) dict["voice"] = Voice;
        if (!string.IsNullOrWhiteSpace(Alignment)) dict["alignment"] = Alignment;
        if (Bias.HasValue) dict["bias"] = Bias.ToString();

        return dict;
    }

    public string ToJson() =>
        JsonSerializer.Serialize(ToDictionary(), new JsonSerializerOptions { WriteIndented = true });

    public bool IsEmpty() =>
        (Reputation == null || Reputation.Count == 0) &&
        (Trust == null || Trust.Count == 0) &&
        (Influence == null || Influence.Count == 0) &&
        (Flags == null || Flags.Count == 0) &&
        (Scalars == null || Scalars.Count == 0) &&
        (Tags == null || Tags.Count == 0) &&
        string.IsNullOrWhiteSpace(Tone.Label) &&
        string.IsNullOrWhiteSpace(Voice) &&
        string.IsNullOrWhiteSpace(Alignment) &&
        !Bias.HasValue;
}