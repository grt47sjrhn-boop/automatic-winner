Great — here’s a **technical API reference** for your resilience reporting toolkit. This complements the README and Contributor Guide, giving collaborators a quick lookup table of method signatures, inputs, and outputs.

---

# 📖 API Reference: Resilience Reporting Toolkit

## Core Classes

### `ResilienceTracker`
- **Purpose**: Records duels and computes resilience overlays.
- **Key Methods**:
    - `Record(ISummary summary, AxisType axisA, AxisType axisB)`  
      Records a duel with narrative + math overlays.
    - `Record(MultiAxisSummary summary)`  
      Records a multi‑axis duel.

---

### `ReportBuilder`
- **Purpose**: Builds structured reports from tracker data.
- **Key Methods**:
    - `BuildReport()` → `ResilienceReport`  
      Produces a report with narrative counts and math overlays.

---

### `ResilienceReport`
- **Purpose**: Data object containing resilience metrics.
- **Fields**:
    - Narrative: `DuelCount`, `ResilienceIndex`, `RecoveryCount`, `CollapseCount`, `WoundCount`, `ConflictCount`, `EquilibriumCount`
    - Math: `AverageHypotenuse`, `CumulativeArea`, `MeanCos`, `MeanSin`, `LogScaledIndex`, `ExpScaledIndex`

---

## Unified Facade

### `ResilienceReportIO`
- **Purpose**: Single entry point for building, exporting, saving, loading, and archiving reports.

#### Build
- `Build(IResilienceTracker tracker)` → `ResilienceReport`

#### Export
- `Export(ResilienceReport report, string format = "json", bool indented = true, bool includeHeader = true)` → `string`
- `ExportBatch(IEnumerable<ResilienceReport> reports, string format = "json", bool indented = true, bool includeHeader = true)` → `string`

#### Import
- `Import(string data, string format = "")` → `List<ResilienceReport>`

#### File I/O
- `Save(ResilienceReport report, string filePath, string format = "json")` → `string`
- `SaveBatch(IEnumerable<ResilienceReport> reports, string filePath, string format = "json")` → `string`
- `Load(string filePath, string format = "")` → `List<ResilienceReport>`

#### Timestamped Save
- `SaveWithTimestamp(ResilienceReport report, string baseFileName, string format = "json")` → `string`
- `SaveBatchWithTimestamp(IEnumerable<ResilienceReport> reports, string baseFileName, string format = "json")` → `string`

#### Directory Save
- `SaveToDateDirectory(ResilienceReport report, string baseDirectory, string format = "json")` → `string`
- `SaveBatchToDateDirectory(IEnumerable<ResilienceReport> reports, string baseDirectory, string format = "json")` → `string`

#### Archiving
- `ArchiveDirectory(string sourceDirectory, string archiveBaseName = "reports_archive")` → `string`
- `ArchiveAllSubdirectories(string baseDirectory, string archiveBaseName = "reports_archive")` → `List<string>`

---

## ✅ Notes for Contributors
- **Format Options**: `"json"` or `"csv"`.
- **Auto‑Detection**: `Import` and `Load` detect format if not specified.
- **Return Values**: Save methods return the actual file path; archive methods return archive paths.
- **Error Handling**: Throws exceptions for invalid directories or unsupported formats.

---

This API reference gives collaborators a **quick lookup table** of everything they can call, without digging into implementation.

Would you like me to also generate a **cheat sheet diagram** (ASCII flowchart) showing the pipeline from `ResilienceTracker` → `ReportBuilder` → `ResilienceReportIO` → Export/Save/Archive?
