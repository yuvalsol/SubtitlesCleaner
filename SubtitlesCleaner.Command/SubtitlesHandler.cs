using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SubtitlesCleaner.Command
{
    internal class SubtitlesHandler
    {
        public static readonly bool IsProduction = true;
        public static readonly bool PrintCleaning = false;
        public static readonly bool CleanHICaseInsensitive = false;
        public static readonly int? FirstSubtitlesCount = null;

        #region Actions

        public void Debug()
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

        public void CleanSubtitles(CleanSubtitlesOptions options)
        {
            PreTasks(options);
            var tasks = GetTasks<CleanSubtitles, CleanSubtitlesOptions>(options).ToArray();
            Task.WaitAll(tasks);
            SaveLog(options);
        }

        public void CleanEmptyAndNonSubtitles(CleanEmptyAndNonSubtitlesOptions options)
        {
            PreTasks(options);
            var tasks = GetTasks<CleanEmptyAndNonSubtitles, CleanEmptyAndNonSubtitlesOptions>(options).ToArray();
            Task.WaitAll(tasks);
            SaveLog(options);
        }

        public void AddTime(AddTimeOptions options)
        {
            PreTasks(options);
            var tasks = GetTasks<AddTime, AddTimeOptions>(options).ToArray();
            Task.WaitAll(tasks);
            SaveLog(options);
        }

        public void SetShowTime(SetShowTimeOptions options)
        {
            PreTasks(options);
            var tasks = GetTasks<SetShowTime, SetShowTimeOptions>(options).ToArray();
            Task.WaitAll(tasks);
            SaveLog(options);
        }

        public void AdjustTiming(AdjustTimingOptions options)
        {
            PreTasks(options);
            var tasks = GetTasks<AdjustTiming, AdjustTimingOptions>(options).ToArray();
            Task.WaitAll(tasks);
            SaveLog(options);
        }

        public void Reorder(ReorderOptions options)
        {
            PreTasks(options);
            var tasks = GetTasks<Reorder, ReorderOptions>(options).ToArray();
            Task.WaitAll(tasks);
            SaveLog(options);
        }

        public void BalanceLines(BalanceLinesOptions options)
        {
            PreTasks(options);
            var tasks = GetTasks<BalanceLines, BalanceLinesOptions>(options).ToArray();
            Task.WaitAll(tasks);
            SaveLog(options);
        }

        #endregion

        #region Path and Files

        private string[] GetFilePaths(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            if (path.EndsWith(":\""))
                path = path.Replace(":\"", ":");

            bool isRecursive = false;
            string[] filePaths = GetFiles(path, isRecursive);

            return filePaths;
        }

        private string[] GetFiles(string path, bool isRecursive)
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

        #endregion

        #region Tasks

        public IEnumerable<Task> GetTasks<TSubtitlesAction, TSharedOptions>(TSharedOptions sharedOptions)
            where TSubtitlesAction : SubtitlesAction, new()
            where TSharedOptions : SharedOptions
        {
            string[] filePaths = GetFilePaths(sharedOptions.path);
            if (filePaths == null || filePaths.Length == 0)
                yield break;

            int fileIndex = 0;
            foreach (var filePath in filePaths)
            {
                yield return Task<SubtitlesActionResult>.Factory.StartNew((object obj) =>
                {
                    var action = new TSubtitlesAction();
                    action.Init(filePath, sharedOptions);
                    SubtitlesActionResult result = action.Do();
                    result.FileIndex = (int)obj;
                    return result;
                }, fileIndex++).ContinueWith(antecedent =>
                {
                    WriteLog(antecedent.Result);
                });
            }
        }

        #endregion

        #region Log

        private object syncObject = new object();
        private StringBuilder log;
        private int fileIndex;

        private void PreTasks(SharedOptions sharedOptions)
        {
            if (sharedOptions.quiet == false)
                log = new StringBuilder();
            fileIndex = 0;
        }

        private void WriteLog(SubtitlesActionResult result)
        {
            if (result.SharedOptions.quiet)
                return;

            while (true)
            {
                lock (syncObject)
                {
                    if (result.FileIndex == fileIndex)
                    {
                        if (string.IsNullOrEmpty(result.SharedOptions.log) && string.IsNullOrEmpty(result.SharedOptions.logAppend))
                        {
                            Console.WriteLine(result.Log);
                        }
                        else
                        {
                            log.Append(result.Log);
                            fileIndex++;
                        }
                        return;
                    }
                }
            }
        }

        private void SaveLog(SharedOptions sharedOptions)
        {
            if (sharedOptions.quiet)
                return;

            if (string.IsNullOrEmpty(sharedOptions.log) == false)
            {
                File.WriteAllText(sharedOptions.log, log.ToString(), Encoding.UTF8);
                log = null;
            }
            else if (string.IsNullOrEmpty(sharedOptions.logAppend) == false)
            {
                File.AppendAllText(sharedOptions.logAppend, log.ToString(), Encoding.UTF8);
                log = null;
            }
        }

        #endregion
    }
}
