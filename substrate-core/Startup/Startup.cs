using System;
using System.Linq;
using System.Reflection;
using substrate_shared.Resolvers.Contract;
using substrate_core.Registries;
using substrate_shared.Descriptors.Base;
using substrate_shared.Resolvers.Contract.Interfaces;

namespace substrate_core.Startup
{
    public static class Startup
    {
        public static AppBootstrap Initialize(Assembly descriptorAssembly, Assembly resolverAssembly)
        {
            var registry = new DescriptorResolverRegistry();

            // Discover descriptors
            var descriptors = descriptorAssembly
                .GetTypes()
                .Where(IsConcreteDescriptor)
                .Select(CreateDescriptor)
                .ToList();

            Console.WriteLine($"[Startup] Found {descriptors.Count} descriptors in {descriptorAssembly.GetName().Name}");

            // Discover resolvers
            var resolvers = resolverAssembly
                .GetTypes()
                .Where(IsConcreteResolver)
                .Select(CreateResolver)
                .ToList();

            Console.WriteLine($"[Startup] Found {resolvers.Count} resolvers in {resolverAssembly.GetName().Name}");

            // Pair descriptors with resolvers by convention: Resolver.TargetDescriptorType
            foreach (var resolver in resolvers)
            {
                var expectedDescriptorName = resolver.Name.Replace("Resolver", "Descriptor");
                //Console.WriteLine($"[Startup] Resolver {resolver.Name} expects descriptor {expectedDescriptorName}");

                var descriptor = descriptors.FirstOrDefault(d => d.GetType().Name == expectedDescriptorName);
                if (descriptor != null)
                {
                    registry.Register(descriptor.GetType(), descriptor, resolver);
                    Console.WriteLine($"[Startup] Registered pair: {descriptor.GetType().Name} -> {resolver.GetType().Name}");
                }
                else
                {
                    Console.WriteLine($"[Startup] No descriptor found matching {expectedDescriptorName}");
                }
            }

            Console.WriteLine($"[Startup] Total pairs registered: {registry.GetAll().Count()}");
            return new AppBootstrap(registry);
        }

        private static bool IsConcreteDescriptor(Type t) =>
            typeof(BaseDescriptor).IsAssignableFrom(t)
            && !t.IsAbstract
            && !t.IsInterface
            && t.GetConstructor(Type.EmptyTypes) != null;

        private static BaseDescriptor CreateDescriptor(Type t) =>
            (BaseDescriptor)Activator.CreateInstance(t)!;

        private static bool IsConcreteResolver(Type t) =>
            typeof(IFrameResolver).IsAssignableFrom(t)
            && !t.IsAbstract
            && !t.IsInterface
            && t.GetConstructor(Type.EmptyTypes) != null;

        private static IFrameResolver CreateResolver(Type t) =>
            (IFrameResolver)Activator.CreateInstance(t)!;
    }

    public sealed class AppBootstrap
    {
        public DescriptorResolverRegistry Registry { get; }

        public AppBootstrap(DescriptorResolverRegistry registry)
        {
            Registry = registry;
        }
    }
}