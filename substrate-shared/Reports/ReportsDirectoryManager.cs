using System;
using System.Collections.Generic;
using System.IO;
using substrate_shared.Reports;

namespace substrate_core.Reporting
{
    public static class ReportDirectoryManager
    {
        private static string EnsureDateDirectory(string baseDirectory)
        {
            string dateFolder = DateTime.Now.ToString("yyyy-MM-dd");
            string fullPath = Path.Combine(baseDirectory, dateFolder);

            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            return fullPath;
        }

        // Save single report into dated folder
        public static string SaveToDateDirectory(ResilienceReport report, string baseDirectory, string format = "json")
        {
            string directory = EnsureDateDirectory(baseDirectory);
            string extension = format.ToLowerInvariant() == "csv" ? ".csv" : ".json";
            string fileName = $"report_{DateTime.Now:HHmmss}{extension}";
            string fullPath = Path.Combine(directory, fileName);

            string data = ReportExporter.ReportIO.Export(report, format);
            File.WriteAllText(fullPath, data);

            return fullPath;
        }

        // Save batch of reports into dated folder
        public static string SaveBatchToDateDirectory(IEnumerable<ResilienceReport> reports, string baseDirectory, string format = "json")
        {
            string directory = EnsureDateDirectory(baseDirectory);
            string extension = format.ToLowerInvariant() == "csv" ? ".csv" : ".json";
            string fileName = $"reports_{DateTime.Now:HHmmss}{extension}";
            string fullPath = Path.Combine(directory, fileName);

            string data = ReportExporter.ReportIO.ExportBatch(reports, format);
            File.WriteAllText(fullPath, data);

            return fullPath;
        }
    }
}