using System;
using System.Collections.Generic;
using substrate_shared.Factories;
using substrate_shared.interfaces;
using substrate_shared.Profiles;
using substrate_shared.structs;

namespace substrate_core
{
    public class DuelEngine
    {
        private readonly IResilienceTracker _tracker;
        private readonly Duelist _persistentDuelist;
        private readonly Random _rng = new Random();

        public DuelEngine(IResilienceTracker tracker, Duelist persistent)
        {
            _tracker = tracker;
            _persistentDuelist = persistent;
        }

        public void Tick(int duelCount = 1)
        {
            for (var i = 0; i < duelCount; i++)
            {
                // Spin up a random opponent
                var opponent = DuelistFactory.CreateRandom();

                // Build vector list: persistent + opponent
                var vectors = new List<BiasVector>
                {
                    _persistentDuelist.ToBiasVector(),
                    opponent.ToBiasVector()
                };

                // Choose resolver type (Simple for 2 vectors, MultiAxis otherwise)
                var resolverType = vectors.Count == 2 ? ResolverType.Simple : ResolverType.MultiAxis;

                // Create resolver
                var resolver = ResolverFactory.CreateResolver(resolverType, vectors);

                // Resolve duel -> ISummary (EventSummary or DuelEventSummary)
                var summary = resolver.Resolve();

                // Record into tracker (using tones instead of AxisA/B)
                _tracker.Record(summary,
                    vectors[0],
                    vectors[1]);

                // Persist results back into the persistent duelist
                _persistentDuelist.ApplyOutcome(summary);
            }
        }
    }
}