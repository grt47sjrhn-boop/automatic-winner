using substrate_shared.Descriptors.Base;
using substrate_shared.Descriptors.Enums;

namespace substrate_shared.interfaces.Reports
{
    public interface IPerformanceReport
    {
        void Increment(DescriptorType type);
        void MarkMissingStrategy(BaseDescriptor d);
        void AddOutcome(string outcomeId);
    }
}