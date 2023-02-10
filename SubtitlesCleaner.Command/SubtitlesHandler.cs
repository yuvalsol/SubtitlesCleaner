using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SubtitlesCleaner.Library;

namespace SubtitlesCleaner.Command
{
    static class SubtitlesHandler
    {
        public static readonly bool IsProduction = true;
        private static readonly bool IsPrintCleaning = false;
        private static readonly bool CleanHICaseInsensitive = false;
        private static readonly int? FirstSubtitlesCount = null;

        public static void Debug()
        {
            Clean(new CleanOptions()
            {
                path = Path.GetFullPath(Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "..", "..", "..", "Subtitles",
                    "Test.srt"
                )),
                cleanHICaseInsensitive = CleanHICaseInsensitive,
                firstSubtitlesCount = FirstSubtitlesCount,
                print = true
            });
        }

        public static void Clean(CleanOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
            {
                Encoding encoding = Encoding.UTF8;
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(
                    filePath,
                    ref encoding,
                    options.firstSubtitlesCount ?? FirstSubtitlesCount
                );

                subtitles = subtitles.CleanSubtitles(options.cleanHICaseInsensitive || CleanHICaseInsensitive, IsPrintCleaning);

                if (options.save)
                    Save(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, options.suppressErrorFile);

                if (options.print)
                    Print(subtitles);
            }
        }

        public static void CleanEmptyAndNonSubtitles(CleanEmptyAndNonSubtitlesOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
            {
                Encoding encoding = Encoding.UTF8;
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(
                    filePath,
                    ref encoding,
                    options.firstSubtitlesCount ?? FirstSubtitlesCount
                );

                subtitles = subtitles.CleanEmptyAndNonSubtitles(IsPrintCleaning);

                if (options.save)
                    Save(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, true);

                if (options.print)
                    Print(subtitles);
            }
        }

        public static void AddTime(AddTimeOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
            {
                Encoding encoding = Encoding.UTF8;
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(
                    filePath,
                    ref encoding,
                    options.firstSubtitlesCount ?? FirstSubtitlesCount
                );

                subtitles.AddTime(options.timeAdded, options.subtitleNumber);

                if (options.save)
                    Save(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, true);

                if (options.print)
                    Print(subtitles);
            }
        }

        public static void SetShowTime(SetShowTimeOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
            {
                Encoding encoding = Encoding.UTF8;
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(
                    filePath,
                    ref encoding,
                    options.firstSubtitlesCount ?? FirstSubtitlesCount
                );

                subtitles.SetShowTime(options.showTime, options.subtitleNumber);

                if (options.save)
                    Save(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, true);

                if (options.print)
                    Print(subtitles);
            }
        }

        public static void AdjustTiming(AdjustTimingOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
            {
                Encoding encoding = Encoding.UTF8;
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(
                    filePath,
                    ref encoding,
                    options.firstSubtitlesCount ?? FirstSubtitlesCount
                );

                subtitles.AdjustTiming(options.firstShowTime, options.lastShowTime);

                if (options.save)
                    Save(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, true);

                if (options.print)
                    Print(subtitles);
            }
        }

        public static void Reorder(ReorderOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
            {
                Encoding encoding = Encoding.UTF8;
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(filePath, ref encoding);

                subtitles = subtitles.Reorder();

                if (options.save)
                    Save(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, true);

                if (options.print)
                    Print(subtitles);
            }
        }

        public static void BalanceLines(BalanceLinesOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
            {
                Encoding encoding = Encoding.UTF8;
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(filePath, ref encoding);

                subtitles = subtitles.BalanceLines();

                if (options.save)
                    Save(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, true);

                if (options.print)
                    Print(subtitles);
            }
        }

        private static string[] GetFilePaths(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            if (path.EndsWith(":\""))
                path = path.Replace(":\"", ":");

            bool isRecursive = false;
            string[] filePaths = GetFiles(path, isRecursive);

            return filePaths;
        }

        private static string[] GetFiles(string path, bool isRecursive)
        {
            string[] filePaths = null;

            string extension = Path.GetExtension(path);
            bool isSRTFile = string.Compare(extension, ".srt", true) == 0;
            if (isSRTFile)
            {
                if (File.Exists(path))
                    filePaths = new string[] { path };
            }
            else if (Directory.Exists(path))
            {
                List<string> lst = new List<string>(Directory.GetFiles(path, "*.srt", (isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)));
                var bakFiles = lst.Where(x => x.EndsWith(".bak.srt")).ToArray();
                foreach (var bakFile in bakFiles)
                {
                    lst.Remove(bakFile);
                    lst.Remove(bakFile.Replace(".bak.srt", ".srt"));
                }
                filePaths = lst.ToArray();
            }

            return filePaths;
        }

        private static void Save(
            List<Subtitle> subtitles,
            Encoding encoding,
            string filePath,
            string outputFile,
            string outputFolder,
            bool suppressBackupFile,
            bool suppressErrorFile)
        {
            string outputFilePath = GetOutputFilePath(filePath, outputFile, outputFolder);

            CreateOutputFolder(outputFilePath);

            if (suppressBackupFile == false)
            {
                string backupFile = outputFilePath.Replace(".srt", ".bak.srt");

                try
                {
                    File.Copy(filePath, backupFile, true);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to save backup file " + backupFile, ex);
                }
            }

            try
            {
                string[] lines = subtitles.ToLines();
                File.WriteAllLines(outputFilePath, lines.Take(lines.Length - 1), encoding);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save subtitles file " + outputFilePath, ex);
            }

            if (suppressErrorFile == false)
            {
                string[] errors = SubtitlesHelper.GetSubtitlesErrors(subtitles);

                if (errors != null && errors.Length > 0)
                {
                    string errorFile = outputFilePath.Replace(".srt", ".error.srt");

                    try
                    {
                        File.WriteAllLines(errorFile, errors, encoding);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Failed to save error file" + errorFile, ex);
                    }
                }
            }
        }

        private static string GetOutputFilePath(string filePath, string outputFile, string outputFolder)
        {
            if (string.IsNullOrEmpty(outputFile) && string.IsNullOrEmpty(outputFolder))
                return filePath;

            if (string.IsNullOrEmpty(outputFile))
                outputFile = Path.GetFileName(filePath);

            if (string.IsNullOrEmpty(outputFolder))
                outputFolder = Path.GetDirectoryName(filePath);

            return Path.GetFullPath(Path.Combine(outputFolder, outputFile));
        }

        private static void CreateOutputFolder(string outputFilePath)
        {
            string folder = Path.GetDirectoryName(outputFilePath);
            if (Directory.Exists(folder))
                return;

            try
            {
                Directory.CreateDirectory(folder);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create folder " + folder, ex);
            }
        }

        private static void Print(List<Subtitle> subtitles)
        {
            foreach (var line in subtitles.ToLines())
                Console.WriteLine(line);
        }
    }
}
