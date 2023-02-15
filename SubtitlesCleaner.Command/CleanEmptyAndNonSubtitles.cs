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
            try
            {
                string fileName = Path.GetFileName(filePath);

                if (options.quiet == false)
                {
                    WriteLog(DateTime.Now, fileName, "Subtitles file {0}", filePath);
                    WriteLog(DateTime.Now, fileName, "Read subtitles start");
                }

                Encoding encoding = Encoding.UTF8;
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(
                    filePath,
                    ref encoding,
                    options.firstSubtitlesCount ?? DebugOptions.Instance.FirstSubtitlesCount
                );

                if (options.quiet == false)
                {
                    WriteLog(DateTime.Now, fileName, "Read subtitles end");

                    WriteLog(DateTime.Now, fileName, "Print cleaning is {0}", (options.printCleaning || DebugOptions.Instance.PrintCleaning ? "enabled" : "disabled"));
                    WriteLog(DateTime.Now, fileName, "Clean empty and non-subtitles start");
                }

                bool thrownException = false;
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    subtitles = subtitles.CleanEmptyAndNonSubtitles(options.printCleaning || DebugOptions.Instance.PrintCleaning);
                }
                catch (Exception ex)
                {
                    Error = ex;
                    thrownException = true;
                    if (options.quiet == false)
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
                    return new SubtitlesActionResult() { FilePath = filePath, SharedOptions = sharedOptions, Succeeded = false, Log = Log, Error = Error };

                if (options.quiet == false)
                {
                    WriteLog(DateTime.Now, fileName, "Clean empty and non-subtitles end");
                    WriteLog(DateTime.Now, fileName, "Clean empty and non-subtitles completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);
                }

                if (options.save)
                    SaveSubtitles(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, true);

                if (options.print)
                    PrintSubtitles(subtitles);

                return new SubtitlesActionResult() { FilePath = filePath, SharedOptions = sharedOptions, Succeeded = true, Log = Log, Error = Error };
            }
            catch (Exception ex)
            {
                Error = ex;
                return new SubtitlesActionResult() { FilePath = filePath, SharedOptions = sharedOptions, Succeeded = false, Log = Log, Error = Error };
            }
        }
    }
}
