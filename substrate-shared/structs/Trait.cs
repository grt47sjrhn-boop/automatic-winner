namespace substrate_shared.structs
{
    public readonly struct Trait
    {
        public readonly string Name;
        public readonly int Tier;
        public Trait(string name, int tier) { Name = name; Tier = tier; }
    }
}