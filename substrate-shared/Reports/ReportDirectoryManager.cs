using System;
using System.Collections.Generic;
using System.IO;

namespace substrate_shared.Reports
{
    public static class ReportDirectoryManager
    {
        private static string EnsureDateDirectory(string baseDirectory)
        {
            var dateFolder = DateTime.Now.ToString("yyyy-MM-dd");
            var fullPath = Path.Combine(baseDirectory, dateFolder);

            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            return fullPath;
        }

        // Save single report into dated folder
        public static string SaveToDateDirectory(ResilienceReport report, string baseDirectory, string format = "json")
        {
            var directory = EnsureDateDirectory(baseDirectory);
            var extension = format.ToLowerInvariant() == "csv" ? ".csv" : ".json";
            var fileName = $"report_{DateTime.Now:HHmmss}{extension}";
            var fullPath = Path.Combine(directory, fileName);

            var data = ReportExporter.ReportIO.Export(report, format);
            File.WriteAllText(fullPath, data);

            return fullPath;
        }

        // Save batch of reports into dated folder
        public static string SaveBatchToDateDirectory(IEnumerable<ResilienceReport> reports, string baseDirectory, string format = "json")
        {
            var directory = EnsureDateDirectory(baseDirectory);
            var extension = format.ToLowerInvariant() == "csv" ? ".csv" : ".json";
            var fileName = $"reports_{DateTime.Now:HHmmss}{extension}";
            var fullPath = Path.Combine(directory, fileName);

            var data = ReportExporter.ReportIO.ExportBatch(reports, format);
            File.WriteAllText(fullPath, data);

            return fullPath;
        }
    }
}