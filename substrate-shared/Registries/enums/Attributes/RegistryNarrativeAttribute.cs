using System;

namespace substrate_shared.Registries.enums.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class RegistryNarrativeAttribute(string description, Bias bias, NarrativeGroup group, int scaleValue) : Attribute
    {
        public string Description { get; } = description;
        public string Narrative => Description;
        public Bias Bias { get; } = bias;
        public NarrativeGroup Group { get; } = group;
        public int ScaleValue { get; } = scaleValue;
    }
}