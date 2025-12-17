using substrate_shared.Registries.enums;
using substrate_shared.Registries.Managers;
using Xunit;

namespace substrate_tests
{
    // This fixture runs once per test collection
    public class RegistryFixture
    {
        public RegistryFixture()
        {
            // Register all enums you rely on in tests
            SuperRegistryManager.Register<IntentAction>();
            SuperRegistryManager.Register<ToneType>();
            SuperRegistryManager.Register<MoodType>();
        }
    }

    // Apply fixture to all tests in this collection
    [CollectionDefinition("Registry collection")]
    public class RegistryCollection : ICollectionFixture<RegistryFixture> { }
}