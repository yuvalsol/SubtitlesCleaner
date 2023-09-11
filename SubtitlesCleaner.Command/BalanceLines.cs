using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using SubtitlesCleaner.Library;

namespace SubtitlesCleaner.Command
{
    internal class BalanceLines : SubtitlesAction
    {
        private BalanceLinesOptions options;

        public override void Init(string filePath, SharedOptions sharedOptions)
        {
            base.Init(filePath, sharedOptions);
            this.options = (BalanceLinesOptions)sharedOptions;
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

                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(filePath, out Encoding encoding);

                List<Subtitle> originalSubtitles = null;
                if (options.suppressBackupFileOnSame)
                    originalSubtitles = subtitles.Clone();

                if (options.quiet == false)
                {
                    WriteLog(DateTime.Now, fileName, "Read subtitles end");

                    WriteLog(DateTime.Now, fileName, "Balance lines start");
                }

                bool thrownException = false;
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    subtitles = subtitles.BalanceLines();
                }
                catch (Exception ex)
                {
                    thrownException = true;
                    if (options.quiet)
                    {
                        lock (Console.Error)
                        {
                            Console.Error.WriteLine(filePath);
                            Console.Error.WriteLine("Balance lines failed");
                            Console.Error.WriteLine(ex.GetExceptionErrorMessage());
                        }
                    }
                    else
                    {
                        WriteLog(DateTime.Now, fileName, "Balance lines failed");
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
                    WriteLog(DateTime.Now, fileName, "Balance lines end");
                    WriteLog(DateTime.Now, fileName, "Balance lines completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);
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
                        Console.Error.WriteLine("Balance lines failed");
                        Console.Error.WriteLine(ex.GetExceptionErrorMessage());
                    }
                }
                else
                {
                    WriteLog(DateTime.Now, fileName, "Balance lines failed");
                    WriteLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                }

                return new SubtitlesActionResult() { FilePath = filePath, SharedOptions = sharedOptions, Log = Log };
            }
        }
    }
}
