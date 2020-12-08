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
            bool isReport = false;

            if (isPrint)
            {
                HandleSubtitles(new Options()
                {
                    path = filePath,
                    print = true,
                    clean = true
                });
            }
            else if (isReport)
            {
                HandleSubtitles(new Options()
                {
                    path = @"G:\Movies\Seen",
                    report = true
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
            if ((options.print || options.save || options.report) == false)
                return;

            if (string.IsNullOrEmpty(options.path))
                return;

            if (options.path.EndsWith(":\""))
                options.path = options.path.Replace(":\"", ":");

            bool isRecursive = options.report;
            string[] filePaths = GetFiles(options.path, isRecursive);

            if (filePaths == null || filePaths.Length == 0)
                return;

            if (options.subtitlesOrder)
            {
                foreach (var filePath in filePaths)
                    SetSubtitlesOrder(options, filePath);
            }
            else if (options.report)
            {
                List<string> reportLines = new List<string>();

                foreach (var filePath in filePaths)
                    ReportSubtitles(reportLines, filePath);

                if (reportLines.Count == 0)
                {
                    reportLines.Add("All files are clean");
                    reportLines.Add(options.path);
                }

                string reportFile = @"E:\My Downloads\srt report.txt";
                File.WriteAllLines(reportFile, reportLines, Encoding.UTF8);
            }
            else
            {
                foreach (var filePath in filePaths)
                    CleanSubtitles(options, filePath);
            }
        }

        private void CleanSubtitles(Options options, string filePath)
        {
            List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(filePath);

            if (options.clean)
                subtitles = subtitles.CleanSubtitles();

            if (options.cleanHICaseInsensitive)
                subtitles = subtitles.CleanHICaseInsensitive();

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

        private static readonly string[] reportExcludedFiles = new string[]
        {
            "אפס ביחסי אנוש.srt",
            "גבעת חלפון אינה עונה.srt",
            "חתונה מאוחרת.srt",
            "ללכת על המים.srt"
        };

        private void ReportSubtitles(List<string> reportLines, string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            if (reportExcludedFiles.Contains(fileName))
                return;

            Console.WriteLine(fileName);

            List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(filePath);
            List<Subtitle> subtitlesCleaned = subtitles.Clone().CleanSubtitles();

            List<string> lines = new List<string>();

            if (subtitles.Count != subtitlesCleaned.Count)
            {
                lines.Add("Number of subtitles are not the same");
                lines.Add("Original file: " + subtitles.Count + " subtitles");
                lines.Add("Cleaned file: " + subtitlesCleaned.Count + " subtitles");
            }
            else
            {
                for (int i = 0; i < subtitles.Count; i++)
                {
                    string s = subtitles[i].ToString();
                    string sc = subtitlesCleaned[i].ToString();
                    if (s != sc)
                    {
                        int subtitleNumber = i + 1;
                        lines.Add("Original file:");
                        lines.Add(subtitleNumber.ToString());
                        lines.Add(s);
                        lines.Add(string.Empty);
                        lines.Add("Cleaned file:");
                        lines.Add(subtitleNumber.ToString());
                        lines.Add(sc);
                        lines.Add("---------------");
                    }
                }
            }

            string[] errors = GetSubtitlesErrors(subtitles);

            if (errors != null && errors.Length > 0)
            {
                lines.Add(string.Empty);
                lines.Add("Errors:");
                lines.Add(fileName);
                lines.Add(string.Empty);
                lines.AddRange(errors);
                lines.Add(string.Empty);
            }

            if (lines.Count > 0)
            {
                reportLines.Add(fileName);
                reportLines.Add(string.Empty);

                reportLines.AddRange(lines);

                reportLines.Add(string.Empty);
                reportLines.Add("***********************************");
                reportLines.Add(string.Empty);
            }
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
            Console.OutputEncoding = Encoding.UTF8;

            foreach (var line in subtitles.ToLines())
                Console.WriteLine(line);
        }
    }
}
