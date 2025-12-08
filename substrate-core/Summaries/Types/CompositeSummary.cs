using substrate_shared.interfaces;
using substrate_shared.Summaries.Base;

namespace substrate_core.Summaries.Types
{
    /// <summary>
    /// Wraps multiple summaries into one narratable artifact.
    /// </summary>
    public class CompositeSummary : EventSummary
    {
        private readonly ISummary _primary;
        private readonly ISummary _secondary;

        public CompositeSummary(ISummary primary, ISummary secondary)
            : base(primary.Title, primary.Description, primary.SummaryType, primary.ResolvedVector != null)
        {
            _primary = primary;
            _secondary = secondary;
        }

        public override void Print()
        {
            _primary.Print();
            _secondary.Print();
        }

        public override string ToString()
        {
            return $"{_primary}\n{_secondary}";
        }
    }
}