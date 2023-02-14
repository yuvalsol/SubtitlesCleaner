using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using SubtitlesCleaner.Library;

namespace SubtitlesCleaner.Command
{
    internal class AdjustTiming : SubtitlesAction
    {
        private AdjustTimingOptions options;

        public AdjustTiming(string filePath, AdjustTimingOptions options) : base(filePath, options)
        {
            this.options = options;
        }

        public override void Do()
        {
            try
            {
                string fileName = Path.GetFileName(filePath);

                WriteLog(DateTime.Now, fileName, "Subtitles file {0}", filePath);
                WriteLog(DateTime.Now, fileName, "Read subtitles start");

                Encoding encoding = Encoding.UTF8;
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(
                    filePath,
                    ref encoding,
                    options.firstSubtitlesCount ?? DebugOptions.Instance.FirstSubtitlesCount
                );

                WriteLog(DateTime.Now, fileName, "Read subtitles end");

                WriteLog(DateTime.Now, fileName, "First show time {0}", options.firstShowTime);
                WriteLog(DateTime.Now, fileName, "Last show time {0}", options.lastShowTime);

                WriteLog(DateTime.Now, fileName, "Adjust timing start");

                bool thrownException = false;
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    subtitles.AdjustTiming(options.firstShowTime, options.lastShowTime);
                }
                catch (Exception ex)
                {
                    thrownException = true;
                    WriteLog(DateTime.Now, fileName, "Adjust timing failed");
                    WriteLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                }
                finally
                {
                    stopwatch.Stop();
                }

                if (thrownException)
                    return;

                WriteLog(DateTime.Now, fileName, "Adjust timing end");
                WriteLog(DateTime.Now, fileName, "Adjust timing completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);

                if (options.save)
                    SaveSubtitles(subtitles, encoding, filePath, options.outputFile, options.outputFolder, options.suppressBackupFile, true);

                if (options.print)
                    PrintSubtitles(subtitles);
            }
            catch (Exception ex)
            {
                Error = ex;
            }
        }
    }
}
