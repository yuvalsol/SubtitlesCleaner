using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
            var tasks = GetTasks<CleanSubtitles, CleanSubtitlesOptions>(options).ToArray();
            Task.WaitAll(tasks);
        }

        public static IEnumerable<Task<SubtitlesActionResult>> GetTasks<TSubtitlesAction, TSharedOptions>(TSharedOptions sharedOptions)
            where TSubtitlesAction : SubtitlesAction, new()
            where TSharedOptions : SharedOptions
        {
            string[] filePaths = GetFilePaths(sharedOptions.path);
            if (filePaths == null || filePaths.Length == 0)
                yield break;

            int fileIndex = 0;
            foreach (var filePath in filePaths)
            {
                yield return Task.Factory.StartNew((object obj) =>
                {
                    var action = new TSubtitlesAction();
                    action.Init(filePath, sharedOptions);
                    var result = action.Do();
                    result.FileIndex = (int)obj;
                    return result;
                }, fileIndex++);
            }
        }

        public static void CleanEmptyAndNonSubtitles(CleanEmptyAndNonSubtitlesOptions options)
        {
            var tasks = GetTasks<CleanEmptyAndNonSubtitles, CleanEmptyAndNonSubtitlesOptions>(options).ToArray();
            Task.WaitAll(tasks);
        }

        public static void AddTime(AddTimeOptions options)
        {
            var tasks = GetTasks<AddTime, AddTimeOptions>(options).ToArray();
            Task.WaitAll(tasks);
        }

        public static void SetShowTime(SetShowTimeOptions options)
        {
            var tasks = GetTasks<SetShowTime, SetShowTimeOptions>(options).ToArray();
            Task.WaitAll(tasks);
        }

        public static void AdjustTiming(AdjustTimingOptions options)
        {
            var tasks = GetTasks<AdjustTiming, AdjustTimingOptions>(options).ToArray();
            Task.WaitAll(tasks);
        }

        public static void Reorder(ReorderOptions options)
        {
            var tasks = GetTasks<Reorder, ReorderOptions>(options).ToArray();
            Task.WaitAll(tasks);
        }

        public static void BalanceLines(BalanceLinesOptions options)
        {
            var tasks = GetTasks<BalanceLines, BalanceLinesOptions>(options).ToArray();
            Task.WaitAll(tasks);
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
