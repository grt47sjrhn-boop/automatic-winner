using System;
using System.Linq;
using System.Reflection;
using substrate_shared.Resolvers.Contract;
using substrate_shared.Resolvers.Contract.Interfaces;

namespace substrate_core.Registries
{
    public static class ResolverBootstrapper
    {
        public static void RegisterAllResolvers(ResolverRegistry registry, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();

            var resolverTypes = assembly
                .GetTypes()
                .Where(t =>
                    typeof(IFrameResolver).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    !t.IsInterface &&
                    t.GetConstructor(Type.EmptyTypes) != null);

            foreach (var type in resolverTypes)
            {
                var instance = (IFrameResolver)Activator.CreateInstance(type)!;
                registry.Register(instance.Name, instance);

                Console.WriteLine($"[ResolverBootstrapper] Registered: {instance.Name} ({instance.Category}) - {instance.Description}");
            }

            Console.WriteLine($"[ResolverBootstrapper] Total resolvers registered: {registry.GetAll().Count()}");
        }
    }
}