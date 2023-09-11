using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using SubtitlesCleaner.Library;

namespace SubtitlesCleaner.Command
{
    internal class CleanEmptyAndNonSubtitles : SubtitlesAction
    {
        private CleanEmptyAndNonSubtitlesOptions options;

        public override void Init(string filePath, SharedOptions sharedOptions)
        {
            base.Init(filePath, sharedOptions);
            this.options = (CleanEmptyAndNonSubtitlesOptions)sharedOptions;
        }

        public override SubtitlesActionResult Do()
        {
            string fileName = Path.GetFileName(filePath);

            try
            {
                if (options.quiet == false)
                {
                    WriteLog(DateTime.Now, fileName, "Subtitles file {0}", filePath);
                    WriteLog(DateTime.Now, fileName, "Read subtitles start");
                }

                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(filePath, out Encoding encoding, options.firstSubtitlesCount);

                List<Subtitle> originalSubtitles = null;
                if (options.suppressBackupFileOnSame)
                    originalSubtitles = subtitles.Clone();

                if (options.quiet == false)
                {
                    WriteLog(DateTime.Now, fileName, "Read subtitles end");

                    WriteLog(DateTime.Now, fileName, "Print cleaning is {0}", (options.printCleaning ? "enabled" : "disabled"));
                    WriteLog(DateTime.Now, fileName, "Clean empty and non-subtitles start");
                }

                bool thrownException = false;
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    subtitles = subtitles.CleanEmptyAndNonSubtitles(options.printCleaning);
                }
                catch (Exception ex)
                {
                    thrownException = true;
                    if (options.quiet)
                    {
                        lock (Console.Error)
                        {
                            Console.Error.WriteLine(filePath);
                            Console.Error.WriteLine("Clean empty and non-subtitles failed");
                            Console.Error.WriteLine(ex.GetExceptionErrorMessage());
                        }
                    }
                    else
                    {
                        WriteLog(DateTime.Now, fileName, "Clean empty and non-subtitles failed");
                        WriteLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                    }
                }
                finally
                {
                    stopwatch.Stop();
                }

                if (thrownException)
                    return new SubtitlesActionResult() { FilePath = filePath, SharedOptions = sharedOptions, Log = Log };

                if (options.quiet == false)
                {
                    WriteLog(DateTime.Now, fileName, "Clean empty and non-subtitles end");
                    WriteLog(DateTime.Now, fileName, "Clean empty and non-subtitles completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);
                }

                if (options.save)
                    SaveSubtitles(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, options.suppressBackupFileOnSame, true, originalSubtitles);

                if (options.print)
                    PrintSubtitles(subtitles);

                return new SubtitlesActionResult() { FilePath = filePath, SharedOptions = sharedOptions, Log = Log };
            }
            catch (Exception ex)
            {
                if (options.quiet)
                {
                    lock (Console.Error)
                    {
                        Console.Error.WriteLine(filePath);
                        Console.Error.WriteLine("Clean empty and non-subtitles failed");
                        Console.Error.WriteLine(ex.GetExceptionErrorMessage());
                    }
                }
                else
                {
                    WriteLog(DateTime.Now, fileName, "Clean empty and non-subtitles failed");
                    WriteLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                }

                return new SubtitlesActionResult() { FilePath = filePath, SharedOptions = sharedOptions, Log = Log };
            }
        }
    }
}
