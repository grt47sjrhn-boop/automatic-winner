using System;
using System.Linq;
using System.Reflection;
using substrate_shared.Descriptors.Base;

namespace substrate_core.Registries
{
    public static class DescriptorBootstrapper
    {
        public static void RegisterAllDescriptors(DescriptorRegistry registry, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();

            var descriptorTypes = assembly
                .GetTypes()
                .Where(t =>
                    typeof(BaseDescriptor).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    !t.IsInterface &&
                    t.GetConstructor(Type.EmptyTypes) != null);

            foreach (var type in descriptorTypes)
            {
                var instance = (BaseDescriptor)Activator.CreateInstance(type)!;
                registry.Register(instance.Id ?? type.Name, instance);

                Console.WriteLine($"[DescriptorBootstrapper] Registered: {type.Name} ({instance.Type})");
            }

            Console.WriteLine($"[DescriptorBootstrapper] Total descriptors registered: {registry.GetAll().Count()}");
        }
    }
}