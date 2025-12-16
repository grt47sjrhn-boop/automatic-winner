using System.Collections.Generic;
using System.Text.Json;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums;

namespace substrate_shared.DescriptorTypes.Payloads
{
    public class PayloadMap
    {
        // Existing typed fields
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

        // 🔑 New: arbitrary payload storage
        private readonly Dictionary<string, object> _extras = new();

        /// <summary>
        /// Indexer for arbitrary payload values (e.g. intentCommand).
        /// </summary>
        public object? this[string key]
        {
            get => _extras.TryGetValue(key, out var value) ? value : null;
            set => _extras[key] = value!;
        }

        /// <summary>
        /// Add or update an extra payload entry.
        /// </summary>
        public void Add(string key, object value) => _extras[key] = value;

        /// <summary>
        /// Try to get a typed payload entry.
        /// </summary>
        public bool TryGet<T>(string key, out T? value)
        {
            if (_extras.TryGetValue(key, out var obj) && obj is T cast)
            {
                value = cast;
                return true;
            }
            value = default;
            return false;
        }

        public Dictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>();

            if (Reputation?.Count > 0) dict["reputation"] = Reputation;
            if (Trust?.Count > 0) dict["trust"] = Trust;
            if (Influence?.Count > 0) dict["influence"] = Influence;
            if (Flags?.Count > 0) dict["flags"] = Flags;
            if (Scalars?.Count > 0) dict["scalars"] = Scalars;
            if (Tags?.Count > 0) dict["tags"] = Tags;
            if (!string.IsNullOrWhiteSpace(Tone?.Label)) dict["tone"] = Tone;
            if (!string.IsNullOrWhiteSpace(Voice)) dict["voice"] = Voice;
            if (!string.IsNullOrWhiteSpace(Alignment)) dict["alignment"] = Alignment;
            if (Bias.HasValue) dict["bias"] = Bias.ToString();

            // 🔑 Include extras
            foreach (var kvp in _extras)
                dict[kvp.Key] = kvp.Value;

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
            string.IsNullOrWhiteSpace(Tone?.Label) &&
            string.IsNullOrWhiteSpace(Voice) &&
            string.IsNullOrWhiteSpace(Alignment) &&
            !Bias.HasValue &&
            _extras.Count == 0;
    }
}