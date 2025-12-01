using substrate_shared.enums;

namespace substrate_shared.types.structs
{
    public struct ToneTuple
    {
        public Tone Primary;        // Resolved primary mood
        public Tone Adjacent1;      // Neighbor or undertone
        public Tone Adjacent2;      // Neighbor or undertone
        public Tone Complementary;  // Opposite/balancing mood
    }
}