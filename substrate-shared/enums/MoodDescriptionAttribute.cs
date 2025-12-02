using System;

namespace substrate_shared.enums
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MoodDescriptionAttribute : Attribute
    {
        public string Description { get; }
        public string Bias { get; }
        public string NarrativeGroup { get; }

        public MoodDescriptionAttribute(string description, string group, string narrativeGroup)
        {
            Description = description;
            Bias = group;
            NarrativeGroup = narrativeGroup;
        }
    }
}