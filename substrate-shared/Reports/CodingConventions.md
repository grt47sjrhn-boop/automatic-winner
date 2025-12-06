Here’s a **Contributor Guide** draft to complement your README. It’s designed to help collaborators extend and maintain the resilience reporting toolkit consistently.

---

# Contributor Guide

Welcome to the **Resilience Reporting Toolkit** contributor guide.  
This document outlines coding conventions, naming rules, and extension patterns to ensure consistency across the project.

---

## 🧩 Architecture Principles

- **Separation of Concerns**  
  - `ResilienceTracker` → core logic for recording duels and computing overlays.  
  - `ReportBuilder` → transforms tracker state into structured reports.  
  - `ResilienceReport` → plain data object with narrative + math fields.  
  - Helpers (`ReportIO`, `ReportFileIO`, etc.) → persistence, export/import, organization.  
  - `ResilienceReportIO` → unified facade for end‑user simplicity.

- **Extensibility**  
  - New helpers should plug into the facade (`ResilienceReportIO`) rather than stand alone.  
  - Keep core tracker logic lean; reporting and persistence belong in helpers.

---

## 📐 Coding Conventions

- **Naming**  
  - Classes: `PascalCase` (e.g., `ReportBuilder`, `ReportArchiver`).  
  - Methods: `PascalCase` (e.g., `BuildReport`, `SaveBatchWithTimestamp`).  
  - Properties: `PascalCase` (e.g., `ResilienceIndex`, `AverageHypotenuse`).  
  - Private fields: `_camelCase` (e.g., `_tracker`).  

- **Formatting**  
  - Use braces `{}` even for single‑line conditionals.  
  - Align assignment operators for readability in initializers.  
  - Keep methods short and focused — one responsibility each.

- **Error Handling**  
  - Throw meaningful exceptions (`ArgumentException`, `DirectoryNotFoundException`).  
  - Avoid silent failures; return clear messages or structured results.

---

## 🔄 Extension Patterns

- **Adding New Metrics**  
  - Extend `ResilienceReport` with new fields.  
  - Update `ReportBuilder.BuildReport()` to populate them.  
  - Ensure exporters (JSON, CSV) include the new fields.  
  - Add unit tests for serialization/deserialization.

- **Adding New Export Formats**  
  - Create a dedicated helper (e.g., `ReportExporterXml`).  
  - Add corresponding importer if round‑trip is needed.  
  - Wire into `ResilienceReportIO` facade for unified access.

- **Adding New Persistence Options**  
  - Follow the pattern of `ReportFileIO` and `ReportDirectoryManager`.  
  - Keep file naming/versioning logic separate from core report logic.  
  - Return actual file paths so users can log or display them.

---

## 🧪 Testing Guidelines

- **Unit Tests**  
  - Verify `ResilienceReport` fields populate correctly.  
  - Test JSON/CSV export/import round‑trip.  
  - Test file save/load with timestamp and directory grouping.  
  - Test archiving creates valid `.zip` files.

- **Integration Tests**  
  - Simulate full pipeline: tracker → report → export → save → load → archive.  
  - Validate consistency across formats.

---

## 📖 Documentation

- Update **README.md** when adding new features.  
- Provide usage examples for new helpers.  
- Keep contributor guide aligned with evolving architecture.

---

## ✅ Contribution Workflow

1. Fork the repository.  
2. Create a feature branch (`feature/new-helper`).  
3. Implement changes following conventions.  
4. Add unit tests.  
5. Update README and Contributor Guide.  
6. Submit a pull request with a clear description.

---

This guide ensures collaborators can extend the toolkit without breaking consistency or clarity.  

Would you like me to also draft a **sample unit test suite** (in xUnit or NUnit) so contributors have a ready template for testing new helpers?
