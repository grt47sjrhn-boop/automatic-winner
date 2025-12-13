using System;
using substrate_shared.interfaces;
using substrate_shared.interfaces.Codex;
using substrate_shared.interfaces.core;
using substrate_shared.interfaces.Profiles;

namespace substrate_shared.Services.Codex
{
    /// <summary>
    /// Singleton service that stashes Codex context objects
    /// for engine access (Report, Summary, Duelist, Inventory).
    /// Implements ICodexContextService and IService.
    /// </summary>
    public sealed class CodexContextService : ICodexContextService
    {
        private static readonly Lazy<CodexContextService> _instance =
            new(() => new CodexContextService());

        public static CodexContextService Instance => _instance.Value;

        private CodexContextService() { }

        public IResilienceReport Report { get; private set; }
        public ISummary Summary { get; private set; }
        public IDuelist Duelist { get; private set; }
        public IManager Inventory { get; private set; }

        // --- Setters ---
        public void SetReport(IResilienceReport report) => Report = report;
        public void SetSummary(ISummary summary) => Summary = summary;
        public void SetDuelist(IDuelist duelist) => Duelist = duelist;
        public void SetInventory(IManager inventory) => Inventory = inventory;

        // --- Clear all context ---
        public void Clear()
        {
            Report = null;
            // Summary = null;
            Duelist = null;
            Inventory = null;
        }

        // --- IService lifecycle methods ---
        public void Initialize()
        {
            // Optional setup logic (e.g., preload defaults)
        }

        public void Reset()
        {
            Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}