// substrate_core/Codex/Core/CodexSpine.cs
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

        // Other services (Library, EpochScheduler, etc.)
        public CodexLibraryService Library { get; private set; }
        public EpochSchedulerService EpochScheduler { get; private set; }
        public Services.IntentBrokerService IntentBroker { get; private set; }
        public Services.OverlayManagerService OverlayManager { get; private set; }
        public Services.MetricTranslatorService MetricTranslator { get; private set; }
        public Services.CodexEntryGeneratorService EntryGenerator { get; private set; }
        public Services.ResilienceReporterService ResilienceReporter { get; private set; }

        public void Initialize()
        {
            Context.Initialize();

            Library = new Services.CodexLibraryService();
            EpochScheduler = new Services.EpochSchedulerService();
            IntentBroker = new Services.IntentBrokerService();
            OverlayManager = new Services.OverlayManagerService();
            MetricTranslator = new Services.MetricTranslatorService();
            EntryGenerator = new Services.CodexEntryGeneratorService();
            ResilienceReporter = new Services.ResilienceReporterService();

            Library.Initialize();
            EpochScheduler.Initialize();
            IntentBroker.Initialize();
            OverlayManager.Initialize();
            MetricTranslator.Initialize();
            EntryGenerator.Initialize();
            ResilienceReporter.Initialize();
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
        }

        /// <summary>
        /// Populate Codex context once with the active report, summary, duelist, and inventory.
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
    }
}