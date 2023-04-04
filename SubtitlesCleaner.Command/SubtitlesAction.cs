using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SubtitlesCleaner.Library;

namespace SubtitlesCleaner.Command
{
    internal abstract class SubtitlesAction
    {
        protected string filePath;
        protected SharedOptions sharedOptions;

        public virtual void Init(string filePath, SharedOptions sharedOptions)
        {
            this.filePath = filePath;
            this.sharedOptions = sharedOptions;

            if (sharedOptions.quiet == false)
                Log = new StringBuilder();
        }

        public virtual StringBuilder Log { get; protected set; }

        public abstract SubtitlesActionResult Do();

        protected virtual void SaveSubtitles(
            List<Subtitle> subtitles,
            Encoding encoding,
            string filePath,
            string outputFile,
            string outputFolder,
            bool suppressBackupFile,
            bool suppressBackupFileOnSame,
            bool suppressWarningsFile,
            List<Subtitle> originalSubtitles)
        {
            string fileName = Path.GetFileName(filePath);

            string outputFilePath = GetOutputFilePath(filePath, outputFile, outputFolder);

            CreateOutputFolder(outputFilePath, fileName);

            SaveBackupFile(outputFilePath, fileName, filePath, subtitles, suppressBackupFile, suppressBackupFileOnSame, originalSubtitles);

            SaveSubtitlesFile(outputFilePath, fileName, filePath, subtitles, encoding);

            SaveWarningsFile(outputFilePath, fileName, filePath, subtitles, encoding, suppressWarningsFile);
        }

        protected virtual void SaveBackupFile(string outputFilePath, string fileName, string filePath, List<Subtitle> subtitles, bool suppressBackupFile, bool suppressBackupFileOnSame, List<Subtitle> originalSubtitles)
        {
            if (suppressBackupFile == false && suppressBackupFileOnSame)
            {
                suppressBackupFile = (
                    subtitles.Count != originalSubtitles.Count ||
                    subtitles.Zip(originalSubtitles, (s, os) =>
                        s.Lines.Count != os.Lines.Count ||
                        s.Lines.Zip(os.Lines, (l, ol) => l != ol).Any(isChanged => isChanged)
                    ).Any(isChanged => isChanged)) == false;
            }

            if (suppressBackupFile == false)
            {
                string backupFile = outputFilePath.Replace(".srt", ".bak.srt");

                try
                {
                    File.Copy(filePath, backupFile, true);
                    if (sharedOptions.quiet == false)
                        WriteLog(DateTime.Now, fileName, "Save subtitles backup file {0}", backupFile);
                }
                catch (Exception ex)
                {
                    if (sharedOptions.quiet)
                    {
                        lock (Console.Error)
                        {
                            Console.Error.WriteLine(filePath);
                            Console.Error.WriteLine("Save subtitles backup file failed {0}", backupFile);
                            Console.Error.WriteLine(ex.GetExceptionErrorMessage());
                        }
                    }
                    else
                    {
                        WriteLog(DateTime.Now, fileName, "Save subtitles backup file failed {0}", backupFile);
                        WriteLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                    }
                }
            }
        }

        protected virtual void SaveSubtitlesFile(string outputFilePath, string fileName, string filePath, List<Subtitle> subtitles, Encoding encoding)
        {
            try
            {
                string[] lines = subtitles.ToLines();
                File.WriteAllLines(outputFilePath, lines.Take(lines.Length - 1), encoding);
                if (sharedOptions.quiet == false)
                    WriteLog(DateTime.Now, fileName, "Save subtitles file {0}", outputFilePath);
            }
            catch (Exception ex)
            {
                if (sharedOptions.quiet)
                {
                    lock (Console.Error)
                    {
                        Console.Error.WriteLine(filePath);
                        Console.Error.WriteLine("Save subtitles file failed {0}", outputFilePath);
                        Console.Error.WriteLine(ex.GetExceptionErrorMessage());
                    }
                }
                else
                {
                    WriteLog(DateTime.Now, fileName, "Save subtitles file failed {0}", outputFilePath);
                    WriteLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                }
            }
        }

