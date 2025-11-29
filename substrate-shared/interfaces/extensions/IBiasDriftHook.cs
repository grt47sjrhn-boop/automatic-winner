using substrate_shared.structs;

namespace substrate_shared.interfaces.extensions
{
    public interface IBiasDriftHook
    {
        void OnBiasDrift(BiasVector ctxOldBias, BiasVector ctxNewBias);
    }
}