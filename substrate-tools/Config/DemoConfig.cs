using substrate_shared.enums;

namespace substrate_tools.Config
{
    public class DemoConfig
    {
        public int TickCount { get; set; } = 20;
        public bool UseHybridMoods { get; set; } = false;
        public bool GenerateCharts { get; set; } = true;
        public bool PrintNarratives { get; set; } = true;
        public NarrativeMode Mode { get; set; } = NarrativeMode.Hybrid;
        public bool ShowHelp { get; set; } = false;
    }
}