using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace substrate_shared.Reports
{
    public static class ReportExporter
    {
        public static string ToJson(ResilienceReport report, bool indented = true)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = indented
            };
            return JsonSerializer.Serialize(report, options);
        }

        public static ResilienceReport FromJson(string json)
        {
            return JsonSerializer.Deserialize<ResilienceReport>(json)!;
        }

        public static class ReportExporterCsv
        {
            public static string ToCsv(ResilienceReport report, bool includeHeader = true)
            {
                var sb = new StringBuilder();

                if (includeHeader)
                {
                    sb.AppendLine(
                        "DuelCount,ResilienceIndex,RecoveryCount,CollapseCount,WoundCount,ConflictCount,EquilibriumCount," +
                        "AverageHypotenuse,CumulativeArea,MeanCos,MeanSin,LogScaledIndex,ExpScaledIndex");
                }

                sb.AppendLine(
                    $"{report.DuelCount}," +
                    $"{report.ResilienceIndex}," +
                    $"{report.RecoveryCount}," +
                    $"{report.CollapseCount}," +
                    $"{report.WoundCount}," +
                    $"{report.ConflictCount}," +
                    $"{report.EquilibriumCount}," +
                    $"{report.AverageHypotenuse:F2}," +
                    $"{report.CumulativeArea:F2}," +
                    $"{report.MeanCos:F2}," +
                    $"{report.MeanSin:F2}," +
                    $"{report.LogScaledIndex:F2}," +
                    $"{report.ExpScaledIndex:F2}"
                );

                return sb.ToString();
            }
        }

        public static class ReportExporterCsvBatch
        {
            public static string ToCsv(IEnumerable<ResilienceReport> reports, bool includeHeader = true)
            {
                var sb = new StringBuilder();

                if (includeHeader)
                {
                    sb.AppendLine(
                        "DuelCount,ResilienceIndex,RecoveryCount,CollapseCount,WoundCount,ConflictCount,EquilibriumCount," +
                        "AverageHypotenuse,CumulativeArea,MeanCos,MeanSin,LogScaledIndex,ExpScaledIndex");
                }

                foreach (var report in reports)
                {
                    sb.AppendLine(
                        $"{report.DuelCount}," +
                        $"{report.ResilienceIndex}," +
                        $"{report.RecoveryCount}," +
                        $"{report.CollapseCount}," +
                        $"{report.WoundCount}," +
                        $"{report.ConflictCount}," +
                        $"{report.EquilibriumCount}," +
                        $"{report.AverageHypotenuse:F2}," +
                        $"{report.CumulativeArea:F2}," +
                        $"{report.MeanCos:F2}," +
                        $"{report.MeanSin:F2}," +
                        $"{report.LogScaledIndex:F2}," +
                        $"{report.ExpScaledIndex:F2}"
                    );
                }

                return sb.ToString();
            }
        }

        public static class ReportImporterCsv
        {
            public static List<ResilienceReport> FromCsv(string csv, bool hasHeader = true)
            {
                var reports = new List<ResilienceReport>();
                var lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                var startIndex = hasHeader ? 1 : 0;

                for (var i = startIndex; i < lines.Length; i++)
                {
                    var parts = lines[i].Split(',');

                    if (parts.Length < 13) continue; // ensure all fields are present

                    var report = new ResilienceReport
                    {
                        DuelCount = int.Parse(parts[0]),
                        ResilienceIndex = int.Parse(parts[1]),
                        RecoveryCount = int.Parse(parts[2]),
                        CollapseCount = int.Parse(parts[3]),
                        WoundCount = int.Parse(parts[4]),
                        ConflictCount = int.Parse(parts[5]),
                        EquilibriumCount = int.Parse(parts[6]),
                        AverageHypotenuse = double.Parse(parts[7]),
                        CumulativeArea = double.Parse(parts[8]),
                        MeanCos = double.Parse(parts[9]),
                        MeanSin = double.Parse(parts[10]),
                        LogScaledIndex = double.Parse(parts[11]),
                        ExpScaledIndex = double.Parse(parts[12])
                    };

                    reports.Add(report);
                }

                return reports;
            }
        }

        public static class ReportIO
        {
            // Export to JSON or CSV
            public static string Export(ResilienceReport report, string format = "json", bool indented = true,
                bool includeHeader = true)
            {
                format = format.ToLowerInvariant();

                if (format == "json")
                {
                    var options = new JsonSerializerOptions { WriteIndented = indented };
                    return JsonSerializer.Serialize(report, options);
                }
                else if (format == "csv")
                {
                    return ReportExporterCsv.ToCsv(report, includeHeader);
                }
                else
                {
                    throw new ArgumentException($"Unsupported format: {format}");
                }
            }

            public static string ExportBatch(IEnumerable<ResilienceReport> reports, string format = "json",
                bool indented = true, bool includeHeader = true)
            {
                format = format.ToLowerInvariant();

                if (format == "json")
                {
                    var options = new JsonSerializerOptions { WriteIndented = indented };
                    return JsonSerializer.Serialize(reports, options);
                }
                else if (format == "csv")
                {
                    return ReportExporterCsvBatch.ToCsv(reports, includeHeader);
                }
                else
                {
                    throw new ArgumentException($"Unsupported format: {format}");
                }
            }

            // Import from JSON or CSV
            public static List<ResilienceReport> Import(string data, string format = "")
            {
                // Auto-detect if format not specified
                if (string.IsNullOrWhiteSpace(format))
                {
                    if (data.TrimStart().StartsWith("{") || data.TrimStart().StartsWith("["))
                        format = "json";
                    else
                        format = "csv";
                }

                format = format.ToLowerInvariant();

                if (format == "json")
                {
                    if (data.TrimStart().StartsWith("["))
                        return JsonSerializer.Deserialize<List<ResilienceReport>>(data)!;
                    else
                        return new List<ResilienceReport> { JsonSerializer.Deserialize<ResilienceReport>(data)! };
                }
                else if (format == "csv")
                {
                    return ReportImporterCsv.FromCsv(data);
                }
                else
                {
                    throw new ArgumentException($"Unsupported format: {format}");
                }
            }
        }

        public static class ReportFileIO
        {
            // Save single report
            public static void Save(ResilienceReport report, string filePath, string format = "json")
            {
                var data = ReportIO.Export(report, format);
                File.WriteAllText(filePath, data);
            }

            // Save batch of reports
            public static void SaveBatch(IEnumerable<ResilienceReport> reports, string filePath, string format = "json")
            {
                var data = ReportIO.ExportBatch(reports, format);
                File.WriteAllText(filePath, data);
            }

            // Load reports (auto-detect format if not specified)
            public static List<ResilienceReport> Load(string filePath, string format = "")
            {
                var data = File.ReadAllText(filePath);
                return ReportIO.Import(data, format);
            }
        }
        
        public static class ReportFileIOWithTimestamp
        {
            private static string AddTimestamp(string baseFileName, string extension)
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                return $"{Path.GetFileNameWithoutExtension(baseFileName)}_{timestamp}{extension}";
            }

            // Save single report with timestamped filename
            public static string SaveWithTimestamp(ResilienceReport report, string baseFileName, string format = "json")
            {
                var extension = format.ToLowerInvariant() == "csv" ? ".csv" : ".json";
                var fileName = AddTimestamp(baseFileName, extension);

                var data = ReportIO.Export(report, format);
                File.WriteAllText(fileName, data);

                return fileName; // return actual saved filename
            }

            // Save batch of reports with timestamped filename
            public static string SaveBatchWithTimestamp(IEnumerable<ResilienceReport> reports, string baseFileName, string format = "json")
            {
                var extension = format.ToLowerInvariant() == "csv" ? ".csv" : ".json";
                var fileName = AddTimestamp(baseFileName, extension);

                var data = ReportIO.ExportBatch(reports, format);
                File.WriteAllText(fileName, data);

                return fileName; // return actual saved filename
            }
        }
    }
}

