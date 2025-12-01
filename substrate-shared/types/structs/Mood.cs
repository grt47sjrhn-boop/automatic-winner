namespace substrate_shared.types.structs
{
    public struct Mood
    {
        public int MoodAxis;      // -11 … +11
        public float Magnitude;   // Strength of incoming cluster
        public string ClusterId;  // Identifier for cluster source
    }
}