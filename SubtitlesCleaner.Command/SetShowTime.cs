using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using SubtitlesCleaner.Library;

namespace SubtitlesCleaner.Command
{
    internal class SetShowTime : SubtitlesAction
    {
        private SetShowTimeOptions options;

        public override void Init(string filePath, SharedOptions sharedOptions)
        {
            base.Init(filePath, sharedOptions);
            this.options = (SetShowTimeOptions)sharedOptions;
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

                Encoding encoding = Encoding.UTF8;
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(filePath, ref encoding, options.firstSubtitlesCount);

                if (options.quiet == false)
                {
                    WriteLog(DateTime.Now, fileName, "Read subtitles end");

                    WriteLog(DateTime.Now, fileName, "Show time {0}", options.showTime);
                    if (options.subtitleNumber != null)
                        WriteLog(DateTime.Now, fileName, "Set show time from subtitle number {0}", options.subtitleNumber);

                    WriteLog(DateTime.Now, fileName, "Set show time start");
                }

                bool thrownException = false;
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    subtitles.SetShowTime(options.showTime, options.subtitleNumber);
                }
                catch (Exception ex)
                {
                    thrownException = true;
                    if (options.quiet == false)
                    {
                        WriteLog(DateTime.Now, fileName, "Set show time failed");
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
                    WriteLog(DateTime.Now, fileName, "Set show time end");
                    WriteLog(DateTime.Now, fileName, "Set show time completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);
                }

                if (options.save)
                    SaveSubtitles(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, true);

                if (options.print)
                    PrintSubtitles(subtitles);

                return new SubtitlesActionResult() { FilePath = filePath, SharedOptions = sharedOptions, Log = Log };
            }
            catch (Exception ex)
            {
                if (options.quiet == false)
                {
                    WriteLog(DateTime.Now, fileName, "Set show time failed");
                    WriteLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                }

                return new SubtitlesActionResult() { FilePath = filePath, SharedOptions = sharedOptions, Log = Log };
            }
        }
    }
}
