using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SubtitlesCleanerLibrary;

namespace SubtitlesCleanerCommand
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
                    Save(subtitles, encoding, filePath, options.outFile, options.outPath);

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
                    Save(subtitles, encoding, filePath, options.outFile, options.outPath);

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
                    Save(subtitles, encoding, filePath, options.outFile, options.outPath);

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
                    Save(subtitles, encoding, filePath, options.outFile, options.outPath);

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
                    Save(subtitles, encoding, filePath, isDisableBackupFile: true);

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
                    Save(subtitles, encoding, filePath);

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
            string outputFileName = null,
            string outputPath = null,
            bool isDisableBackupFile = false)
        {
            string outputFilePath = filePath;

            if (string.IsNullOrEmpty(outputFileName) == false || string.IsNullOrEmpty(outputPath) == false)
            {
                if (string.IsNullOrEmpty(outputFileName))
                    outputFileName = Path.GetFileName(filePath);

                if (string.IsNullOrEmpty(outputPath))
                    outputPath = Path.GetDirectoryName(filePath);

                outputFilePath = Path.GetFullPath(Path.Combine(outputPath, outputFileName));
            }

            if (isDisableBackupFile == false)
            {
                if (outputFilePath == filePath)
                    File.Copy(filePath, filePath.Replace(".srt", ".bak.srt"), true);
            }

            File.WriteAllLines(outputFilePath, subtitles.ToLines(), encoding);

            if (isDisableBackupFile == false)
            {
                string[] errors = GetSubtitlesErrors(subtitles);

                if (errors != null && errors.Length > 0)
                {
                    string errorFile = filePath.Replace(".srt", ".error.srt");
                    File.WriteAllLines(errorFile, errors, encoding);
                }
            }
        }

        private static string[] GetSubtitlesErrors(List<Subtitle> subtitles)
        {
            return subtitles.Select((subtitle, index) =>
                new
                {
                    subtitle,
                    index,
                    HasErrors = subtitle.HasErrors()
                }
            )
            .Where(x => x.HasErrors)
            .SelectMany(x => x.subtitle.ToLines(x.index))
            .ToArray();
        }

        private static void Print(List<Subtitle> subtitles)
        {
            foreach (var line in subtitles.ToLines())
                Console.WriteLine(line);
        }
    }
}
