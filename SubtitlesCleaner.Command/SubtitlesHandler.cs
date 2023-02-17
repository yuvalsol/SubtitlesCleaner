using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static readonly bool PrintCleaning = false;
        private static readonly bool CleanHICaseInsensitive = false;
        private static readonly int? FirstSubtitlesCount = null;

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
                print = true,
                printCleaning = PrintCleaning,
                cleanHICaseInsensitive = CleanHICaseInsensitive,
                firstSubtitlesCount = FirstSubtitlesCount,
                quiet = true,
                sequential = true
            });
        }

        public void CleanSubtitles(CleanSubtitlesOptions options)
        {
            Do<CleanSubtitles, CleanSubtitlesOptions>(options);
        }

        public void CleanEmptyAndNonSubtitles(CleanEmptyAndNonSubtitlesOptions options)
        {
            Do<CleanEmptyAndNonSubtitles, CleanEmptyAndNonSubtitlesOptions>(options);
        }

        public void AddTime(AddTimeOptions options)
        {
            Do<AddTime, AddTimeOptions>(options);
        }

        public void SetShowTime(SetShowTimeOptions options)
        {
            Do<SetShowTime, SetShowTimeOptions>(options);
        }

        public void AdjustTiming(AdjustTimingOptions options)
        {
            Do<AdjustTiming, AdjustTimingOptions>(options);
        }

        public void Reorder(ReorderOptions options)
        {
            Do<Reorder, ReorderOptions>(options);
        }

        public void BalanceLines(BalanceLinesOptions options)
        {
            Do<BalanceLines, BalanceLinesOptions>(options);
        }

        #endregion

        #region Do

        private void Do<TSubtitlesAction, TSharedOptions>(TSharedOptions options)
            where TSubtitlesAction : SubtitlesAction, new()
            where TSharedOptions : SharedOptions
        {
            if (options.sequential)
            {
                if (options.quiet)
                {
                    string[] filePaths = GetFilePaths(options.path, out bool isPathExists);
                    if (filePaths != null && filePaths.Length > 0)
                        DoSequentially<TSubtitlesAction, TSharedOptions>(filePaths, options).ToArray();
                }
                else
                {
                    string logFilePath = GetLogFilePath(options);
                    if (string.IsNullOrEmpty(logFilePath))
                    {
                        WriteLogToConsole(DateTime.Now, options, "SubtitlesCleanerCommand", "Version {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
                        WriteLogToConsole(DateTime.Now, options, "SubtitlesCleanerCommand", options.ToString());

                        var stopwatch = Stopwatch.StartNew();

                        int fileCount = 0;
                        string[] filePaths = GetFilePaths(options.path, out bool isPathExists);
                        if (filePaths != null && filePaths.Length > 0)
                        {
                            foreach (var result in DoSequentially<TSubtitlesAction, TSharedOptions>(filePaths, options))
                            {
                                OutputLogToConsole(result.Log);
                                fileCount++;
                            }
                        }
                        else if (isPathExists)
                        {
                            WriteLogToConsole(DateTime.Now, options, "SubtitlesCleanerCommand", "No subtitle files were found at path {0}", options.path);
                        }
                        else
                        {
                            WriteLogToConsole(DateTime.Now, options, "SubtitlesCleanerCommand", "Path doesn't exist {0}", options.path);
                        }

                        stopwatch.Stop();

                        WriteLogToConsole(DateTime.Now, options, "SubtitlesCleanerCommand", "Processed {0} file{1}", fileCount, (fileCount == 1 ? string.Empty : "s"));
                        WriteLogToConsole(DateTime.Now, options, "SubtitlesCleanerCommand", "Completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);
                        if (fileCount > 0)
                        {
                            var averageTime = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds / fileCount);
                            WriteLogToConsole(DateTime.Now, options, "SubtitlesCleanerCommand", "Average time {0:mm}:{0:ss}.{0:fff} ({1} ms)", averageTime, averageTime.TotalMilliseconds);
                        }
                    }
                    else
                    {
                        CreateLogFile(logFilePath);

                        WriteLogToFile(logFilePath, DateTime.Now, options, "SubtitlesCleanerCommand", "Version {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
                        WriteLogToFile(logFilePath, DateTime.Now, options, "SubtitlesCleanerCommand", options.ToString());

                        var stopwatch = Stopwatch.StartNew();

                        int fileCount = 0;
                        string[] filePaths = GetFilePaths(options.path, out bool isPathExists);
                        if (filePaths != null && filePaths.Length > 0)
                        {
                            foreach (var result in DoSequentially<TSubtitlesAction, TSharedOptions>(filePaths, options))
                            {
                                OutputLogToFile(logFilePath, result.Log);
                                fileCount++;
                            }
                        }
                        else if (isPathExists)
                        {
                            WriteLogToFile(logFilePath, DateTime.Now, options, "SubtitlesCleanerCommand", "No subtitle files were found at path {0}", options.path);
                        }
                        else
                        {
                            WriteLogToFile(logFilePath, DateTime.Now, options, "SubtitlesCleanerCommand", "Path doesn't exist {0}", options.path);
                        }

                        stopwatch.Stop();

                        WriteLogToFile(logFilePath, DateTime.Now, options, "SubtitlesCleanerCommand", "Processed {0} file{1}", fileCount, (fileCount == 1 ? string.Empty : "s"));
                        WriteLogToFile(logFilePath, DateTime.Now, options, "SubtitlesCleanerCommand", "Completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);
                        if (fileCount > 0)
                        {
                            var averageTime = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds / fileCount);
                            WriteLogToFile(logFilePath, DateTime.Now, options, "SubtitlesCleanerCommand", "Average time {0:mm}:{0:ss}.{0:fff} ({1} ms)", averageTime, averageTime.TotalMilliseconds);
                        }
                    }
                }
            }
            else
            {
                if (options.quiet)
                {
                    string[] filePaths = GetFilePaths(options.path, out bool isPathExists);
                    if (filePaths != null && filePaths.Length > 0)
                        Task.WaitAll(DoQuietConcurrently<TSubtitlesAction, TSharedOptions>(filePaths, options).ToArray());
                }
                else
                {
                    string logFilePath = GetLogFilePath(options);
                    if (string.IsNullOrEmpty(logFilePath))
                    {
                        WriteLogToConsole(DateTime.Now, options, "SubtitlesCleanerCommand", "Version {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
                        WriteLogToConsole(DateTime.Now, options, "SubtitlesCleanerCommand", options.ToString());

                        var stopwatch = Stopwatch.StartNew();

                        int fileCount = 0;
                        string[] filePaths = GetFilePaths(options.path, out bool isPathExists);
                        if (filePaths != null && filePaths.Length > 0)
                        {
                            queue = new ConcurrentQueue<SubtitlesActionResult>();

                            using (blockingQueue = new BlockingCollection<SubtitlesActionResult>(queue, filePaths.Length))
                            {
                                var queueTask = Task.Run(ConsumeLogQueueToConsole);

                                var tasks = DoConcurrently<TSubtitlesAction, TSharedOptions>(filePaths, options).ToArray();
                                Task.WaitAll(tasks);

                                blockingQueue.CompleteAdding();
                                queueTask.Wait();

                                fileCount = tasks.Length;
                            }

                            blockingQueue = null;
                            queue = null;
                        }
                        else if (isPathExists)
                        {
                            WriteLogToConsole(DateTime.Now, options, "SubtitlesCleanerCommand", "No subtitle files were found at path {0}", options.path);
                        }
                        else
                        {
                            WriteLogToConsole(DateTime.Now, options, "SubtitlesCleanerCommand", "Path doesn't exist {0}", options.path);
                        }

                        stopwatch.Stop();

                        WriteLogToConsole(DateTime.Now, options, "SubtitlesCleanerCommand", "Processed {0} file{1}", fileCount, (fileCount == 1 ? string.Empty : "s"));
                        WriteLogToConsole(DateTime.Now, options, "SubtitlesCleanerCommand", "Completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);
                        if (fileCount > 0)
                        {
                            var averageTime = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds / fileCount);
                            WriteLogToConsole(DateTime.Now, options, "SubtitlesCleanerCommand", "Average time {0:mm}:{0:ss}.{0:fff} ({1} ms)", averageTime, averageTime.TotalMilliseconds);
                        }
                    }
                    else
                    {
                        CreateLogFile(logFilePath);

                        WriteLogToFile(logFilePath, DateTime.Now, options, "SubtitlesCleanerCommand", "Version {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
                        WriteLogToFile(logFilePath, DateTime.Now, options, "SubtitlesCleanerCommand", options.ToString());

                        var stopwatch = Stopwatch.StartNew();

                        int fileCount = 0;
                        string[] filePaths = GetFilePaths(options.path, out bool isPathExists);
                        if (filePaths != null && filePaths.Length > 0)
                        {
                            queue = new ConcurrentQueue<SubtitlesActionResult>();

                            using (blockingQueue = new BlockingCollection<SubtitlesActionResult>(queue, filePaths.Length))
                            {
                                var queueTask = Task.Run(() => ConsumeLogQueueToFile(logFilePath));

                                var tasks = DoConcurrently<TSubtitlesAction, TSharedOptions>(filePaths, options).ToArray();
                                Task.WaitAll(tasks);

                                blockingQueue.CompleteAdding();
                                queueTask.Wait();

                                fileCount = tasks.Length;
                            }

                            blockingQueue = null;
                            queue = null;
                        }
                        else if (isPathExists)
                        {
                            WriteLogToFile(logFilePath, DateTime.Now, options, "SubtitlesCleanerCommand", "No subtitle files were found at path {0}", options.path);
                        }
                        else
                        {
                            WriteLogToFile(logFilePath, DateTime.Now, options, "SubtitlesCleanerCommand", "Path doesn't exist {0}", options.path);
                        }

                        stopwatch.Stop();

                        WriteLogToFile(logFilePath, DateTime.Now, options, "SubtitlesCleanerCommand", "Processed {0} file{1}", fileCount, (fileCount == 1 ? string.Empty : "s"));
                        WriteLogToFile(logFilePath, DateTime.Now, options, "SubtitlesCleanerCommand", "Completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);
                        if (fileCount > 0)
                        {
                            var averageTime = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds / fileCount);
                            WriteLogToFile(logFilePath, DateTime.Now, options, "SubtitlesCleanerCommand", "Average time {0:mm}:{0:ss}.{0:fff} ({1} ms)", averageTime, averageTime.TotalMilliseconds);
                        }
                    }
                }
            }
        }

        private IEnumerable<Task> DoConcurrently<TSubtitlesAction, TSharedOptions>(string[] filePaths, TSharedOptions options)
            where TSubtitlesAction : SubtitlesAction, new()
            where TSharedOptions : SharedOptions
        {
            foreach (var filePath in filePaths)
            {
                var action = new TSubtitlesAction();
                action.Init(filePath, options);
                yield return Task.Run(() => { blockingQueue.Add(action.Do()); });
            }
        }

        private IEnumerable<Task<SubtitlesActionResult>> DoQuietConcurrently<TSubtitlesAction, TSharedOptions>(string[] filePaths, TSharedOptions options)
            where TSubtitlesAction : SubtitlesAction, new()
            where TSharedOptions : SharedOptions
        {
            foreach (var filePath in filePaths)
            {
                var action = new TSubtitlesAction();
                action.Init(filePath, options);
                yield return Task.Run(action.Do);
            }
        }

        private IEnumerable<SubtitlesActionResult> DoSequentially<TSubtitlesAction, TSharedOptions>(string[] filePaths, TSharedOptions options)
            where TSubtitlesAction : SubtitlesAction, new()
            where TSharedOptions : SharedOptions
        {
            foreach (var filePath in filePaths)
            {
                var action = new TSubtitlesAction();
                action.Init(filePath, options);
                yield return action.Do();
            }
        }

        #endregion

        #region Files

        private string[] GetFilePaths(string path, out bool isPathExists)
        {
            if (string.IsNullOrEmpty(path))
            {
                isPathExists = false;
                return null;
            }

            if (path.EndsWith(":\""))
                path = path.Replace(":\"", ":");

            bool isRecursive = false;
            return GetFiles(path, isRecursive, out isPathExists);
        }

        private string[] GetFiles(string path, bool isRecursive, out bool isPathExists)
        {
            isPathExists = File.Exists(path) || Directory.Exists(path);

            bool isSRTFile = string.Compare(Path.GetExtension(path), ".srt", true) == 0;
            if (isSRTFile)
            {
                if (File.Exists(path))
                    return new string[] { path };
            }
            else if (Directory.Exists(path))
            {
                List<string> files = new List<string>(Directory.GetFiles(path, "*.srt", (isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)));
                var bakFiles = files.Where(x => x.EndsWith(".bak.srt")).ToArray();
                foreach (var bakFile in bakFiles)
                {
                    files.Remove(bakFile);
                    files.Remove(bakFile.Replace(".bak.srt", ".srt"));
                }
                return files.ToArray();
            }

            return null;
        }

        #endregion

        #region Log

        private static readonly Encoding encoding = Encoding.UTF8;

        private string GetLogFilePath(SharedOptions options)
        {
            string logFilePath = null;
            if (string.IsNullOrEmpty(options.log) == false)
                logFilePath = options.log;
            else if (string.IsNullOrEmpty(options.logAppend) == false)
                logFilePath = options.logAppend;

            if (string.IsNullOrEmpty(logFilePath) == false)
            {
                if (Directory.Exists(logFilePath) || logFilePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    logFilePath = Path.Combine(logFilePath,
                        string.Format("log_{0}.{1}", options.Verb, options.csv ? "csv" : "txt"));
                }
                else if (string.IsNullOrEmpty(Path.GetExtension(logFilePath)))
                {
                    logFilePath += options.csv ? ".csv" : ".txt";
                }
            }

            return logFilePath;
        }

        private void CreateLogFile(string logFilePath)
        {
            string folder = Path.GetDirectoryName(logFilePath);
            if (Directory.Exists(folder) == false)
                Directory.CreateDirectory(folder);

            File.WriteAllText(logFilePath, string.Empty, encoding);
        }

        private ConcurrentQueue<SubtitlesActionResult> queue;
        private BlockingCollection<SubtitlesActionResult> blockingQueue;

        private void ConsumeLogQueueToFile(string logFilePath)
        {
            foreach (var result in blockingQueue.GetConsumingEnumerable())
                OutputLogToFile(logFilePath, result.Log);
        }

        private void ConsumeLogQueueToConsole()
        {
            foreach (var result in blockingQueue.GetConsumingEnumerable())
                OutputLogToConsole(result.Log);
        }

        private void WriteLogToFile(string path, DateTime time, SharedOptions options, string name, string format, params object[] args)
        {
            OutputLogToFile(path, GetLog(time, options, name, string.Format(format, args)));
        }

        private void WriteLogToConsole(DateTime time, SharedOptions options, string name, string format, params object[] args)
        {
            OutputLogToConsole(GetLog(time, options, name, string.Format(format, args)));
        }

        private void WriteLogToFile(string path, DateTime time, SharedOptions options, string name, string message)
        {
            OutputLogToFile(path, GetLog(time, options, name, message));
        }

        private void WriteLogToConsole(DateTime time, SharedOptions options, string name, string message)
        {
            OutputLogToConsole(GetLog(time, options, name, message));
        }

        private void OutputLogToFile(string logFilePath, StringBuilder log)
        {
            File.AppendAllText(logFilePath, log.ToString(), encoding);
        }

        private void OutputLogToConsole(StringBuilder log)
        {
            Console.WriteLine(log.ToString().Trim());
        }

        private StringBuilder GetLog(DateTime time, SharedOptions options, string name, string message)
        {
            string[] lines = (message ?? string.Empty).Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            StringBuilder log = new StringBuilder();

            if (options.csv)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    log.AppendFormat("\"{0:yyyy-MM-ddTHH:mm:ss.fffZ}\"", time);
                    log.Append(",\"");
                    log.Append(name);
                    log.Append("\",\"");
                    log.Append(lines[i].Replace("\"", "\"\""));
                    log.AppendLine("\"");
                }
            }
            else
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    log.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff}", time);
                    log.Append("\t");
                    log.Append(name);
                    log.Append("\t");
                    log.AppendLine(lines[i]);
                }
            }

            return log;
        }

        #endregion
    }
}
