using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine;
using SubtitlesCL;

namespace SubtitlesApp
{
    class SubtitlesHandler
    {
        private static readonly bool IsProduction = true;
        private static readonly bool CleanHICaseInsensitive = false;
        private static readonly bool IsPrint = false;
        private static readonly int? FirstSubtitlesCount = null;

        public void Run(string[] args)
        {
            if (IsProduction)
                Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(HandleSubtitles);
            else
                Debug();
        }

        private void Debug()
        {
            string filePath = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "..", "..", "..", "Subtitles",
                "Test.srt"
            ));

            bool isPrint = true;

            if (isPrint)
            {
                HandleSubtitles(new Options()
                {
                    path = filePath,
                    print = true,
                    clean = true
                });
            }
            else
            {
                string outputFileName = null;
                string outputPath = null;

                //outputFileName = "template.srt";
                //outputPath = @"E:\My Downloads";
                //outputPath = @"D:\yuvals\files";

                HandleSubtitles(new Options()
                {
                    path = filePath,
                    save = true,
                    clean = true,
                    outFile = outputFileName,
                    outPath = outputPath
                });

                // shift
                //HandleSubtitles(new Options()
                //{
                //    path = filePath,
                //    save = true,
                //    shift = true,
                //    shiftTime = "+00:00:00,000",
                //    subtitleNumber = 1
                //});

                // showTime
                //HandleSubtitles(new Options()
                //{
                //    path = filePath,
                //    save = true,
                //    move = true,
                //    showTime = "00:00:00,000",
                //    subtitleNumber = 1
                //});

                // adjust
                //HandleSubtitles(new Options()
                //{
                //    path = filePath,
                //    save = true,
                //    adjust = true,
                //    showStart = "00:00:00,000",
                //    showEnd = "00:00:00,000"
                //});
            }
        }

        private void HandleSubtitles(Options options)
        {
            if ((options.print || options.save) == false)
                return;

            if (string.IsNullOrEmpty(options.path))
                return;

            if (options.path.EndsWith(":\""))
                options.path = options.path.Replace(":\"", ":");

            bool isRecursive = false;
            string[] filePaths = GetFiles(options.path, isRecursive);

            if (filePaths == null || filePaths.Length == 0)
                return;

            if (options.subtitlesOrder)
            {
                foreach (var filePath in filePaths)
                    SetSubtitlesOrder(options, filePath);
            }
            else
            {
                foreach (var filePath in filePaths)
                    CleanSubtitles(options, filePath);
            }
        }

        private void CleanSubtitles(Options options, string filePath)
        {
            List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(filePath, options.firstSubtitlesCount ?? FirstSubtitlesCount);

            if (options.clean)
                subtitles = subtitles.CleanSubtitles(options.cleanHICaseInsensitive || CleanHICaseInsensitive, IsPrint);

            if (options.shift)
                subtitles.Shift(options.shiftTime, options.subtitleNumber);

            if (options.move)
                subtitles.MoveTo(options.showTime, options.subtitleNumber);

            if (options.adjust)
                subtitles.Adjust(options.showStart, options.showEnd);

            if (options.save)
                Save(subtitles, filePath, options.outFile, options.outPath);

            if (options.print)
                Print(subtitles);
        }

        private void SetSubtitlesOrder(Options options, string filePath)
        {
            List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(filePath);

            subtitles = subtitles.SetSubtitlesOrder();

            if (options.save)
                Save(subtitles, filePath, isDisableBackupFile: true);

            if (options.print)
                Print(subtitles);
        }

        private string[] GetFiles(string path, bool isRecursive)
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

        private void Save(
            List<Subtitle> subtitles,
            string filePath,
            string outputFileName = null,
            string outputPath = null,
            bool isDisableBackupFile = false)
        {
            string outputFilePath = filePath;

            if (string.IsNullOrEmpty(outputFileName) == false || string.IsNullOrEmpty(outputPath) == false)
            {
                if (string.IsNullOrEmpty(outputFileName))
                    outputFileName = Path.GetFileName(filePath);

                if (string.IsNullOrEmpty(outputPath))
                    outputPath = Path.GetDirectoryName(filePath);

                outputFilePath = Path.GetFullPath(Path.Combine(outputPath, outputFileName));
            }

            if (isDisableBackupFile == false)
            {
                if (outputFilePath == filePath)
                    File.Copy(filePath, filePath.Replace(".srt", ".bak.srt"), true);
            }

            File.WriteAllLines(outputFilePath, subtitles.ToLines(), Encoding.UTF8);

            if (isDisableBackupFile == false)
            {
                string[] errors = GetSubtitlesErrors(subtitles);

                if (errors != null && errors.Length > 0)
                {
                    string errorFile = filePath.Replace(".srt", ".error.srt");
                    File.WriteAllLines(errorFile, errors, Encoding.UTF8);
                }
            }
        }

        private string[] GetSubtitlesErrors(List<Subtitle> subtitles)
        {
            return subtitles.Select((subtitle, index) =>
                new
                {
                    subtitle,
                    index,
                    HasErrors = subtitle.HasErrors()
                }
            )
            .Where(x => x.HasErrors)
            .SelectMany(x => x.subtitle.ToLines(x.index))
            .ToArray();
        }

        private void Print(List<Subtitle> subtitles)
        {
            foreach (var line in subtitles.ToLines())
                Console.WriteLine(line);
        }
    }
}
