using System.Collections.Generic;
using substrate_shared.Registries.enums;
using substrate_shared.structs;

namespace substrate_shared.interfaces.Profiles
{
    public interface IDuelist
    {
        string Name { get; }
        double Bias { get; }
        double Resilience { get; }
        int Wounds { get; }
        int Recoveries { get; }
        BiasVector BiasVector { get; }
        double Difficulty { get; set; }

        /// <summary>
        /// Apply a duel outcome summary to update bias/resilience state.
        /// </summary>
        void ApplyOutcome(ISummary summary, double difficulty);

        /// <summary>
        /// Convert current duelist state into a BiasVector.
        /// </summary>
        BiasVector ToBiasVector();

        /// <summary>
        /// Seed duelist with a starting tone.
        /// </summary>
        void SeedTone(ToneType seedTone);

        /// <summary>
        /// String representation of duelist state.
        /// </summary>
        string ToString();
        
        void AddDuel(ISummary duel);
        IEnumerable<ISummary> GetAllDuels();

    }
}