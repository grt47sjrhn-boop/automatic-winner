using System.Collections.Generic;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_core.Resolvers
{
    /// <summary>
    /// VectorBiasResolver orchestrates the full resolver pipeline for a single tick.
    /// It runs all sub-resolvers in sequence and aggregates their outputs into a unified ResolutionResult.
    /// </summary>
    public class VectorBiasResolver : IResolver
    {
        private readonly List<IResolver> _pipeline;

        public VectorBiasResolver()
        {
            _pipeline = new List<IResolver>
            {
                new DeltaVectorResolver(), //1st
                new PersistenceResolver(), //2nd
                new ToneClusterResolver(), //3rd
                new VolatilityResolver(),  //4th
                new TriggerResolver(),     //5th  
                new PersonalityResolver(), //6th
                new IntentActionResolver() //7th
            };
        }

        public ResolutionResult Resolve(VectorBias vb, Mood mv)
        {
            ResolutionResult lastResult = default;

            foreach (var resolver in _pipeline)
            {
                lastResult = resolver.Resolve(vb, mv);
                vb = lastResult.Bias;
            }

            // Return the final resolution result after the pipeline
            return lastResult;
        }
    }
}