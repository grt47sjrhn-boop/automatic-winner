using substrate_core.Resolvers.Base;
using substrate_shared.interfaces;

namespace substrate_core.Resolvers
{
    public class SystemResolver : Resolver
    {
        public override string Name => nameof(SystemResolver);

        public override ISummary Resolve()
        {
            // TODO: implement system-specific resolution logic
            throw new System.NotImplementedException();
        }

        public override void Describe()
        {
            // TODO: provide description of this resolver
            throw new System.NotImplementedException();
        }
    }
}