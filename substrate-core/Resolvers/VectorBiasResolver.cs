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
        public List<string> TraceLog { get; } = [];
        
        public VectorBiasResolver()
        {
            _pipeline =
            [
                new TraitsResolver(),      //1st
                new DeltaVectorResolver(), //2st
                new PersistenceResolver(), //3rd
                new VolatilityResolver(),  //4th
                new ToneClusterResolver(), //5th
                new TriggerResolver(),     //6th  
                new PersonalityResolver(), //7th
                new IntentActionResolver() //8th
            ];
        }

        public ResolutionResult Resolve(VectorBias vb, Mood mv)
        {
            ResolutionResult lastResult = default;

            foreach (var resolver in _pipeline)
            {
                lastResult = resolver.Resolve(vb, mv);
                vb = lastResult.Bias;
                TraceLog.Add($"{resolver.GetType().Name} resolved at tick {vb.TickId}");
                
            }

            lastResult.TraceLog = TraceLog;
            // Return the final resolution result after the pipeline
            return lastResult;
        }
    }
}