using System.Collections.Generic;
using substrate_core.Reporting;
using substrate_shared.interfaces;

namespace substrate_shared.Reports
{
    public static class ResilienceReportIo
    {
        // Build a report from tracker
        public static ResilienceReport Build(IResilienceTracker tracker)
        {
            var builder = new ReportBuilder(tracker);
            return builder.BuildReport();
        }

        // Export single report
        public static string Export(ResilienceReport report, string format = "json", bool indented = true, bool includeHeader = true)
        {
            return ReportExporter.ReportIO.Export(report, format, indented, includeHeader);
        }

        // Export batch
        public static string ExportBatch(IEnumerable<ResilienceReport> reports, string format = "json", bool indented = true, bool includeHeader = true)
        {
            return ReportExporter.ReportIO.ExportBatch(reports, format, indented, includeHeader);
        }

        // Import reports
        public static List<ResilienceReport> Import(string data, string format = "")
        {
            return ReportExporter.ReportIO.Import(data, format);
        }

        // Save single report to file
        public static string Save(ResilienceReport report, string filePath, string format = "json")
        {
            ReportExporter.ReportFileIO.Save(report, filePath, format);
            return filePath;
        }

        // Save batch to file
        public static string SaveBatch(IEnumerable<ResilienceReport> reports, string filePath, string format = "json")
        {
            ReportExporter.ReportFileIO.SaveBatch(reports, filePath, format);
            return filePath;
        }

        // Load reports from file
        public static List<ResilienceReport> Load(string filePath, string format = "")
        {
            return ReportExporter.ReportFileIO.Load(filePath, format);
        }

        // Save with timestamp
        public static string SaveWithTimestamp(ResilienceReport report, string baseFileName, string format = "json")
        {
            return ReportExporter.ReportFileIOWithTimestamp.SaveWithTimestamp(report, baseFileName, format);
        }

        public static string SaveBatchWithTimestamp(IEnumerable<ResilienceReport> reports, string baseFileName, string format = "json")
        {
            return ReportExporter.ReportFileIOWithTimestamp.SaveBatchWithTimestamp(reports, baseFileName, format);
        }

        // Save into dated directory
        public static string SaveToDateDirectory(ResilienceReport report, string baseDirectory, string format = "json")
        {
            return ReportDirectoryManager.SaveToDateDirectory(report, baseDirectory, format);
        }

        public static string SaveBatchToDateDirectory(IEnumerable<ResilienceReport> reports, string baseDirectory, string format = "json")
        {
            return ReportDirectoryManager.SaveBatchToDateDirectory(reports, baseDirectory, format);
        }

        // Archive helpers
        public static string ArchiveDirectory(string sourceDirectory, string archiveBaseName = "reports_archive")
        {
            return ReportArchiver.ArchiveDirectory(sourceDirectory, archiveBaseName);
        }

        public static List<string> ArchiveAllSubdirectories(string baseDirectory, string archiveBaseName = "reports_archive")
        {
            return ReportArchiver.ArchiveAllSubdirectories(baseDirectory, archiveBaseName);
        }
    }
}