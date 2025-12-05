using substrate_shared.enums;
using substrate_shared.types.models;

namespace substrate_shared.types.structs
{
    public struct ToneTuple
    {
        public NarrativeTone? Primary;        // Resolved primary mood
        public NarrativeTone Adjacent1;      // Neighbor or undertone
        public NarrativeTone Adjacent2;      // Neighbor or undertone
        public NarrativeTone Complementary;  // Opposite/balancing mood
    }
}