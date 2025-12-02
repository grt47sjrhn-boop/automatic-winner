namespace substrate_shared.types.structs
{
    public struct AngularCategoryInfo
    {
        public float MinTheta { get; set; }
        public float MaxTheta { get; set; }
        public string Category { get; set; }
        
        public bool InRange(float theta)
        {
            return theta >= MinTheta && theta < MaxTheta;
        }


        public override string ToString()
        {
            return $"{Category}[{MinTheta:F2}→{MaxTheta:F2}]";
        }

    }
}