using System;
using System.Collections.Generic;
using substrate_core.Registries;
using substrate_shared.DescriptorTypes.Frames;
using substrate_shared.DescriptorTypes.Validators;
using substrate_shared.Errors;
using substrate_shared.interfaces.Reports;
using substrate_shared.Providers.Contract;
using substrate_shared.Providers.Base;
using substrate_shared.Resolvers.Contract;

namespace substrate_core.Internal.Engines
{
    public sealed class CatalystEngine
    {
        private bool _enabled;
        private readonly IServiceProviderRegistry _services;
        private readonly ResolverRegistry _resolverRegistry;

        private readonly DescriptorRegistry _descriptorRegistry;

        public CatalystEngine(IServiceProviderRegistry services, bool enabled = true)
        {
            _services = services;
            _enabled = enabled;

            if (!_services.Has<ResolverRegistry>())
                throw new InvalidOperationException("ResolverRegistry not registered.");

            if (!_services.Has<DescriptorRegistry>())
                throw new InvalidOperationException("DescriptorRegistry not registered.");

            _resolverRegistry = _services.Get<ResolverRegistry>();
            _descriptorRegistry = _services.Get<DescriptorRegistry>();

            Console.WriteLine($"[CatalystEngine] Initialized with registry: {_resolverRegistry.Name}");
            foreach (var resolver in _resolverRegistry.GetAll())
            {
                Console.WriteLine($"[CatalystEngine] Registered resolver: {resolver.Name}");
            }

            Console.WriteLine($"[CatalystEngine] Descriptor registry: {_descriptorRegistry.Name}");
            foreach (var descriptor in _descriptorRegistry.GetAll())
            {
                Console.WriteLine($"[CatalystEngine] Registered descriptor: {descriptor.Type} ({descriptor.GetType().Name})");
            }
        }

        
        public bool Execute(IReportSummary report)
        {
            if (!_enabled) return true;

            if (!_services.Has<ISimulationFrameProvider>())
            {
                report.LogError("No ISimulationFrameProvider registered.");
                return false;
            }

            var frameProvider = _services.Get<ISimulationFrameProvider>();
            var frame = frameProvider.GetFrame();

            if (!ValidateFrame(frame, report)) return false;

            foreach (var resolver in _resolverRegistry.GetAll())
            {
                Console.WriteLine($"[CatalystEngine] Executing resolver: {resolver.Name}");
                resolver.Resolve(frame, report);
            }

            return true;
        }

        public bool ValidateFrame(SimulationFrame frame, IReportSummary report)
        {
            List<string> errors;

            if (DescriptorValidatorDispatcher.Validate(frame, out errors))
                return true;

            foreach (var error in errors)
            {
                var validationError = new ValidationError(error, source: "CatalystEngine");
                report.LogValidationError(validationError);
            }

            return false;
        }

        public void Toggle(bool enabled) => _enabled = enabled;
    }
}