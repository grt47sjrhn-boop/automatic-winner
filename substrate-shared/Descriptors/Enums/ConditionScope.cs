namespace substrate_shared.Descriptors.Enums
{
    public enum ConditionScope
    {
        Self,       // Applies to this descriptor
        Subject,    // Applies to a specific subject
        Any,        // Applies if any match
        All         // Applies if all match
    }
}