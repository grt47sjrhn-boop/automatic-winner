namespace substrate_shared.Registries.Contract.Base;

public interface IRegistry<T>
{
    string Name { get; set; }
    void Register(string key, T item);
    bool TryGet(string key, out T? item);
    bool Contains(string key);
    void Clear();
}