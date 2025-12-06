namespace substrate_core.Reporting
{
    public class ResilienceReport
    {
        // Narrative layer
        public int DuelCount { get; set; }
        public int ResilienceIndex { get; set; }
        public int RecoveryCount { get; set; }
        public int CollapseCount { get; set; }
        public int WoundCount { get; set; }
        public int ConflictCount { get; set; }
        public int EquilibriumCount { get; set; }

        // Math overlay layer
        public double AverageHypotenuse { get; set; }
        public double CumulativeArea { get; set; }
        public double MeanCos { get; set; }
        public double MeanSin { get; set; }
        public double LogScaledIndex { get; set; }
        public double ExpScaledIndex { get; set; }

        public override string ToString()
        {
            return
                $"Resilience Report → Duels: {DuelCount}, Index: {ResilienceIndex}, " +
                $"Recoveries: {RecoveryCount}, Collapses: {CollapseCount}, Wounds: {WoundCount}, Conflicts: {ConflictCount}, Equilibria: {EquilibriumCount}. " +
                $"Math → AvgHyp: {AverageHypotenuse:F2}, CumArea: {CumulativeArea:F2}, MeanCos: {MeanCos:F2}, MeanSin: {MeanSin:F2}, " +
                $"LogIndex: {LogScaledIndex:F2}, ExpIndex: {ExpScaledIndex:F2}";
        }
    }
}