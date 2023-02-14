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

        public BalanceLines(string filePath, BalanceLinesOptions options) : base(filePath, options)
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
                List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(filePath, ref encoding);

                WriteLog(DateTime.Now, fileName, "Read subtitles end");

                WriteLog(DateTime.Now, fileName, "Balance lines start");

                bool thrownException = false;
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    subtitles = subtitles.BalanceLines();
                }
                catch (Exception ex)
                {
                    thrownException = true;
                    WriteLog(DateTime.Now, fileName, "Balance lines failed");
                    WriteLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                }
                finally
                {
                    stopwatch.Stop();
                }

                if (thrownException)
                    return;

                WriteLog(DateTime.Now, fileName, "Balance lines end");
                WriteLog(DateTime.Now, fileName, "Balance lines completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);

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
