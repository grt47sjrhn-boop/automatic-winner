namespace substrate_shared.Registries.Manager
{
    public interface IRegistryManager
    {
        TRegistry Get<TRegistry>() where TRegistry : class, IRegistryMarker;
        void Register<TRegistry>(TRegistry registry) where TRegistry : class, IRegistryMarker;
        bool Contains<TRegistry>() where TRegistry : class, IRegistryMarker;
    }

    public interface IRegistryMarker { } // Marker interface for all registries
}