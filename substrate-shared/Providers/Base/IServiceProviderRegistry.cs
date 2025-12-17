namespace substrate_shared.Providers.Base
{
    public interface IServiceProviderRegistry
    {
        T Get<T>() where T : class;
        bool Has<T>() where T : class;
        bool TryGet<T>(out T? service) where T : class;
    }
}