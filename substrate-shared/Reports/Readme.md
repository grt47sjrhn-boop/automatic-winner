Perfect — here’s a polished **README.md draft** for your resilience reporting toolkit. It’s written to onboard collaborators quickly, explain the architecture, and highlight usage examples.

---

# Resilience Reporting Toolkit

A modular suite for tracking, analyzing, and exporting **resilience metrics** in narratable systems.  
This toolkit unifies **narrative overlays** (recoveries, collapses, wounds, conflicts, equilibria) with **mathematical overlays** (hypotenuse averages, cumulative areas, cosine/sine means, scaled indices) into structured reports.

---

## ✨ Features

- **ResilienceTracker**  
  Records duels, computes resilience indices, and maintains both narrative and math overlays.

- **ReportBuilder**  
  Produces structured `ResilienceReport` objects with counts and metrics.

- **Exporters & Importers**
    - JSON (single + batch)
    - CSV (single + batch)
    - Round‑trip support (export → save → reload)

- **File I/O Helpers**  
  Save/load reports directly from disk.

- **Timestamped & Directory Managers**  
  Automatic versioning and daily folder grouping.

- **Archiver**  
  Compress daily folders into `.zip` archives for storage or transfer.

- **Unified Facade (`ResilienceReportIO`)**  
  One entry point for building, exporting, saving, loading, and archiving.

---

## 📦 Installation

Include the toolkit in your project:

```bash
dotnet add package ResilienceToolkit
```

*(or reference the DLL directly if you’re distributing it as part of PRISMx)*

---

## 🚀 Quick Start

```csharp
var tracker = new ResilienceTracker();
tracker.Record(summary1, joy, despair);

// Build report
var report = ResilienceReportIO.Build(tracker);

// Export to JSON
string json = ResilienceReportIO.Export(report, "json");
Console.WriteLine(json);

// Save with timestamp
string savedFile = ResilienceReportIO.SaveWithTimestamp(report, "resilience_report", "json");
Console.WriteLine($"Saved as: {savedFile}");

// Save into dated directory
string datedPath = ResilienceReportIO.SaveToDateDirectory(report, "Reports", "csv");
Console.WriteLine($"Saved into: {datedPath}");

// Archive daily folder
string archivePath = ResilienceReportIO.ArchiveDirectory("Reports/2025-12-06");
Console.WriteLine($"Archived to: {archivePath}");
```

---

## 📊 Report Structure

Each `ResilienceReport` contains:

| Narrative Layer   | Math Overlay Layer |
|-------------------|--------------------|
| DuelCount         | AverageHypotenuse  |
| ResilienceIndex   | CumulativeArea     |
| RecoveryCount     | MeanCos            |
| CollapseCount     | MeanSin            |
| WoundCount        | LogScaledIndex     |
| ConflictCount     | ExpScaledIndex     |
| EquilibriumCount  |                    |

---

## 🛠 Contributing

- Fork the repo and create feature branches.
- Add unit tests for new helpers.
- Keep APIs consistent with the unified facade (`ResilienceReportIO`).
- Document new features in this README.

---

## 📄 License

MIT License — free to use, extend, and distribute.

---

This README gives collaborators a **clear map of the toolkit**: what it does, how to use it, and how to extend it.

Would you like me to also scaffold a **Contributor Guide** (separate from README) that explains coding conventions, naming rules, and extension patterns for new helpers?
