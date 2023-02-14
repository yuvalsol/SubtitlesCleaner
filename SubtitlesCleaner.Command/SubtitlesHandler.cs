using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SubtitlesCleaner.Command
{
    static class SubtitlesHandler
    {
        public static readonly bool IsProduction = true;
        public static readonly bool PrintCleaning = false;
        public static readonly bool CleanHICaseInsensitive = false;
        public static readonly int? FirstSubtitlesCount = null;

        public static void Debug()
        {
            CleanSubtitles(new CleanSubtitlesOptions()
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

        public static void CleanSubtitles(CleanSubtitlesOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
                new CleanSubtitles(filePath, options).Do();
        }

        public static void CleanEmptyAndNonSubtitles(CleanEmptyAndNonSubtitlesOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
                new CleanEmptyAndNonSubtitles(filePath, options).Do();
        }

        public static void AddTime(AddTimeOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
                new AddTime(filePath, options).Do();
        }

        public static void SetShowTime(SetShowTimeOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
                new SetShowTime(filePath, options).Do();
        }

        public static void AdjustTiming(AdjustTimingOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
                new AdjustTiming(filePath, options).Do();
        }

        public static void Reorder(ReorderOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
                new Reorder(filePath, options).Do();
        }

        public static void BalanceLines(BalanceLinesOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
                new BalanceLines(filePath, options).Do();
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
    }
}
