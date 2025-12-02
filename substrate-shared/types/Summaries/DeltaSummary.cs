using substrate_shared.interfaces;

namespace substrate_shared.types.Summaries
{
    public class DeltaSummary : ISummary
    {
        public string Name => nameof(DeltaSummary);

        public float DeltaAxis { get; set; }
        public float Magnitude { get; set; }
        public float Hypotenuse { get; set; }
        public float Area { get; set; }
        public float AngleTheta { get; set; }
        public float SinTheta { get; set; }
        public float CosTheta { get; set; }
        public float TanTheta { get; set; }

        public string Describe() =>
            $"ΔAxis={DeltaAxis:F2}, Mag={Magnitude:F2}, Hyp={Hypotenuse:F2}, Area={Area:F2}, " +
            $"θ={AngleTheta:F2} rad | sinθ={SinTheta:F2}, cosθ={CosTheta:F2}, tanθ={TanTheta:F2}";
    }
}