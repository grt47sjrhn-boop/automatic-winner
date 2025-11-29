namespace substrate_shared.structs
{
    public readonly struct ResolutionLayers
    {
        public readonly float Dominant;
        public readonly float Secondary;
        public readonly float Hidden;

        public ResolutionLayers(float dominant, float secondary, float hidden)
        {
            Dominant = dominant;
            Secondary = secondary;
            Hidden = hidden;
        }
    }
}