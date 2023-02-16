using System;
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
                print = true,
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
                    DoSequentially<TSubtitlesAction, TSharedOptions>(options).ToArray();
                }
                else
                {
                    PreDo(options);
                    var stopwatch = Stopwatch.StartNew();

                    int fileCount = 0;
                    foreach (var result in DoSequentially<TSubtitlesAction, TSharedOptions>(options))
                    {
                        WriteLogSequentially(result);
                        fileCount++;
                    }

                    stopwatch.Stop();
                    PostDo(options, fileCount, stopwatch);
                }
            }
            else
            {
                if (options.quiet)
                {
                    Task.WaitAll(DoQuietConcurrently<TSubtitlesAction, TSharedOptions>(options).ToArray());
                }
                else
                {
                    PreDo(options);
                    var stopwatch = Stopwatch.StartNew();

                    var tasks = DoConcurrently<TSubtitlesAction, TSharedOptions>(options).ToArray();
                    Task.WaitAll(tasks);

                    stopwatch.Stop();
                    int fileCount = tasks.Length;
                    PostDo(options, fileCount, stopwatch);
                }
            }
        }

        private void PreDo<TSharedOptions>(TSharedOptions options)
            where TSharedOptions : SharedOptions
        {
            log = new StringBuilder();
            fileIndex = 0;
            WriteLog(DateTime.Now, options, "SubtitlesCleanerCommand", "Version {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
            WriteLog(DateTime.Now, options, "SubtitlesCleanerCommand", options.ToString());
        }

        private void PostDo<TSharedOptions>(TSharedOptions options, int fileCount, Stopwatch stopwatch)
            where TSharedOptions : SharedOptions
        {
            WriteLog(DateTime.Now, options, "SubtitlesCleanerCommand", "Processed {0} file{1}", fileCount, (fileCount == 1 ? string.Empty : "s"));
            WriteLog(DateTime.Now, options, "SubtitlesCleanerCommand", "Completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);
            SaveLog(options);
        }

        private IEnumerable<Task> DoConcurrently<TSubtitlesAction, TSharedOptions>(TSharedOptions sharedOptions)
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

        private IEnumerable<Task<SubtitlesActionResult>> DoQuietConcurrently<TSubtitlesAction, TSharedOptions>(TSharedOptions sharedOptions)
            where TSubtitlesAction : SubtitlesAction, new()
            where TSharedOptions : SharedOptions
        {
            string[] filePaths = GetFilePaths(sharedOptions.path);
            if (filePaths == null || filePaths.Length == 0)
                yield break;

            foreach (var filePath in filePaths)
            {
                var action = new TSubtitlesAction();
                action.Init(filePath, sharedOptions);
                yield return Task<SubtitlesActionResult>.Factory.StartNew(action.Do);
            }
        }

        private IEnumerable<SubtitlesActionResult> DoSequentially<TSubtitlesAction, TSharedOptions>(TSharedOptions sharedOptions)
            where TSubtitlesAction : SubtitlesAction, new()
            where TSharedOptions : SharedOptions
        {
            string[] filePaths = GetFilePaths(sharedOptions.path);
            if (filePaths == null || filePaths.Length == 0)
                yield break;

            foreach (var filePath in filePaths)
            {
                var action = new TSubtitlesAction();
                action.Init(filePath, sharedOptions);
                yield return action.Do();
            }
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
            return GetFiles(path, isRecursive);
        }

        private string[] GetFiles(string path, bool isRecursive)
        {
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

        private object syncObject = new object();
        private StringBuilder log;
        private int fileIndex;

        protected virtual void WriteLog(DateTime time, SharedOptions sharedOptions, string name, string format, params object[] args)
        {
            WriteLog(time, sharedOptions, name, string.Format(format, args));
        }

        private void WriteLog(DateTime time, SharedOptions sharedOptions, string name, string message)
        {
            if (string.IsNullOrEmpty(sharedOptions.log) && string.IsNullOrEmpty(sharedOptions.logAppend))
            {
                StringBuilder log = new StringBuilder();
                WriteLog(log, time, sharedOptions, name, message);
                Console.WriteLine(log.ToString().Trim());
                log = null;
            }
            else
            {
                WriteLog(log, time, sharedOptions, name, message);
            }
        }

        private void WriteLog(StringBuilder log, DateTime time, SharedOptions sharedOptions, string name, string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            string[] lines = (message ?? string.Empty).Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            if (sharedOptions.csv)
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
                        try
                        {
                            if (string.IsNullOrEmpty(result.SharedOptions.log) && string.IsNullOrEmpty(result.SharedOptions.logAppend))
                                Console.WriteLine(result.Log.ToString().Trim());
                            else
                                log.Append(result.Log);
                        }
                        catch { }
                        finally
                        {
                            fileIndex++;
                        }

                        return;
                    }
                }
            }
        }

        private void WriteLogSequentially(SubtitlesActionResult result)
        {
            if (result.SharedOptions.quiet)
                return;

            if (string.IsNullOrEmpty(result.SharedOptions.log) && string.IsNullOrEmpty(result.SharedOptions.logAppend))
                Console.WriteLine(result.Log.ToString().Trim());
            else
                log.Append(result.Log);
        }

        private void SaveLog(SharedOptions sharedOptions)
        {
            if (sharedOptions.quiet)
                return;

            if (string.IsNullOrEmpty(sharedOptions.log) == false)
            {
                string folder = Path.GetDirectoryName(sharedOptions.log);
                if (Directory.Exists(folder) == false)
                    Directory.CreateDirectory(folder);

                File.WriteAllText(sharedOptions.log, log.ToString(), Encoding.UTF8);
                log = null;
            }
            else if (string.IsNullOrEmpty(sharedOptions.logAppend) == false)
            {
                string folder = Path.GetDirectoryName(sharedOptions.logAppend);
                if (Directory.Exists(folder) == false)
                    Directory.CreateDirectory(folder);

                File.AppendAllText(sharedOptions.logAppend, log.ToString(), Encoding.UTF8);
                log = null;
            }
        }

        #endregion
    }
}
