using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using substrate_shared.interfaces.core;
using substrate_shared.Traits.Base;
using substrate_shared.Traits.Types;

namespace substrate_shared.Reports
{
    public static class ReportExporter
    {
        public static string ToJson(IResilienceReport report, bool indented = true)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = indented
            };
            return JsonSerializer.Serialize(report, options);
        }

        public static IResilienceReport FromJson(string json)
        {
            return JsonSerializer.Deserialize<IResilienceReport>(json)!;
        }

        public static class ReportExporterCsv
        {
            public static string ToCsv(IResilienceReport report, bool includeHeader = true)
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
            public static string ToCsv(IEnumerable<IResilienceReport> reports, bool includeHeader = true)
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
            public static List<IResilienceReport> FromCsv(string csv, bool hasHeader = true)
            {
                var reports = new List<IResilienceReport>();
                var lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                var startIndex = hasHeader ? 1 : 0;

                for (var i = startIndex; i < lines.Length; i++)
                {
                    var parts = lines[i].Split(',');

                    if (parts.Length < 13) continue; // ensure all fields are present

                    // Construct concrete report, expose as interface
                    var report = new ResilienceReport();
                    report.SetMetrics(
                        duelCount: int.Parse(parts[0]),
                        resilienceIndex: double.Parse(parts[1]),
                        totalResilience: 0, // placeholder if not in CSV
                        recoveryCount: int.Parse(parts[2]),
                        collapseCount: int.Parse(parts[3]),
                        woundCount: int.Parse(parts[4]),
                        conflictCount: int.Parse(parts[5]),
                        equilibriumCount: int.Parse(parts[6]),
                        avgHypotenuse: double.Parse(parts[7]),
                        cumulativeArea: double.Parse(parts[8]),
                        meanCos: double.Parse(parts[9]),
                        meanSin: double.Parse(parts[10]),
                        logScaledIndex: double.Parse(parts[11]),
                        expScaledIndex: double.Parse(parts[12]),
                        toneDistribution: new Dictionary<string,int>(),
                        intentDistribution: new Dictionary<string,int>(),
                        crystalGroups: new List<TraitCrystalGroup>(),
                        crystals: new List<TraitCrystal>(),
                        rarityCounts: new Dictionary<string,int>(),
                        crystalNarratives: new List<string>(),
                        biasSummaries: new List<string>(),
                        crystalCount: 0,
                        outcomes: new Dictionary<string,int>(),
                        crystalRarity: new Dictionary<string,int>(),
                        brillianceCuts: new Dictionary<string,int>()
                    );

                    reports.Add(report); // stored as IResilienceReport
                }

                return reports;
            }
        }
        
        public static class ReportIO
        {
            // Export to JSON or CSV
            public static string Export(IResilienceReport report, string format = "json", bool indented = true,
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

            public static string ExportBatch(IEnumerable<IResilienceReport> reports, string format = "json",
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
            public static List<IResilienceReport> Import(string data, string format = "")
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
                        return JsonSerializer.Deserialize<List<IResilienceReport>>(data)!;
                    else
                        return new List<IResilienceReport> { JsonSerializer.Deserialize<IResilienceReport>(data)! };
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
            public static void Save(IResilienceReport report, string filePath, string format = "json")
            {
                var data = ReportIO.Export(report, format);
                File.WriteAllText(filePath, data);
            }

            // Save batch of reports
            public static void SaveBatch(IEnumerable<IResilienceReport> reports, string filePath, string format = "json")
            {
                var data = ReportIO.ExportBatch(reports, format);
                File.WriteAllText(filePath, data);
            }

            // Load reports (auto-detect format if not specified)
            public static List<IResilienceReport> Load(string filePath, string format = "")
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
            public static string SaveWithTimestamp(IResilienceReport report, string baseFileName, string format = "json")
            {
                var extension = format.ToLowerInvariant() == "csv" ? ".csv" : ".json";
                var fileName = AddTimestamp(baseFileName, extension);

                var data = ReportIO.Export(report, format);
                File.WriteAllText(fileName, data);

                return fileName; // return actual saved filename
            }

            // Save batch of reports with timestamped filename
            public static string SaveBatchWithTimestamp(IEnumerable<IResilienceReport> reports, string baseFileName, string format = "json")
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

