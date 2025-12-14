using substrate_core.Internal.Engines;
using substrate_shared.interfaces;
using substrate_shared.interfaces.Codex;
using substrate_shared.interfaces.core;
using substrate_shared.interfaces.Profiles;
using substrate_shared.Services.Codex;

namespace substrate_core.Codex.Core
{
    public sealed class CodexSpine
    {
        private static readonly CodexSpine _instance = new CodexSpine();
        public static CodexSpine Instance => _instance;

        private CodexSpine() { }

        // Context service reference
        public CodexContextService Context => CodexContextService.Instance;

        // Services
        public CodexLibraryService Library { get; private set; }
        public EpochSchedulerService EpochScheduler { get; private set; }
        public IntentBrokerService IntentBroker { get; private set; }
        public OverlayManagerService OverlayManager { get; private set; }
        public MetricTranslatorService MetricTranslator { get; private set; }
        public CodexEntryGeneratorService EntryGenerator { get; private set; }
        public ResilienceReporterService ResilienceReporter { get; private set; }
        public ProfileRegistryService ProfileRegistry { get; private set; }

        // Catalyst Engine
        public CatalystEngine Catalyst { get; private set; }

        public void Initialize()
        {
            // Initialize context service
            Context.Initialize();

            // Spin up services
            Library = new CodexLibraryService();
            EpochScheduler = new EpochSchedulerService();
            IntentBroker = new IntentBrokerService();
            OverlayManager = new OverlayManagerService();
            MetricTranslator = new MetricTranslatorService();
            EntryGenerator = new CodexEntryGeneratorService();
            ResilienceReporter = new ResilienceReporterService();
            ProfileRegistry = new ProfileRegistryService();

            // Catalyst starts enabled
            Catalyst = new CatalystEngine(enabled: true);

            // Initialize services
            Library.Initialize();
            EpochScheduler.Initialize();
            IntentBroker.Initialize();
            OverlayManager.Initialize();
            MetricTranslator.Initialize();
            EntryGenerator.Initialize();
            ResilienceReporter.Initialize();
            ProfileRegistry.Initialize();
        }

        public void Shutdown()
        {
            Context.Dispose();
            EpochScheduler.Shutdown();
            ResilienceReporter.Shutdown();
            EntryGenerator.Shutdown();
            MetricTranslator.Shutdown();
            OverlayManager.Shutdown();
            IntentBroker.Shutdown();
            Library.Shutdown();
            ProfileRegistry.Shutdown();

            Catalyst.Toggle(false);
        }

        /// <summary>
        /// Populate Codex context with the active report, summary, duelist, and inventory.
        /// </summary>
        public void PopulateContext(
            IResilienceReport report,
            ISummary summary,
            IDuelist duelist,
            IManager inventory)
        {
            Context.SetReport(report);
            Context.SetSummary(summary);
            Context.SetDuelist(duelist);
            Context.SetInventory(inventory);
        }

        /// <summary>
        /// End-of-cycle evaluation: prompt Catalyst to enrich overlays.
        /// </summary>
        public void FinalizeCycle()
        {
            Catalyst.Apply();
        }
    }
}