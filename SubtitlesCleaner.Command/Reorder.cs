﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using SubtitlesCleaner.Library;

namespace SubtitlesCleaner.Command
{
    internal class Reorder : SubtitlesAction
    {
        private ReorderOptions options;

        public override void Init(string filePath, SharedOptions sharedOptions)
        {
            base.Init(filePath, sharedOptions);
            this.options = (ReorderOptions)sharedOptions;
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

                    WriteLog(DateTime.Now, fileName, "Reorder start");
                }

                bool thrownException = false;
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    subtitles = subtitles.Reorder();
                }
                catch (Exception ex)
                {
                    thrownException = true;
                    if (options.quiet)
                    {
                        lock (Console.Error)
                        {
                            Console.Error.WriteLine(filePath);
                            Console.Error.WriteLine("Reorder failed");
                            Console.Error.WriteLine(ex.GetExceptionErrorMessage());
                        }
                    }
                    else
                    {
                        WriteLog(DateTime.Now, fileName, "Reorder failed");
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
                    WriteLog(DateTime.Now, fileName, "Reorder end");
                    WriteLog(DateTime.Now, fileName, "Reorder completion time {0:mm}:{0:ss}.{0:fff} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);
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
                        Console.Error.WriteLine("Reorder failed");
                        Console.Error.WriteLine(ex.GetExceptionErrorMessage());
                    }
                }
                else
                {
                    WriteLog(DateTime.Now, fileName, "Reorder failed");
                    WriteLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                }

                return new SubtitlesActionResult() { FilePath = filePath, SharedOptions = sharedOptions, Log = Log };
            }
        }
    }
}
