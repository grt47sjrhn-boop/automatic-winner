using System.Collections.Generic;

namespace substrate_shared.Registries.Contract.Base
{
    public interface IEnumerableRegistry<T> : IRegistry<T>
    {
        IEnumerable<T> GetAll();
    }
}