namespace substrate_tools.Config
{
    public class DemoConfig
    {
        public int TickCount { get; set; } = 20;          // number of moods to generate
        public bool UseHybridMoods { get; set; } = false; // toggle between random/hybrid
        public bool GenerateCharts { get; set; } = true;  // toggle chart generation
        public bool PrintNarratives { get; set; } = true; // toggle narrative printing
    }
}