        protected virtual void SaveWarningsFile(string outputFilePath, string fileName, string filePath, List<Subtitle> subtitles, Encoding encoding, bool suppressWarningsFile)
        {
            if (suppressWarningsFile == false)
            {
                string[] warnings = SubtitlesHelper.GetSubtitlesWarnings(subtitles);

                if (warnings != null && warnings.Length > 0)
                {
                    string warningsFile = outputFilePath.Replace(".srt", ".warnings.txt");

                    try
                    {
                        File.WriteAllLines(warningsFile, warnings, encoding);
                        if (sharedOptions.quiet == false)
                            WriteLog(DateTime.Now, fileName, "Save subtitles warnings file {0}", warningsFile);
                    }
                    catch (Exception ex)
                    {
                        if (sharedOptions.quiet)
                        {
                            lock (Console.Error)
                            {
                                Console.Error.WriteLine(filePath);
                                Console.Error.WriteLine("Save subtitles warnings file failed {0}", warningsFile);
                                Console.Error.WriteLine(ex.GetExceptionErrorMessage());
                            }
                        }
                        else
                        {
                            WriteLog(DateTime.Now, fileName, "Save subtitles warnings file failed {0}", warningsFile);
                            WriteLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                        }
                    }
                }
            }
        }

        protected virtual string GetOutputFilePath(string filePath, string outputFile, string outputFolder)
        {
            if (string.IsNullOrEmpty(outputFile) && string.IsNullOrEmpty(outputFolder))
                return filePath;

            if (string.IsNullOrEmpty(outputFile))
                outputFile = Path.GetFileName(filePath);

            if (string.IsNullOrEmpty(outputFolder))
                outputFolder = Path.GetDirectoryName(filePath);

            return Path.GetFullPath(Path.Combine(outputFolder, outputFile));
        }

        protected virtual void CreateOutputFolder(string outputFilePath, string fileName)
        {
            string folder = Path.GetDirectoryName(outputFilePath);
            if (Directory.Exists(folder))
                return;

            try
            {
                Directory.CreateDirectory(folder);
                if (sharedOptions.quiet == false)
                    WriteLog(DateTime.Now, fileName, "Create output folder {0}", folder);
            }
            catch (Exception ex)
            {
                if (sharedOptions.quiet)
                {
                    lock (Console.Error)
                    {
                        Console.Error.WriteLine(filePath);
                        Console.Error.WriteLine("Create output folder failed {0}", folder);
                        Console.Error.WriteLine(ex.GetExceptionErrorMessage());
                    }
                }
                else
                {
                    WriteLog(DateTime.Now, fileName, "Create output folder failed {0}", folder);
                    WriteLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                }
            }
        }

        protected virtual void PrintSubtitles(List<Subtitle> subtitles)
        {
            foreach (var line in subtitles.ToLines())
                Console.WriteLine(line);
        }

        protected virtual void WriteLog(DateTime time, string fileName, string format, params object[] args)
        {
            WriteLog(time, fileName, string.Format(format, args));
        }

        protected virtual void WriteLog(DateTime time, string fileName, string message)
        {
            string[] lines = (message ?? string.Empty).Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            if (sharedOptions.csv)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    Log.AppendFormat("\"{0:yyyy-MM-ddTHH:mm:ss.fffZ}\"", time);
                    Log.Append(",\"");
                    Log.Append(fileName);
                    Log.Append("\",\"");
                    Log.Append(lines[i].Replace("\"", "\"\""));
                    Log.AppendLine("\"");
                }
            }
            else
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    Log.AppendFormat("{0:yyyy-MM-dd HH:mm:ss.fff}", time);
                    Log.Append("\t");
                    Log.Append(fileName);
                    Log.Append("\t");
                    Log.AppendLine(lines[i]);
                }
            }
        }
    }
}
