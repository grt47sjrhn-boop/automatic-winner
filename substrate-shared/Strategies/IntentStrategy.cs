using substrate_shared.Descriptors.Types;
using substrate_shared.interfaces.Reports;
using substrate_shared.Providers.Contract;
using substrate_shared.Registries.ResolverRegistry;

namespace substrate_shared.Strategies
{
    public class IntentStrategy : IDescriptorStrategy<IntentDescriptor>
    {
        private readonly DescriptorResolverRegistry _resolverRegistry;

        public IntentStrategy(DescriptorResolverRegistry resolverRegistry)
        {
            _resolverRegistry = resolverRegistry;
        }

        // Interface method only
        public IntentActionStrategy Execute(IntentDescriptor descriptor, ISimulationFrameReceiver receiver, IReportSummary report)
        {
            // Resolve the canonical descriptor + resolver pair
            var (baseDescriptor, resolver) = _resolverRegistry.Resolve(descriptor.Type);

            // Build a frame containing just this intent
            var frame = new substrate_shared.Descriptors.Frames.SimulationFrame
            {
                Intent = descriptor
            };

            // Delegate to the resolver with the frame
            resolver.Resolve(frame, report);

            // Return the actionable strategy payload
            return new IntentActionStrategy(descriptor.Id, descriptor.Type, descriptor.Tone);
        }
    }
}