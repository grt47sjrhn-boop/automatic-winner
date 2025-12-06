using System;

namespace substrate_shared.Registries.enums.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class NarrativeAttribute : Attribute
    {
        public string Description { get; }
        public Bias Bias { get; }

        public NarrativeAttribute(string description, Bias bias)
        {
            Description = description;
            Bias = bias;
        }
    }
}