using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static readonly bool PrintCleaning = false;
        private static readonly bool CleanHICaseInsensitive = false;
        private static readonly int? FirstSubtitlesCount = null;

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
            {
                string fileName = Path.GetFileName(filePath);

                PrintLog(DateTime.Now, fileName, "Subtitles file {0}", filePath);
                PrintLog(DateTime.Now, fileName, "Read subtitles start");

                Encoding encoding = Encoding.UTF8;
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(
                    filePath,
                    ref encoding,
                    options.firstSubtitlesCount ?? FirstSubtitlesCount
                );

                PrintLog(DateTime.Now, fileName, "Read subtitles end");

                PrintLog(DateTime.Now, fileName, "Clean hearing-impaired case insensitive is {0}", (options.cleanHICaseInsensitive || CleanHICaseInsensitive ? "enabled" : "disabled"));
                PrintLog(DateTime.Now, fileName, "Print cleaning is {0}", (options.printCleaning || PrintCleaning ? "enabled" : "disabled"));
                PrintLog(DateTime.Now, fileName, "Clean subtitles start");

                bool thrownException = false;
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    subtitles = subtitles.CleanSubtitles(options.cleanHICaseInsensitive || CleanHICaseInsensitive, options.printCleaning || PrintCleaning);
                }
                catch (Exception ex)
                {
                    thrownException = true;
                    PrintLog(DateTime.Now, fileName, "Clean subtitles failed");
                    PrintLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                }
                finally
                {
                    stopwatch.Stop();
                }

                if (thrownException)
                    return;

                PrintLog(DateTime.Now, fileName, "Clean subtitles end");
                PrintLog(DateTime.Now, fileName, "Clean subtitles completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);

                if (options.save)
                    SaveSubtitles(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, options.suppressErrorFile);

                if (options.print)
                    PrintSubtitles(subtitles);
            }
        }

        public static void CleanEmptyAndNonSubtitles(CleanEmptyAndNonSubtitlesOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
            {
                string fileName = Path.GetFileName(filePath);

                PrintLog(DateTime.Now, fileName, "Subtitles file {0}", filePath);
                PrintLog(DateTime.Now, fileName, "Read subtitles start");

                Encoding encoding = Encoding.UTF8;
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(
                    filePath,
                    ref encoding,
                    options.firstSubtitlesCount ?? FirstSubtitlesCount
                );

                PrintLog(DateTime.Now, fileName, "Read subtitles end");

                PrintLog(DateTime.Now, fileName, "Print cleaning is {0}", (options.printCleaning || PrintCleaning ? "enabled" : "disabled"));
                PrintLog(DateTime.Now, fileName, "Clean empty and non-subtitles start");

                bool thrownException = false;
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    subtitles = subtitles.CleanEmptyAndNonSubtitles(options.printCleaning || PrintCleaning);
                }
                catch (Exception ex)
                {
                    thrownException = true;
                    PrintLog(DateTime.Now, fileName, "Clean empty and non-subtitles failed");
                    PrintLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                }
                finally
                {
                    stopwatch.Stop();
                }

                if (thrownException)
                    return;

                PrintLog(DateTime.Now, fileName, "Clean empty and non-subtitles end");
                PrintLog(DateTime.Now, fileName, "Clean empty and non-subtitles completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);

                if (options.save)
                    SaveSubtitles(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, true);

                if (options.print)
                    PrintSubtitles(subtitles);
            }
        }

        public static void AddTime(AddTimeOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
            {
                string fileName = Path.GetFileName(filePath);

                PrintLog(DateTime.Now, fileName, "Subtitles file {0}", filePath);
                PrintLog(DateTime.Now, fileName, "Read subtitles start");

                Encoding encoding = Encoding.UTF8;
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(
                    filePath,
                    ref encoding,
                    options.firstSubtitlesCount ?? FirstSubtitlesCount
                );

                PrintLog(DateTime.Now, fileName, "Read subtitles end");

                PrintLog(DateTime.Now, fileName, "Time added {0}", options.timeAdded);
                if (options.subtitleNumber != null)
                    PrintLog(DateTime.Now, fileName, "Add time from subtitle number {0}", options.subtitleNumber);

                PrintLog(DateTime.Now, fileName, "Add time start");

                bool thrownException = false;
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    subtitles.AddTime(options.timeAdded, options.subtitleNumber);
                }
                catch (Exception ex)
                {
                    thrownException = true;
                    PrintLog(DateTime.Now, fileName, "Add time failed");
                    PrintLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                }
                finally
                {
                    stopwatch.Stop();
                }

                if (thrownException)
                    return;

                PrintLog(DateTime.Now, fileName, "Add time end");
                PrintLog(DateTime.Now, fileName, "Add time completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);

                if (options.save)
                    SaveSubtitles(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, true);

                if (options.print)
                    PrintSubtitles(subtitles);
            }
        }

        public static void SetShowTime(SetShowTimeOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
            {
                string fileName = Path.GetFileName(filePath);

                PrintLog(DateTime.Now, fileName, "Subtitles file {0}", filePath);
                PrintLog(DateTime.Now, fileName, "Read subtitles start");

                Encoding encoding = Encoding.UTF8;
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(
                    filePath,
                    ref encoding,
                    options.firstSubtitlesCount ?? FirstSubtitlesCount
                );

                PrintLog(DateTime.Now, fileName, "Read subtitles end");

                PrintLog(DateTime.Now, fileName, "Show time {0}", options.showTime);
                if (options.subtitleNumber != null)
                    PrintLog(DateTime.Now, fileName, "Set show time from subtitle number {0}", options.subtitleNumber);

                PrintLog(DateTime.Now, fileName, "Set show time start");

                bool thrownException = false;
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    subtitles.SetShowTime(options.showTime, options.subtitleNumber);
                }
                catch (Exception ex)
                {
                    thrownException = true;
                    PrintLog(DateTime.Now, fileName, "Set show time failed");
                    PrintLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                }
                finally
                {
                    stopwatch.Stop();
                }

                if (thrownException)
                    return;

                PrintLog(DateTime.Now, fileName, "Set show time end");
                PrintLog(DateTime.Now, fileName, "Set show time completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);

                if (options.save)
                    SaveSubtitles(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, true);

                if (options.print)
                    PrintSubtitles(subtitles);
            }
        }

        public static void AdjustTiming(AdjustTimingOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
            {
                string fileName = Path.GetFileName(filePath);

                PrintLog(DateTime.Now, fileName, "Subtitles file {0}", filePath);
                PrintLog(DateTime.Now, fileName, "Read subtitles start");

                Encoding encoding = Encoding.UTF8;
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(
                    filePath,
                    ref encoding,
                    options.firstSubtitlesCount ?? FirstSubtitlesCount
                );

                PrintLog(DateTime.Now, fileName, "Read subtitles end");

                PrintLog(DateTime.Now, fileName, "First show time {0}", options.firstShowTime);
                PrintLog(DateTime.Now, fileName, "Last show time {0}", options.lastShowTime);

                PrintLog(DateTime.Now, fileName, "Adjust timing start");

                bool thrownException = false;
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    subtitles.AdjustTiming(options.firstShowTime, options.lastShowTime);
                }
                catch (Exception ex)
                {
                    thrownException = true;
                    PrintLog(DateTime.Now, fileName, "Adjust timing failed");
                    PrintLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                }
                finally
                {
                    stopwatch.Stop();
                }

                if (thrownException)
                    return;

                PrintLog(DateTime.Now, fileName, "Adjust timing end");
                PrintLog(DateTime.Now, fileName, "Adjust timing completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);

                if (options.save)
                    SaveSubtitles(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, true);

                if (options.print)
                    PrintSubtitles(subtitles);
            }
        }

        public static void Reorder(ReorderOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
            {
                string fileName = Path.GetFileName(filePath);

                PrintLog(DateTime.Now, fileName, "Subtitles file {0}", filePath);
                PrintLog(DateTime.Now, fileName, "Read subtitles start");

                Encoding encoding = Encoding.UTF8;
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(filePath, ref encoding);

                PrintLog(DateTime.Now, fileName, "Read subtitles end");

                PrintLog(DateTime.Now, fileName, "Reorder start");

                bool thrownException = false;
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    subtitles = subtitles.Reorder();
                }
                catch (Exception ex)
                {
                    thrownException = true;
                    PrintLog(DateTime.Now, fileName, "Reorder failed");
                    PrintLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                }
                finally
                {
                    stopwatch.Stop();
                }

                if (thrownException)
                    return;

                PrintLog(DateTime.Now, fileName, "Reorder end");
                PrintLog(DateTime.Now, fileName, "Reorder completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);

                if (options.save)
                    SaveSubtitles(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, true);

                if (options.print)
                    PrintSubtitles(subtitles);
            }
        }

        public static void BalanceLines(BalanceLinesOptions options)
        {
            string[] filePaths = GetFilePaths(options.path);
            if (filePaths == null || filePaths.Length == 0)
                return;

            foreach (var filePath in filePaths)
            {
                string fileName = Path.GetFileName(filePath);

                PrintLog(DateTime.Now, fileName, "Subtitles file {0}", filePath);
                PrintLog(DateTime.Now, fileName, "Read subtitles start");

                Encoding encoding = Encoding.UTF8;
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(filePath, ref encoding);

                PrintLog(DateTime.Now, fileName, "Read subtitles end");

                PrintLog(DateTime.Now, fileName, "Balance lines start");

                bool thrownException = false;
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    subtitles = subtitles.BalanceLines();
                }
                catch (Exception ex)
                {
                    thrownException = true;
                    PrintLog(DateTime.Now, fileName, "Balance lines failed");
                    PrintLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                }
                finally
                {
                    stopwatch.Stop();
                }

                if (thrownException)
                    return;

                PrintLog(DateTime.Now, fileName, "Balance lines end");
                PrintLog(DateTime.Now, fileName, "Balance lines completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);

                if (options.save)
                    SaveSubtitles(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, true);

                if (options.print)
                    PrintSubtitles(subtitles);
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

        private static void SaveSubtitles(
            List<Subtitle> subtitles,
            Encoding encoding,
            string filePath,
            string outputFile,
            string outputFolder,
            bool suppressBackupFile,
            bool suppressErrorFile)
        {
            string fileName = Path.GetFileName(filePath);

            string outputFilePath = GetOutputFilePath(filePath, outputFile, outputFolder);

            CreateOutputFolder(outputFilePath, fileName);

            if (suppressBackupFile == false)
            {
                string backupFile = outputFilePath.Replace(".srt", ".bak.srt");

                try
                {
                    File.Copy(filePath, backupFile, true);
                    PrintLog(DateTime.Now, fileName, "Save backup subtitles file {0}", backupFile);
                }
                catch (Exception ex)
                {
                    PrintLog(DateTime.Now, fileName, "Save backup subtitles file failed {0}", backupFile);
                    PrintLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                }
            }

            try
            {
                string[] lines = subtitles.ToLines();
                File.WriteAllLines(outputFilePath, lines.Take(lines.Length - 1), encoding);
                PrintLog(DateTime.Now, fileName, "Save subtitles file {0}", outputFilePath);
            }
            catch (Exception ex)
            {
                PrintLog(DateTime.Now, fileName, "Save subtitles file failed {0}", outputFilePath);
                PrintLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
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
                        PrintLog(DateTime.Now, fileName, "Save subtitles error file {0}", errorFile);
                    }
                    catch (Exception ex)
                    {
                        PrintLog(DateTime.Now, fileName, "Save subtitles error file failed {0}", errorFile);
                        PrintLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
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

        private static void CreateOutputFolder(string outputFilePath, string fileName)
        {
            string folder = Path.GetDirectoryName(outputFilePath);
            if (Directory.Exists(folder))
                return;

            try
            {
                Directory.CreateDirectory(folder);
                PrintLog(DateTime.Now, fileName, "Create output folder {0}", folder);
            }
            catch (Exception ex)
            {
                PrintLog(DateTime.Now, fileName, "Create output folder failed {0}", folder);
                PrintLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
            }
        }

        private static void PrintSubtitles(List<Subtitle> subtitles)
        {
            foreach (var line in subtitles.ToLines())
                Console.WriteLine(line);
        }

        private static void PrintLog(DateTime time, string fileName, string format, params object[] args)
        {
            PrintLog(time, fileName, string.Format(format, args));
        }

        private static void PrintLog(DateTime time, string fileName, string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            string[] lines = (message ?? string.Empty).Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            Console.WriteLine("{0:yyy-MM-dd HH:mm:ss.fff}\t{1}\t{2}", time, fileName, lines[0]);
            for (int i = 1; i < lines.Length; i++)
                Console.WriteLine("                          \t   \t{0}", lines[i]);
        }
    }
}
