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

        public SubtitlesAction(string filePath, SharedOptions sharedOptions)
        {
            this.filePath = filePath;
            this.sharedOptions = sharedOptions;

            if (sharedOptions.quiet == false)
                Log = new StringBuilder();
        }

        public virtual StringBuilder Log { get; protected set; }
        public virtual Exception Error { get; protected set; }

        public abstract void Do();

        protected virtual void SaveSubtitles(
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
                    if (sharedOptions.quiet == false)
                        WriteLog(DateTime.Now, fileName, "Save backup subtitles file {0}", backupFile);
                }
                catch (Exception ex)
                {
                    if (sharedOptions.quiet == false)
                    {
                        WriteLog(DateTime.Now, fileName, "Save backup subtitles file failed {0}", backupFile);
                        WriteLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                    }
                }
            }

            try
            {
                string[] lines = subtitles.ToLines();
                File.WriteAllLines(outputFilePath, lines.Take(lines.Length - 1), encoding);
                if (sharedOptions.quiet == false)
                    WriteLog(DateTime.Now, fileName, "Save subtitles file {0}", outputFilePath);
            }
            catch (Exception ex)
            {
                if (sharedOptions.quiet == false)
                {
                    WriteLog(DateTime.Now, fileName, "Save subtitles file failed {0}", outputFilePath);
                    WriteLog(DateTime.Now, fileName, ex.GetExceptionErrorMessage());
                }
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
                        if (sharedOptions.quiet == false)
                            WriteLog(DateTime.Now, fileName, "Save subtitles error file {0}", errorFile);
                    }
                    catch (Exception ex)
                    {
                        if (sharedOptions.quiet == false)
                        {
                            WriteLog(DateTime.Now, fileName, "Save subtitles error file failed {0}", errorFile);
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
                if (sharedOptions.quiet == false)
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
            if (string.IsNullOrEmpty(message))
                return;

            string[] lines = (message ?? string.Empty).Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            Log.AppendFormat("{0:yyy-MM-dd HH:mm:ss.fff}", time);
            Log.Append("\t");
            Log.Append(fileName);
            Log.Append("\t");
            Log.AppendLine(lines[0]);

            for (int i = 1; i < lines.Length; i++)
            {
                Log.Append("                          \t   \t");
                Log.AppendLine(lines[i]);
            }
        }
    }
}
