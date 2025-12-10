using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace substrate_shared.Reports
{
    public static class ReportArchiver
    {
        // Archive a specific directory into a zip file
        public static string ArchiveDirectory(string sourceDirectory, string archiveBaseName = "reports_archive")
        {
            if (!Directory.Exists(sourceDirectory))
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDirectory}");

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var archiveFile = $"{archiveBaseName}_{timestamp}.zip";

            ZipFile.CreateFromDirectory(sourceDirectory, archiveFile);

            return archiveFile; // return path to created archive
        }

        // Archive all subdirectories inside a base folder (e.g., daily folders)
        public static List<string> ArchiveAllSubdirectories(string baseDirectory, string archiveBaseName = "reports_archive")
        {
            var archives = new List<string>();

            if (!Directory.Exists(baseDirectory))
                throw new DirectoryNotFoundException($"Base directory not found: {baseDirectory}");

            foreach (var dir in Directory.GetDirectories(baseDirectory))
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var archiveFile = $"{archiveBaseName}_{Path.GetFileName(dir)}_{timestamp}.zip";

                ZipFile.CreateFromDirectory(dir, archiveFile);
                archives.Add(archiveFile);
            }

            return archives;
        }
    }
}