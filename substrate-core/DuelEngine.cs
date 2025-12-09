using substrate_core.Managers;
using substrate_shared.Engagements.Types;
using substrate_shared.interfaces;
using substrate_shared.Managers;
using substrate_shared.Profiles;
using substrate_shared.structs;
using EnvironmentMood = substrate_shared.Environment.EnvironmentMood;

namespace substrate_core
{
    /// <summary>
    /// Engine that runs duels for a persistent duelist and records outcomes.
    /// </summary>
    public class DuelEngine
    {
        private readonly IResilienceTracker _tracker;
        private readonly Duelist _persistent;
        private readonly IBiasManager _biasManager;
        private readonly IFacetManager _facetManager;
        private readonly IToneManager _toneManager;
        private readonly IRarityManager _rarityManager;
        private readonly InventoryManager _inventory;

        public DuelEngine(
            IResilienceTracker tracker,
            Duelist persistent,
            IBiasManager biasManager,
            IFacetManager facetManager,
            IToneManager toneManager,
            IRarityManager rarityManager,
            InventoryManager inventory)
        {
            _tracker       = tracker;
            _persistent    = persistent;
            _biasManager   = biasManager;
            _facetManager  = facetManager;
            _toneManager   = toneManager;
            _rarityManager = rarityManager;
            _inventory     = inventory;
        }

        /// <summary>
        /// Run a number of duel ticks.
        /// </summary>
        public void Tick(int count)
        {
            for (int i = 0; i < count; i++)
            {
                // 🔹 Instead of the simple generator:
                // var opponent = _biasManager.GenerateOpponent();

                // 🔹 Use tuned generator with profile + mood
                var profile = OpponentProfiles.Challenge; // or StoryMode/Nightmare
                var mood    = EnvironmentMood.Storm;      // or Sanctuary/Void/Carnival
                BiasDescriptor? tilt = null;              // optional seed tilt

                var opponent = _biasManager.GenerateOpponentWeighted(profile, mood, tilt);

                // Continue duel as before
                var duel = new DuelEngagement(
                    _inventory,
                    _persistent.BiasVector,
                    opponent,
                    _biasManager,
                    _facetManager,
                    _toneManager,
                    _rarityManager
                );

                duel.Resolve();
                var summary = duel.Finalize();

                _tracker.AddSummary(summary);
                foreach (var crystal in _inventory.GetCrystals())
                {
                    _tracker.AddCrystal(crystal);
                }

                _persistent.ApplyOutcome(summary);
            }
        }
    }
}