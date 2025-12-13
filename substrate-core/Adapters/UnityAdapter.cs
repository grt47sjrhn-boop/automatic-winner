using substrate_core;
using substrate_core.Helpers;
using substrate_core.Internal.Engines;
using substrate_core.Internal.Resolvers;
using substrate_core.Managers;
using substrate_core.Models;
using substrate_core.Overlays; // 🔹 for GeometryOverlay, TrigOverlay
using substrate_shared.interfaces;
using substrate_shared.interfaces.core;
using substrate_shared.interfaces.Managers;
using substrate_shared.interfaces.Overlays; // 🔹 for IGeometryOverlay, ITrigOverlay
using substrate_shared.Reports;
using substrate_shared.Registries.enums;

namespace substrate_core.Adapters
{
    public class UnityAdapter : IUnityAdapter
    {
        private readonly IBiasManager _biasManager;
        private readonly IFacetManager _facetManager;
        private readonly IToneManager _toneManager;
        private readonly IRarityManager _rarityManager;
        private readonly InventoryManager _inventory;
        private readonly IResilienceTracker _tracker;
        private readonly Duelist _persistent;
        private readonly DuelEngine _engine;

        // 🔹 Overlays
        private readonly IGeometryOverlay _geometryOverlay;
        private readonly ITrigOverlay _trigOverlay;

        public UnityAdapter(double seedBias = 0.0, ToneType? seedTone = null, double difficulty = 1.0)
        {
            _biasManager   = new BiasManager();
            _facetManager  = new FacetManager();
            _toneManager   = new ToneManager();
            _rarityManager = new RarityManager();
            _inventory     = new InventoryManager();
            _tracker       = new ResilienceTracker();

            // 🔹 Instantiate overlays
            _geometryOverlay = new GeometryOverlay();
            _trigOverlay     = new TrigOverlay(_geometryOverlay);

            _persistent = new Duelist("Persistent Hero", initialBias: seedBias);
            if (seedTone.HasValue)
            {
                _persistent.SeedTone(seedTone.Value);
            }

            _engine = new DuelEngine(
                _tracker,
                _persistent,
                _biasManager,
                _facetManager,
                _toneManager,
                _rarityManager,
                _inventory,
                _geometryOverlay, // 🔹 pass overlays
                _trigOverlay,
                difficulty
            );
        }

        public IDuelResult RunDuel(int tickCount = 1, bool verbose = false, bool export = false, string exportFormat = "json")
        {
            _engine.Tick(tickCount);

            var latestSummary    = _tracker.DuelSummaries[^1];
            var resilienceReport = ReportsIO.Build(_tracker, _inventory);

            if (export)
            {
                ReportsIO.SaveWithTimestamp(resilienceReport, "report", exportFormat);
            }

            return new DuelResult
            {
                Summary         = latestSummary,
                Report          = resilienceReport,
                PersistentState = _persistent
            };
        }
    }
}