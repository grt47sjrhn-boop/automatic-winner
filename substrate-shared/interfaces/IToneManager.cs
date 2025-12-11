using System.Collections.Generic;
using substrate_shared.interfaces.Details;
using substrate_shared.Registries.enums;
using substrate_shared.structs;

namespace substrate_shared.interfaces
{
    public interface IToneManager : IManager
    {
        IToneCut Cut(IReadOnlyDictionary<ToneType,int> toneValues);
        ToneType DetermineDominant(BiasVector speakerA, BiasVector speakerB);
        bool CheckBalance(BiasVector speakerA, BiasVector speakerB);
        IToneCut ResolveToneCut(BiasVector persistentBiasVector, BiasVector opponent);
    }
}